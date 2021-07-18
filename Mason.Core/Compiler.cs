using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using BepInEx;
using Mason.Core.IR;
using Mason.Core.Markup;
using Mason.Core.Parsing.Projects;
using Mason.Core.Parsing.Projects.v1;
using Mason.Core.Parsing.Thunderstore;
using Mason.Core.RefsAndDefs;
using Mason.Core.Thunderstore;
using Mono.Cecil;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using YamlDotNet.Core;
using Version = System.Version;

namespace Mason.Core
{
	public class Compiler
	{
		private const string StratumGUID = "stratum";

		private static readonly string[] BepinexSystems =
		{
			"core", "patchers", "monomod", "plugins"
		};

		private static readonly Version MinimumStratumVersion = new(1, 0, 0);

		internal static readonly Regex ComponentRegex = new("^[a-zA-Z0-9](?:[a-zA-Z0-9_]*[a-zA-Z0-9])?$");

		private static IAssemblyResolver CreateResolver(CompilerParameters parameters)
		{
			DefaultAssemblyResolver resolver = new();

			resolver.AddSearchDirectory(parameters.ManagedDirectory);

			string bepInEx = parameters.BepInExDirectory;
			foreach (string system in BepinexSystems)
			{
				string dir = Path.Combine(bepInEx, system);
				if (!Directory.Exists(dir))
					continue;

				resolver.AddSearchDirectory(dir);

				foreach (string subdir in Directory.GetDirectories(dir, "*", SearchOption.AllDirectories))
					resolver.AddSearchDirectory(subdir);
			}

			return resolver;
		}

		private readonly Generator _generator;
		private readonly IProjectParser _parser;

		private readonly IDisposable _refs;

		private readonly JsonSerializer _serializer = JsonSerializer.Create(new JsonSerializerSettings
		{
			Converters = new JsonConverter[]
			{
				new PackageReferenceConverter(), new MarkedConverter(), new NullableConverter()
			},
			ContractResolver = new DefaultContractResolver
			{
				NamingStrategy = new SnakeCaseNamingStrategy()
			}
		});

		public Compiler(CompilerParameters parameters)
		{
			IAssemblyResolver resolver = CreateResolver(parameters);
			RootRefsOwner refsOwner = new(parameters, resolver);

			_refs = refsOwner;
			_parser = new AgnosticProjectParser
			{
				Versions =
				{
					[1] = new V1ProjectParser()
				}
			};
			_generator = new Generator(resolver, refsOwner.Refs);
		}

		public string? ManifestPath { get; private set; }

		public string? ProjectPath { get; private set; }

		private bool AddStratumDependency(Metadata metadata, string projectFile, CompilerOutput output)
		{
			IList<Marked<BepInDependency>>? deps = metadata.Dependencies;
			foreach (Marked<BepInDependency> dep in deps.OrEmptyIfNull())
			{
				BepInDependency value = dep.Value;
				if (value.DependencyGUID != StratumGUID)
					continue;

				if (!value.Flags.HasFlag(BepInDependency.DependencyFlags.HardDependency))
				{
					output.Failure(MarkupMessage.File(projectFile, dep.Range, "Stratum cannot be a soft dependency"));
					return false;
				}

				Version? depVersion = value.MinimumVersion;
				switch (depVersion.GoodCompareTo(MinimumStratumVersion))
				{
					case 1:
						output.Warnings.Add(MarkupMessage.File(projectFile, dep.Range,
							$"Stratum dependency with a minimum version ({depVersion}) greater than required ({MinimumStratumVersion})"));
						return true;
					case 0:
						output.Warnings.Add(MarkupMessage.File(projectFile, dep.Range, "Redundant Stratum dependency"));
						return true;
					case -1:
						output.Failure(MarkupMessage.File(projectFile, dep.Range,
							$"Stratum dependency with a minimum version ({depVersion}) less than required ({MinimumStratumVersion})"));
						return false;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}

			deps = metadata.Dependencies = new List<Marked<BepInDependency>>();

			{
				BepInDependency stratumDep = new(StratumGUID, MinimumStratumVersion.ToString());

				deps.Add(new Marked<BepInDependency>(stratumDep, default));
			}

			return true;
		}

		public ICompilerOutput? Compile(string projectDirectory, Stream ret)
		{
			if (!Directory.Exists(projectDirectory))
				throw new DirectoryNotFoundException("Directory does not exist.");

			string manifestFile = ManifestPath = Path.Combine(projectDirectory, "manifest.json");
			if (!File.Exists(manifestFile))
				return null;

			string projectFile = ProjectPath = Path.Combine(projectDirectory, "project.yaml");
			if (!File.Exists(projectFile))
				return null;

			CompilerOutput output = new();
			Manifest manifest;
			{
				using StreamReader text = new(manifestFile);
				using JsonTextReader json = new(text);

				try
				{
					manifest = _serializer.Deserialize<Manifest>(json)!;
				}
				catch (JsonSerializationException e)
				{
					output.Failure(MarkupMessage.File(manifestFile, e.GetIndex(), e.Message));
					return output;
				}

				ICompilerOutput? Validate(Marked<string> component, string name)
				{
					if (ComponentRegex.IsMatch(component.Value))
						return null;

					output.Failure(MarkupMessage.File(manifestFile, component.Range, $"{name} may only contain the characters a-z A-Z 0-9 _ and cannot start or end with _"));
					return output;
				}

				Marked<string> name = manifest.Name;
				ICompilerOutput? earlyRet = Validate(name, "Name");
				if (earlyRet != null)
					return earlyRet;

				Marked<string>? author = manifest.Author;
				if (author.HasValue)
				{
					earlyRet = Validate(author.Value, "Author");
					if (earlyRet != null)
						return earlyRet;
				}
			}

			Mod ir;
			{
				using StreamReader text = new(projectFile);
				MergingParser parser = new(new Parser(text));

				Mod? irBuffer = _parser.Parse(manifest, manifestFile, parser, projectFile, projectDirectory, output);
				if (irBuffer == null)
					return output;
				ir = irBuffer;
			}

			if (!AddStratumDependency(ir.Metadata, projectFile, output))
				return output;

			// TODO: check for stratum incompatibility

			ir = ir.Optimize();

			_generator.Generate(ir).Write(ret);
			ret.Position = 0;

			output.Success(ir.Metadata.Package);

			return output;
		}

		public void Dispose()
		{
			_refs.Dispose();
		}
	}
}
