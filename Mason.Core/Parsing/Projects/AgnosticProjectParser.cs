using System.Collections.Generic;
using Mason.Core.IR;
using Mason.Core.Markup;
using Mason.Core.Thunderstore;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace Mason.Core.Parsing.Projects
{
	internal class AgnosticProjectParser : IProjectParser
	{
		public Dictionary<byte, IProjectParser> Versions { get; } = new();

		public Mod? Parse(Manifest manifest, string manifestFile, IParser project, string projectFile, string directory,
			CompilerOutput output)
		{
			const string tagName = "version";

			project.Consume<StreamStart>();
			project.Consume<DocumentStart>();
			var start = project.Consume<MappingStart>();

			ParsingEvent? c = project.Current;
			MarkupIndex lastIndex = start.End.GetIndex();

			{
				MarkupRange? range = null;

				if (c is null)
					range = new MarkupRange(default, lastIndex);
				else if (c is not Scalar {Value: tagName})
					range = c.GetRange();
				else
					lastIndex = c.End.GetIndex();

				if (range.HasValue)
				{
					output.Failure(MarkupMessage.File(projectFile, range.Value,
						"Template files must begin with the '" + tagName + "' property."));
					return null;
				}
			}

			byte numeric = default;
			MarkupRange valueRange = default;
			{
				MarkupRange? range = null;

				if (!project.MoveNext())
				{
					range = new MarkupRange(lastIndex, lastIndex);
				}
				else
				{
					c = project.Current;

					if (c is null)
						range = new MarkupRange(lastIndex, lastIndex);
					else if (c is not Scalar value || !byte.TryParse(value.Value, out numeric))
						range = c.GetRange();
					else
						valueRange = c.GetRange();
				}

				if (range.HasValue)
				{
					output.Failure(
						MarkupMessage.File(projectFile, range.Value, "The '" + tagName + "' property must have a numeric value."));
					return null;
				}
			}

			if (!Versions.TryGetValue(numeric, out IProjectParser version))
			{
				output.Failure(MarkupMessage.File(projectFile, valueRange,
					$"Version {numeric} is not supported by this version of Mason."));
				return null;
			}

			project.MoveNext();

			Mod? ret = version.Parse(manifest, manifestFile, new SliceParser(project), projectFile, directory, output);
			if (ret is null)
				return ret;

			project.Consume<MappingEnd>();
			lastIndex = project.Consume<DocumentEnd>().End.GetIndex();

			{
				c = project.Current;

				MarkupRange? range = null;
				if (c is null)
					range = new MarkupRange(lastIndex, lastIndex);
				else if (c is not StreamEnd)
					range = c.GetRange();

				if (range.HasValue)
				{
					output.Failure(MarkupMessage.File(projectFile, range.Value, "Project files may contain only one document."));
					return null;
				}
			}

			project.MoveNext();

			return ret;
		}
	}
}
