using System;
using System.Text;

namespace Mason.Core.Markup
{
	public class MarkupMessage
	{
		public static MarkupMessage Path(string path, UnformattedMarkupMessage message, params object[] args)
		{
			return new(new MarkupLocation(path, default), message, args);
		}

		public static MarkupMessage File(string path, MarkupIndex index, UnformattedMarkupMessage message, params object[] args)
		{
			return new(new MarkupLocation(path, index), message, args);
		}

		public static MarkupMessage File(string path, MarkupRange range, UnformattedMarkupMessage message, params object[] args)
		{
			return new(new MarkupLocation(path, range.Start, range.End), message, args);
		}

		private MarkupMessage(MarkupLocation location, UnformattedMarkupMessage unformatted, params object[] args)
		{
			Location = location;
			Unformatted = unformatted;
			Args = args;
		}

		public MarkupLocation Location { get; }

		public UnformattedMarkupMessage Unformatted { get; }

		public object[] Args { get; }

		private void ToString(StringBuilder builder, Func<string, string> pathToString)
		{
			builder
				.Append(pathToString(Location.Path))
				.Append('(')
				.Append(Location.Start.Line)
				.Append(',')
				.Append(Location.Start.Column);
			{
				MarkupIndex? end = Location.End;
				if (end.HasValue)
				{
					MarkupIndex value = end.Value;

					builder
						.Append('-')
						.Append(value.Line)
						.Append(',')
						.Append(value.Column);
				}
			}

			builder
				.Append("): ")
				.Append('[')
				.Append(Unformatted.ID.Scope)
				.Append(Unformatted.ID.Number)
				.Append("] ")
				.AppendFormat(Unformatted.Content, Args);
		}

		public string ToString(Func<string, string> pathToString)
		{
			StringBuilder builder = new();

			ToString(builder, pathToString);

			return builder.ToString();
		}

		public string ToString(string severity, Func<string, string> pathToString)
		{
			StringBuilder builder = new StringBuilder(severity)
				.Append(" at ");

			ToString(builder, pathToString);

			return builder.ToString();
		}
	}
}
