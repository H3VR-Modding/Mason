using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using Mason.Core;
using Mason.Core.Markup;
using Newtonsoft.Json;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Mason.Standalone
{
	internal class Program
	{
		public static async Task<int> Main(string[] args)
		{
			try
			{
				await MainSafe(args);

				return (int) ExitCode.Ok;
			}
			catch (ExitException e)
			{
				if (e.Markup is { } markup)
					Console.WriteLine(markup.ToString("error", RelativePath));

				return (int) e.Code;
			}
		}

		private static Task MainSafe(string[] args)
		{
			ParserResult<object> result = Parser.Default.ParseArguments<BuildOptions, PackOptions>(args);

			if (result is NotParsed<object>)
				throw new ExitException(ExitCode.Internal);

			var opt = (Options) ((Parsed<object>) result).Value;
			Console.WriteLine("Parsed commandline arguments");

			string projectDir = Path.Combine(Environment.CurrentDirectory, opt.Directory);
			if (!Directory.Exists(projectDir))
				throw new ExitException(ExitCode.ProjectDirectoryDoesNotExist,
					MarkupMessage.Path(projectDir, "The project directory does not exist"));

			FileInfo file = new(opt.Config);
			if (!file.Exists)
				throw new ExitException(ExitCode.MissingConfig, MarkupMessage.Path(file.FullName, "Missing configuration file"));

			Config config = ReadConfig(file);

			Console.WriteLine("Parsed config");

			ValidateConfig(config);

			Program inst = new(config, projectDir);

			return opt switch
			{
				BuildOptions x => inst.Build(x),
				PackOptions x => inst.Pack(x),
				_ => throw new ExitException(ExitCode.Internal)
			};
		}

		private static void ValidateConfig(Config config)
		{
			{
				string dir = config.Directories.Bepinex;
				if (!Directory.Exists(dir))
					throw new ExitException(ExitCode.BepInExDirectoryNotFound,
						MarkupMessage.Path(dir, "Could not find the BepInEx directory"));
			}

			{
				string dir = config.Directories.Managed;
				if (!Directory.Exists(dir))
					throw new ExitException(ExitCode.ManagedDirectoryNotFound,
						MarkupMessage.Path(dir, "Could not find the Markup directory"));
			}
		}

		private static Config ReadConfig(FileInfo file)
		{
			using StreamReader reader = file.OpenText();

			IDeserializer deserializer = new DeserializerBuilder()
				.WithNamingConvention(UnderscoredNamingConvention.Instance)
				.Build();

			var config = deserializer.Deserialize<Config>(reader);
			config.ResolvePaths(file.DirectoryName ?? throw new IOException("Config file was not in a directory."));

			return config;
		}

		private static async Task Write(Stream content, string dest)
		{
			string? directory = Path.GetDirectoryName(Path.GetFullPath(dest));
			if (!string.IsNullOrEmpty(directory))
				Directory.CreateDirectory(directory);

			await using FileStream file = File.Create(dest);
			Console.WriteLine("Acquired output file");

			await content.CopyToAsync(file);
			Console.WriteLine("Wrote content to disk");
		}

		private static string RelativePath(string path)
		{
			return Path.GetRelativePath(Environment.CurrentDirectory, path);
		}

		private readonly Config _config;
		private readonly string _dir;

		private Program(Config config, string dir)
		{
			_config = config;
			_dir = dir;
		}

		private async Task Build(BuildOptions opt)
		{
			using CompilerOutput output = Compile();
			await Write(output.Bootstrap, opt.Output);
		}

		private async Task Pack(PackOptions opt)
		{
			MemoryStream archive;
			{
				using CompilerOutput output = Compile();

				Console.WriteLine("Creating package");
				archive = await Zip(output, opt.Compression);
			}

			await using (archive)
				await Write(archive, opt.Output);
		}

		private async Task<MemoryStream> Zip(CompilerOutput output, CompressionLevel compression)
		{
			const string pluginsPrefix = "files/plugins/";

			DateTimeOffset now = DateTimeOffset.Now;
			MemoryStream backing = new();
			using (ZipArchive archive = new(backing, ZipArchiveMode.Create, true))
			{
				async Task CreateEntry(string path, Stream content)
				{
					ZipArchiveEntry entry = archive.CreateEntry(path, compression);
					entry.LastWriteTime = now;
					entry.ExternalAttributes = ZipUtils.ExternalAttributes;

					await using Stream compressed = entry.Open();
					await content.CopyToAsync(compressed);
				}

				await CreateEntry(pluginsPrefix + "bootstrap.dll", output.Bootstrap);
				Console.WriteLine("Added bootstrap DLL");

				{
					await using MemoryStream manifest = new();
					{
						await using StreamWriter text = new(manifest, leaveOpen: true);
						using JsonTextWriter json = new(text);

						Compiler.ManifestSerializer.Serialize(json, output.Manifest);
					}
					manifest.Position = 0;

					await CreateEntry("manifest.json", backing);
				}
				Console.WriteLine("Added minified manifest");

				foreach (string file in new[]
				{
					"README.md", "icon.png"
				})
				{
					if (!File.Exists(file))
						throw new ExitException(ExitCode.MissingThunderstoreFiles,
							MarkupMessage.Path(file, "Missing file is required for a Thunderstore package"));

					archive.AddFile(file, file, compression);
				}
				Console.WriteLine("Added Thunderstore-required files");

				AddResources(archive, pluginsPrefix, output.ReferencedPaths, compression);
				Console.WriteLine("Added resources");
			}
			backing.Position = 0;

			Console.WriteLine("Constructed archive");

			return backing;
		}

		private void AddResources(ZipArchive archive, string root, IEnumerable<string> paths, CompressionLevel compression)
		{
			StringBuilder builder = new();
			HashSet<string> entries = new();
			foreach (string path in paths)
			{
				const string resources = Compiler.ResourcesDirectory;

				string zipDir = root + resources + "/";
				string realPath = Path.Combine(resources, path);

				if (File.Exists(realPath))
				{
					if (entries.Contains(path))
						continue;

					archive.AddFile(realPath, zipDir + path, compression);
					entries.Add(path);
				}
				else if (Directory.Exists(realPath))
				{
					if (entries.Contains(path))
						continue;

					builder.Append(zipDir).Append(path);
					archive.CreateEntry(builder.ToString());
					archive.AddDirectory(realPath, compression, builder, entries);
					builder.Clear();

					entries.Add(path);
				}
				else
				{
					throw new IOException("Neither a file nor a directory was referenced");
				}
			}
		}

		private CompilerOutput Compile()
		{
			CompilerParameters parameters = new(_config.Directories.Managed, _config.Directories.Bepinex);
			Compiler compiler = new(parameters);
			Console.WriteLine("Constructed compiler");

			CompilerOutput output;
			try
			{
				output = compiler.Compile(_dir);
			}
			catch (FileNotFoundException e)
			{
				if (e.FileName is not { } fileName)
					throw;

				throw new ExitException(ExitCode.MissingProjectFiles, MarkupMessage.Path(fileName, e.Message));
			}
			catch (CompilerException e)
			{
				throw new ExitException(ExitCode.Compiler, e.Markup);
			}

			IList<MarkupMessage> warnings = output.Warnings;

			foreach (MarkupMessage warning in warnings)
				Console.WriteLine(warning.ToString("warning", RelativePath));

			Console.WriteLine($"Compiled {output.Package} with {warnings.Count} warnings");

			return output;
		}
	}
}
