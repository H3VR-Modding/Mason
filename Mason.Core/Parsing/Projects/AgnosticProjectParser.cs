using System.Collections.Generic;
using Mason.Core.Markup;
using Mason.Core.Projects;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace Mason.Core.Parsing.Projects
{
	internal class AgnosticProjectParser : IProjectParser
	{
		public Dictionary<byte, IProjectParser> Versions { get; } = new();

		public ParserOutput Parse(UnparsedProject project)
		{
			const string tagName = "version";

			IParser parser = project.Parser;

			parser.Consume<StreamStart>();
			parser.Consume<DocumentStart>();
			var start = parser.Consume<MappingStart>();

			ParsingEvent? c = parser.Current;
			MarkupIndex lastIndex = start.End.GetIndex();

			{
				MarkupRange? range = null;

				if (c == null)
					range = new MarkupRange(default, lastIndex);
				else if (c is not Scalar {Value: tagName})
					range = c.GetRange();
				else
					lastIndex = c.End.GetIndex();

				if (range.HasValue)
					throw new CompilerException(MarkupMessage.File(project.Path, range.Value,
						"Project files must begin with the '" + tagName + "' property"));
			}

			byte numeric = default;
			MarkupRange valueRange = default;
			{
				MarkupRange? range = null;

				if (!parser.MoveNext())
				{
					range = new MarkupRange(lastIndex, lastIndex);
				}
				else
				{
					c = parser.Current;

					if (c == null)
						range = new MarkupRange(lastIndex, lastIndex);
					else if (c is not Scalar value || !byte.TryParse(value.Value, out numeric))
						range = c.GetRange();
					else
						valueRange = c.GetRange();
				}

				if (range.HasValue)
					throw new CompilerException(MarkupMessage.File(project.Path, range.Value,
						"The '" + tagName + "' property must have a numeric value."));
			}

			if (!Versions.TryGetValue(numeric, out IProjectParser version))
				throw new CompilerException(MarkupMessage.File(project.Path, valueRange,
					$"Version {numeric} is not supported by this version of Mason."));

			parser.MoveNext();

			ParserOutput ret = version.Parse(project.WithParser(new SliceParser(parser)));

			parser.Consume<MappingEnd>();
			lastIndex = parser.Consume<DocumentEnd>().End.GetIndex();

			{
				c = parser.Current;

				MarkupRange? range = null;
				if (c == null)
					range = new MarkupRange(lastIndex, lastIndex);
				else if (c is not StreamEnd)
					range = c.GetRange();

				if (range.HasValue)
					throw new CompilerException(
						MarkupMessage.File(project.Path, range.Value, "Project files may contain only one document"));
			}

			parser.MoveNext();

			return ret;
		}
	}
}
