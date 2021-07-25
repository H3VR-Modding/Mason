using System;
using Mason.Core.Markup;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Mason.Core.Parsing.Projects
{
	internal class MarkupMessageIDTypeConverter : IYamlTypeConverter
	{
		public bool Accepts(Type type)
		{
			return type == typeof(MarkupMessageID);
		}

		public object? ReadYaml(IParser parser, Type type)
		{
			var scalar = parser.Consume<Scalar>();
			if (scalar.IsNull())
				return null;

			string value = scalar.Value;
			if (value.Length < 2)
				throw new FormatException("Markup message IDs must be at least 2 characters in length (scope and numeric)");

			char scope = value[0];

			if (!ushort.TryParse(value.Substring(1, value.Length - 1), out ushort numeric))
				throw new FormatException("The scope of a markup message ID must be immediately followed by its numeric");

			return new MarkupMessageID(scope, numeric);
		}

		public void WriteYaml(IEmitter emitter, object? value, Type type)
		{
			throw new NotSupportedException();
		}
	}
}
