using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using Mason.Core;
using Mason.Core.Markup;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Mason.Standalone
{
	internal class Program
	{
		private readonly Config _config;
		private readonly string _dir;

		public static async Task<int> Main(string[] args)
		{
			var result = Parser.Default.ParseArguments<BuildOptions, PackOptions>(args);

			if (result is NotParsed<object>)
				return (int) ExitCode.Internal;

			var opt = (Options) ((Parsed<object>) result).Value;
			Console.WriteLine("Parsed commandline arguments");

			var projectDir = Path.Combine(Environment.CurrentDirectory, opt.Directory);
			if (!Directory.Exists(projectDir))
			{
				Error(MarkupMessage.Path(projectDir, $"The project directory does not exist"));
				return (int) ExitCode.ProjectDirectoryDoesNotExist;
			}

			var file = new FileInfo(opt.Config);
			if (!file.Exists)
			{
				Error(MarkupMessage.Path(file.FullName, $"Missing configuration file"));
				return (int) ExitCode.MissingConfig;
			}

			var config = ReadConfig(file);

			Console.WriteLine("Parsed config");

			{
				var dir = config.Directories.Bepinex;
				if (!Directory.Exists(dir))
				{
					Error(MarkupMessage.Path(dir, "Could not find the BepInEx directory"));
					return (int) ExitCode.BepInExDirectoryNotFound;
				}
			}

			{
				var dir = config.Directories.Managed;
				if (!Directory.Exists(dir))
				{
					Error(MarkupMessage.Path(dir, "Could not find the Markup directory"));
					return (int) ExitCode.ManagedDirectoryNotFound;
				}
			}

			var inst = new Program(config, projectDir);
			return (int) await (opt switch
			{
				BuildOptions x => inst.Build(x),
				PackOptions x => inst.Pack(x),
				_ => Task.FromResult(ExitCode.Internal)
			});
		}

		private static Config ReadConfig(FileInfo file)
		{
			using var reader = file.OpenText();

			var deserializer = new DeserializerBuilder()
				.WithNamingConvention(UnderscoredNamingConvention.Instance)
				.Build();

			var config = deserializer.Deserialize<Config>(reader);
			config.ResolvePaths(file.DirectoryName ?? throw new IOException("Config file was not in a directory."));

			return config;
		}

		private static async Task Write(Stream content, string dest)
		{
			await using var file = File.Create(dest);
			Console.WriteLine("Acquired output file");

			await content.CopyToAsync(file);
			Console.WriteLine("Wrote content to disk");
		}

		private Program(Config config, string dir)
		{
			_config = config;
			_dir = dir;
		}

		private async Task<ExitCode> Build(BuildOptions opt)
		{
			MemoryStream buffer;
			{
				var code = Compile(out buffer, out _);
				if (code != ExitCode.None)
					return code;
			}

			await using (buffer)
				await Write(buffer, opt.Output);

			return ExitCode.None;
		}

		private async Task<ExitCode> Pack(PackOptions opt)
		{
			const string pluginsPrefix = "files/plugins/";

			MemoryStream buffer;
			ICompilerOutput output;
			{
				var code = Compile(out buffer, out output!);
				if (code != ExitCode.None)
					return code;
			}

			await using (buffer)
			{
				await using var backing = new MemoryStream();
				using (var archive = new ZipArchive(backing, ZipArchiveMode.Create, true))
				{
					{
						var plugin = archive.CreateEntry(pluginsPrefix + "bootstrap.dll", opt.Compression);
						plugin.LastWriteTime = DateTimeOffset.Now;

						await using var content = plugin.Open();
						await buffer.CopyToAsync(content);
					}

					// TODO: add Stratum dependency to manifest.json before adding to zip

					foreach (var file in new[] {"manifest.json", "README.md", "icon.png"})
					{
						if (!File.Exists(file))
						{
							Error(MarkupMessage.Path(file, "Missing file is required for a Thunderstore package"));
							return ExitCode.MissingThunderstoreFiles;
						}

						archive.AddFile(file, file, opt.Compression);
					}

					var builder = new StringBuilder();
					var entries = new HashSet<string>();
					foreach (var path in output.ReferencedPaths)
					{
						const string resources = "resources";
						const string resourcesDir = resources + "/";
						const string zipDir = pluginsPrefix + resourcesDir;

						var realPath = Path.Combine(resources, path);

						if (File.Exists(realPath))
						{
							if (entries.Contains(path))
								continue;

							archive.AddFile(realPath, zipDir + path, opt.Compression);
							entries.Add(path);
						}
						else if (Directory.Exists(realPath))
						{
							if (entries.Contains(path))
								continue;

							builder.Append(zipDir).Append(path);
							archive.CreateEntry(builder.ToString());
							archive.AddDirectory(realPath, opt.Compression, builder, entries);
							builder.Clear();

							entries.Add(path);
						}
						else
							throw new IOException("Neither a file nor a directory was referenced");
					}
				}

				Console.WriteLine($"Constructed archive");

				backing.Position = 0;
				await Write(backing, opt.Output);
			}

			return ExitCode.None;
		}

		private ExitCode Compile(out MemoryStream buffer, out ICompilerOutput? output)
		{
			buffer = new MemoryStream(2 * 4096);

			var parameters = new CompilerParameters(_config.Directories.Managed, _config.Directories.Bepinex);
			var compiler = new Compiler(parameters);
			Console.WriteLine("Constructed compiler");

			output = compiler.Compile(_dir, buffer);
			if (output is null)
			{
				if (compiler.ProjectPath is { } project)
					Error(MarkupMessage.Path(project, "Cannot compile Mason project without project file"));
				else if (compiler.ManifestPath is { } manifest)
					Error(MarkupMessage.Path(manifest, "Cannot compile Mason project without Thunderstore manifest"));
				else
					Error(MarkupMessage.Path(_dir, "Missing project files"));

				return ExitCode.MissingProjectFiles;
			}

			if (!output.MatchSuccess(out var error, out var package))
			{
				Error(error);
				return ExitCode.Compiler;
			}

			var warnings = output.Warnings;

			foreach (var warning in warnings)
				Console.WriteLine(warning.ToString("warning", RelativePath));

			Console.WriteLine($"Compiled {package.Value} with {warnings.Count} warnings");

			return ExitCode.None;
		}

		private static void Error(MarkupMessage message) => Console.WriteLine(message.ToString("error", RelativePath));

		private static string RelativePath(string path) => Path.GetRelativePath(Environment.CurrentDirectory, path);
	}
}
