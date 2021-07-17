using System;
using System.Text;

namespace Mason.Core.Markup
{
	public class MarkupMessage
	{
		public static MarkupMessage Path(string path, string message)
		{
			return new(message, new MarkupLocation(path, default));
		}

		public static MarkupMessage File(string path, MarkupIndex index, string message)
		{
			return new(message, new MarkupLocation(path, index));
		}

		public static MarkupMessage File(string path, MarkupRange range, string message)
		{
			return new(message, new MarkupLocation(path, range.Start, range.End));
		}

		private MarkupMessage(string message, MarkupLocation location)
		{
			Message = message;
			Location = location;
		}

		public string Message { get; }

		public MarkupLocation Location { get; }

		public string ToString(string severity, Func<string, string> pathToString)
		{
			StringBuilder builder = new StringBuilder(severity)
				.Append(" at ")
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
				.Append(Message);

			return builder.ToString();
		}
	}
}
