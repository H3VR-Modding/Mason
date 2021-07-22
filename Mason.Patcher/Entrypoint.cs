using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BepInEx;
using BepInEx.Logging;
using Mason.Core;
using Mason.Core.Markup;
using Mono.Cecil;

namespace Mason.Patcher
{
	public static class Entrypoint
	{
		private static readonly ManualLogSource Logger = BepInEx.Logging.Logger.CreateLogSource("Mason Patcher");

		public static IEnumerable<string> TargetDLLs => Enumerable.Empty<string>();

		public static void Patch(AssemblyDefinition asm)
		{
			Logger.LogWarning("There aren't be any assemblies to patch, but BepInEx called the patch function. Assembly: " + asm);
		}

		public static void Finish()
		{
			Logger.LogDebug("Bootstrapping Mason...");

			CompilerParameters parameters = new(Paths.ManagedPath, Paths.BepInExRootPath);
			Compiler compiler = new(parameters);

			Logger.LogDebug("Constructed compiler");

			foreach (string directory in Directory.GetDirectories(Paths.PluginPath))
			{
				string GetDirectoryName()
				{
					return Path.GetFileName(directory);
				}

				void Error(string message, string? name = null)
				{
					Logger.LogError($"Failed to compile {name ??= GetDirectoryName()}:\n{message}");
				}

				void ErrorMarkup(MarkupMessage markup, string? name = null)
				{
					Error(markup.ToString("error", Path.GetFullPath), name);
				}

				CompilerOutput output;
				try
				{
					output = compiler.Compile(directory);
				}
				catch (FileNotFoundException e)
				{
					if (e.FileName is not { } fileName)
						throw;

					Logger.LogDebug($"Skipping {GetDirectoryName()} because of missing file: '{fileName}'");
					continue;
				}
				catch (CompilerException e)
				{
					ErrorMarkup(e.Markup, e.Package?.Name);
					continue;
				}
				catch (Exception e)
				{
					Error(e.ToString());
					continue;
				}

				IList<MarkupMessage> warnings = output.Warnings;

				if (warnings.Count > 0)
				{
					StringBuilder? builder = new StringBuilder().AppendLine();

					foreach (MarkupMessage warning in warnings)
						builder.AppendLine(warning.ToString("warning", Path.GetFullPath));

					Logger.LogWarning(builder.ToString());
				}

				Logger.LogInfo($"Compiled {output.Package} with {warnings.Count} warnings");

				using FileStream? file = File.Create(Path.Combine(directory, "bootstrap.dll"));
				Logger.LogDebug("Acquired output file");

				output.Bootstrap.CopyTo(file);
				Logger.LogDebug("Wrote content to disk");
			}
		}
	}
}
