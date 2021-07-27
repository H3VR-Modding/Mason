using System;
using System.IO;
using Mason.Core.IR;
using Mason.Core.Markup;
using Mason.Core.Parsing.Projects;
using Mason.Core.Parsing.Thunderstore;
using Mason.Core.Projects;
using Mason.Core.RefsAndDefs;
using Mason.Core.Thunderstore;
using Mono.Cecil;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using YamlDotNet.Core;
using Version = System.Version;

namespace Mason.Core
{
	public class Compiler : IDisposable
	{
		public const string ResourcesDirectory = "resources";

		private static readonly string[] BepinexSystems =
		{
			"core",
			"patchers",
			"monomod",
			"plugins"
		};

		internal static readonly Version MinimumStratumVersion = new(1, 0, 0);
		internal static readonly GuidString StratumGUID = GuidString.Parse("stratum");

		public static JsonSerializer ManifestSerializer { get; } = JsonSerializer.Create(new JsonSerializerSettings
		{
			Converters = new JsonConverter[]
			{
				new PackageReferenceConverter(),
				new MarkedConverter(),
				new NullableConverter(),
				new PackageComponentStringConverter(),
				new DescriptionStringConverter(),
				new SimpleSemVersionConverter()
			},
			ContractResolver = new DefaultContractResolver
			{
				NamingStrategy = new SnakeCaseNamingStrategy()
			},
			ReferenceLoopHandling = ReferenceLoopHandling.Serialize // Without this, the NullableConverter causes a recursion exception
			// (even though its not trying to serialize a nullable type)
		});

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

		private Manifest ReadManifest(string file)
		{
			using StreamReader text = new(file);
			using JsonTextReader json = new(text);

			Manifest manifest;
			try
			{
				manifest = ManifestSerializer.Deserialize<Manifest>(json) ?? throw new CompilerException(MarkupMessage.File(file,
					default(MarkupIndex), Messages.ManifestNull));
			}
			catch (JsonSerializationException e)
			{
				throw new CompilerException(MarkupMessage.File(file, e.GetIndex(), Messages.ManifestFailedDeserialization, e.Message));
			}

			return manifest;
		}

		public CompilerOutput Compile(string projectDirectory)
		{
			if (!Directory.Exists(projectDirectory))
				throw new DirectoryNotFoundException("Project directory does not exist.");

			string manifestFile = Path.Combine(projectDirectory, "manifest.json");
			if (!File.Exists(manifestFile))
				throw new FileNotFoundException("A Thunderstore manifest is required to compile a Mason mod", manifestFile);

			string projectFile = Path.Combine(projectDirectory, "project.yaml");
			if (!File.Exists(projectFile))
				throw new FileNotFoundException("A Mason project file is required to compile a Mason mod", projectFile);

			Manifest manifest = ReadManifest(manifestFile);

			ParserOutput parsed;
			{
				using StreamReader text = new(projectFile);
				MergingParser parser = new(new Parser(text));

				UnparsedProject project = new(manifest, parser, projectDirectory, projectFile, manifestFile);
				parsed = _parser.Parse(project);
			}

			Mod mod = parsed.Mod;
			mod = mod.Optimize();

			MemoryStream buffer = new();
			try
			{
				_generator.Generate(mod).Write(buffer);

				buffer.SetLength(buffer.Position);
				buffer.Position = 0;

				return new CompilerOutput(buffer, manifest, parsed);
			}
			catch
			{
				buffer.Dispose();

				throw;
			}
		}

		public void Dispose()
		{
			_refs.Dispose();
		}
	}
}
