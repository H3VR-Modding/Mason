using System;
using Mason.Core.Thunderstore;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Mason.Core.Parsing.Projects
{
	internal abstract class ConstrainedStringTypeConverter<T> : IYamlTypeConverter where T : ConstrainedString<T>
	{
		public bool Accepts(Type type)
		{
			return type == typeof(T);
		}

		public object? ReadYaml(IParser parser, Type type)
		{
			var scalar = parser.Consume<Scalar>();
			if (scalar.IsNull())
				return null;

			return Parse(scalar.Value);
		}

		public void WriteYaml(IEmitter emitter, object? value, Type type)
		{
			throw new NotSupportedException();
		}

		protected abstract T Parse(string scalar);
	}
}
