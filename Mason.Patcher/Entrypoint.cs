using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BepInEx;
using BepInEx.Logging;
using Mason.Core;
using Mason.Core.Markup;
using Mason.Core.Thunderstore;
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

			using MemoryStream buffer = new(2 * 4096);

			foreach (string directory in Directory.GetDirectories(Paths.PluginPath))
			{
				string GetDirectoryName() => Path.GetFileName(directory);

				buffer.SetLength(0);

				ICompilerOutput? output;
				try
				{
					output = compiler.Compile(directory, buffer);
				}
				catch (Exception e)
				{
					Logger.LogError($"Failed to compile {GetDirectoryName()}:\n{e}");
					continue;
				}

				if (output == null)
					continue;

				if (!output.MatchSuccess(out MarkupMessage? error, out PackageReference? package))
				{
					string name = package?.Name ?? GetDirectoryName();

					Logger.LogError($"Failed to compile {name}:\n{error.ToString("error", Path.GetFullPath)}");
				}
				else
				{
					IList<MarkupMessage> warnings = output.Warnings;

					if (warnings.Count > 0)
					{
						StringBuilder? builder = new StringBuilder().AppendLine();

						foreach (MarkupMessage warning in warnings)
							builder.AppendLine(warning.ToString("warning", Path.GetFullPath));

						Logger.LogWarning(builder.ToString());
					}

					Logger.LogInfo($"Compiled {package.Value} with {warnings.Count} warnings");

					using FileStream? file = File.Create(Path.Combine(directory, "bootstrap.dll"));
					Logger.LogDebug("Acquired output file");

					buffer.CopyTo(file);
					Logger.LogDebug("Wrote content to disk");
				}
			}
		}
	}
}
