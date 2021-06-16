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
		private static readonly Version MinimumStratumVersion = new(1, 0, 0);

		private static readonly string[] BepinexSystems =
		{
			"core",
			"patchers",
			"monomod",
			"plugins"
		};
		private static readonly Regex NameRegex = new("^[a-zA-Z0-9_]+$");

		private readonly IDisposable _refs;
		private readonly JsonSerializer _serializer = JsonSerializer.Create(new()
		{
			Converters = new JsonConverter[]
			{
				new PackageReferenceConverter(),
				new MarkedConverter()
			},
			ContractResolver = new DefaultContractResolver()
			{
				NamingStrategy = new SnakeCaseNamingStrategy()
			}
		});
		private readonly IProjectParser _parser;
		private readonly Generator _generator;

		private static IAssemblyResolver CreateResolver(CompilerParameters parameters)
		{
			var resolver = new DefaultAssemblyResolver();

			resolver.AddSearchDirectory(parameters.ManagedDirectory);

			var bepInEx = parameters.BepInExDirectory;
			foreach (var system in BepinexSystems)
			{
				var dir = Path.Combine(bepInEx, system);
				if (!Directory.Exists(dir))
					continue;

				resolver.AddSearchDirectory(dir);

				foreach (var subdir in Directory.GetDirectories(dir, "*", SearchOption.AllDirectories))
					resolver.AddSearchDirectory(subdir);
			}

			return resolver;
		}

		public string? ManifestPath { get; private set; }

		public string? ProjectPath { get; private set; }

		public Compiler(CompilerParameters parameters)
		{
			var resolver = CreateResolver(parameters);
			var refsOwner = new RootRefsOwner(parameters, resolver);

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

		private bool AddStratumDependency(Metadata metadata, string projectFile, CompilerOutput output)
		{
			var deps = metadata.Dependencies;
			foreach (var dep in deps.OrEmptyIfNull())
			{
				var value = dep.Value;
				if (value.DependencyGUID != StratumGUID)
					continue;

				if (!value.Flags.HasFlag(BepInDependency.DependencyFlags.HardDependency))
				{
					output.Failure(MarkupMessage.File(projectFile, dep.Range, "Stratum cannot be a soft dependency"));
					return false;
				}

				var depVersion = value.MinimumVersion;
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
					default: throw new ArgumentOutOfRangeException();
				}
			}

			deps = metadata.Dependencies = new List<Marked<BepInDependency>>();

			{
				var stratumDep = new BepInDependency(StratumGUID, MinimumStratumVersion.ToString());

				deps.Add(new(stratumDep, default));
			}

			return true;
		}

		public ICompilerOutput? Compile(string projectDirectory, Stream ret)
		{
			if (!Directory.Exists(projectDirectory))
				throw new DirectoryNotFoundException("Directory does not exist.");

			var manifestFile = ManifestPath = Path.Combine(projectDirectory, "manifest.json");
			if (!File.Exists(manifestFile))
				return null;

			var projectFile = ProjectPath = Path.Combine(projectDirectory, "project.yaml");
			if (!File.Exists(projectFile))
				return null;

			var output = new CompilerOutput();
			Manifest manifest;
			{
				using var text = new StreamReader(manifestFile);
				using var json = new JsonTextReader(text);

				try
				{
					manifest = _serializer.Deserialize<Manifest>(json)!;
				}
				catch (JsonSerializationException e)
				{
					output.Failure(MarkupMessage.File(manifestFile, e.GetIndex(), e.Message));
					return output;
				}

				var name = manifest.Name;
				if (!NameRegex.IsMatch(name.Value))
				{
					output.Failure(MarkupMessage.File(manifestFile, name.Range, "Names may only contain the characters a-z A-Z 0-9 _"));
					return output;
				}

				// We don't validate author because we cannot ensure the author in the file is the same as the uploader, + author is an
				// r2mm property, not TS
			}

			Mod ir;
			{
				using var text = new StreamReader(projectFile);
				var parser = new MergingParser(new Parser(text));

				var irBuffer = _parser.Parse(manifest, manifestFile, parser, projectFile, projectDirectory, output);
				if (irBuffer is null)
					return output;
				ir = irBuffer;
			}

			if (!AddStratumDependency(ir.Metadata, projectFile, output))
				return output;

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
