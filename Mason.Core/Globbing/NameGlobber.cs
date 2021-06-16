using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Mason.Core.Globbing
{
	internal class NameGlobber
	{
		private static void ApplyGlobs(string value, int start, int length, StringBuilder result, List<GlobberFactory.NameReplacementEntry> nameReplacementEntries)
		{
			if (length == 0) return;

			foreach (var glob in nameReplacementEntries)
			{
				var match = glob.Filter.Match(value, start, length);
				if (!match.Success) continue;

				if (!glob.MatchAllowed(out var replacement))
				{
					throw new ArgumentException($"Name replacement glob not allowed: '{match.Value}'", nameof(value));
				}

				var groups = match.Groups;
				var groupCount = groups.Count - 1;

				// Parse before the match
				ApplyGlobs(value, start, match.Index - start, result, nameReplacementEntries);

				// Replace match
				if (groupCount == 0)
				{
					result.Append(replacement);
				}
				else
				{
					var parameters = new object[groupCount];
					for (var i = 0; i < parameters.Length; ++i)
					{
						parameters[i] = match.Groups[i + 1].Value;
					}

					result.AppendFormat(replacement, parameters);
				}

				// Parse after the match
				var end = match.Index + match.Length;
				ApplyGlobs(value, end, value.Length - end, result, nameReplacementEntries);

				return;
			}

			var raw = value.Substring(start, length);
			var escaped = Regex.Escape(raw);
			result.Append(escaped);
		}

		private readonly Regex _regex;

		public NameGlobber(string name, List<GlobberFactory.NameReplacementEntry> nameReplacements)
		{
			var builder = new StringBuilder();

			builder.Append('^');
			ApplyGlobs(name, 0, name.Length, builder, nameReplacements);
			builder.Append('$');

			_regex = new Regex(builder.ToString());
		}

		public IEnumerable<string> Globber(string directory) => Directory.GetFileSystemEntries(directory)
			.Where(x => _regex.IsMatch(Path.GetFileName(x)));
	}
}
