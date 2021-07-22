using System;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Mason.Core.Parsing.Projects
{
	internal abstract class ConstrainedStringTypeConverter<T> : IYamlTypeConverter
	{
		public bool Accepts(Type type)
		{
			return type == typeof(T);
		}

		public object? ReadYaml(IParser parser, Type type)
		{
			string scalar = parser.Consume<Scalar>().Value;

			return Parse(scalar);
		}

		public void WriteYaml(IEmitter emitter, object? value, Type type)
		{
			throw new NotSupportedException();
		}

		protected abstract T Parse(string scalar);
	}
}
