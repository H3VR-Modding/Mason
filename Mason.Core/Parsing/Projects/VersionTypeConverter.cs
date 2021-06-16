using System;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using Version = System.Version;

namespace Mason.Core.Parsing.Projects
{
	internal class VersionTypeConverter : IYamlTypeConverter
	{
		public bool Accepts(Type type) => type == typeof(Version);

		public object ReadYaml(IParser parser, Type type)
		{
			var scalar = parser.Consume<Scalar>();

			return new Version(scalar.Value);
		}

		public void WriteYaml(IEmitter emitter, object? value, Type type) => throw new NotSupportedException();
	}
}
