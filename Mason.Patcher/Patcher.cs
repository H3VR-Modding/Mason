using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BepInEx;
using BepInEx.Logging;
using Mason.Core;
using Mason.Core.Markup;

namespace Mason.Patcher
{
	public class Patcher
	{
		private readonly Compiler _compiler;
        private readonly ManualLogSource _logger;

        public Patcher(Compiler compiler, ManualLogSource logger)
        {
	        _compiler = compiler;
	        _logger = logger;
        }

        public void Run()
        {
	        string[] candidates = Directory.GetDirectories(Paths.PluginPath);
	        _logger.LogInfo($"Found {candidates.Length} possible projects");

			foreach (string directory in candidates)
			{
				try
				{
					Compile(directory);
				}
				catch (Exception e)
				{
					_logger.LogError($"Failed to compile directory: '{directory}'\n{e}");
				}
			}
		}

		private void Compile(string directory)
		{
			string GetDirectoryName()
            {
            	return Path.GetFileName(directory);
            }

            void Error(string message, string? name = null)
            {
            	_logger.LogError($"Failed to compile {name ??= GetDirectoryName()}:\n{message}");
            }

            void ErrorMarkup(MarkupMessage markup, string? name = null)
            {
            	Error(markup.ToString("error", Path.GetFullPath), name);
            }

            CompilerOutput output;
            try
            {
            	output = _compiler.Compile(directory);
            }
            catch (FileNotFoundException e)
            {
            	if (e.FileName is not { } fileName)
            		throw;

            	_logger.LogDebug($"Skipping {GetDirectoryName()} because of missing file: '{fileName}'");
            	return;
            }
            catch (CompilerException e)
            {
            	ErrorMarkup(e.Markup, e.Package?.Name);
                return;
            }
            catch (Exception e)
            {
            	Error(e.ToString());
                return;
            }

            using (output)
				PostCompile(directory, output);
		}

		private void PostCompile(string directory, CompilerOutput output)
		{
			IList<MarkupMessage> warnings = output.Warnings;

			if (warnings.Count > 0)
			{
				StringBuilder builder = new();

				foreach (MarkupMessage warning in warnings)
					builder.AppendLine().Append(warning.ToString("warning", Path.GetFullPath));

				_logger.LogWarning(builder.ToString());
			}

			_logger.LogInfo($"Compiled {output.Package} with {warnings.Count} warnings");

			using FileStream file = File.Create(Path.Combine(directory, "bootstrap.dll"));
			_logger.LogDebug("Acquired output file");

			output.Bootstrap.CopyTo(file);
			_logger.LogDebug("Wrote content to disk");
		}
	}
}
