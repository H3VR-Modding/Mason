using System;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using Version = System.Version;

namespace Mason.Core.Parsing.Projects
{
	public class VersionTypeConverter : IYamlTypeConverter
	{
		public bool Accepts(Type type)
		{
			return type == typeof(Version);
		}

		public object? ReadYaml(IParser parser, Type type)
		{
			var scalar = parser.Consume<Scalar>();
			if (scalar.IsNull())
				return null;

			return new Version(scalar.Value);
		}

		public void WriteYaml(IEmitter emitter, object? value, Type type)
		{
			throw new NotSupportedException();
		}
	}
}
