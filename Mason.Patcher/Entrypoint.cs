using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BepInEx;
using BepInEx.Logging;
using Mason.Core;
using Mono.Cecil;

namespace Mason.Patcher
{
	public static class Entrypoint
	{
		private static ManualLogSource _logger = Logger.CreateLogSource("Mason Patcher");

		public static IEnumerable<string> TargetDLLs => Enumerable.Empty<string>();

		public static void Patch(AssemblyDefinition asm)
		{
			_logger.LogWarning("There aren't be any assemblies to patch, but BepInEx called the patch function. Assembly: " + asm);
		}

		public static void Finish()
		{
			_logger.LogDebug("Bootstrapping Mason...");

			var parameters = new CompilerParameters(Paths.ManagedPath, Paths.BepInExRootPath);
			var compiler = new Compiler(parameters);

			_logger.LogDebug("Constructed compiler");

			using var buffer = new MemoryStream(2 * 4096);

			foreach (var directory in Directory.GetDirectories(Paths.PluginPath))
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
					_logger.LogError($"Failed to compile {GetDirectoryName()}:\n{e}");
					continue;
				}

				if (output is null)
					continue;

				if (!output.MatchSuccess(out var error, out var package))
				{
					var name = package?.Name ?? GetDirectoryName();

					_logger.LogError($"Failed to compile {name}:\n{error.ToString("error", Path.GetFullPath)}");
				}
				else
				{
					var warnings = output.Warnings;

					if (warnings.Count > 0)
					{
						var builder = new StringBuilder().AppendLine();

						foreach (var warning in warnings)
							builder.AppendLine(warning.ToString("warning", Path.GetFullPath));

						_logger.LogWarning(builder.ToString());
					}

					_logger.LogInfo($"Compiled {package.Value} with {warnings.Count} warnings");

					using var file = File.Create(Path.Combine(directory, "bootstrap.dll"));
					_logger.LogDebug("Acquired output file");

					buffer.CopyTo(file);
					_logger.LogDebug($"Wrote content to disk");
				}
			}
		}
	}
}
