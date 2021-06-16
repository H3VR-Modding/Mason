using System;
using System.Text;

namespace Mason.Core.Markup
{
	public class MarkupMessage
	{
		public static MarkupMessage Path(string path, string message) => new(message, new(path, default));
		public static MarkupMessage File(string path, MarkupIndex index, string message) => new(message, new(path, index));
		public static MarkupMessage File(string path, MarkupRange range, string message) => new(message, new(path, range.Start, range.End));

		public string Message { get; }

		public MarkupLocation Location { get; }

		private MarkupMessage(string message, MarkupLocation location)
		{
			Message = message;
			Location = location;
		}

		public string ToString(string severity, Func<string, string> pathToString)
		{
			var builder = new StringBuilder(severity)
				.Append(" at ")
				.Append(pathToString(Location.Path))
				.Append('(')
				.Append(Location.Start.Line)
				.Append(',')
				.Append(Location.Start.Column);
			{
				var end = Location.End;
				if (end.HasValue)
				{
					var value = end.Value;

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
