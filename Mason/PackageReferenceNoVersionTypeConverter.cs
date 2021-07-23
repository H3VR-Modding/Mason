using System;
using Mason.Core;
using Mason.Core.Thunderstore;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Mason.Standalone
{
	internal class PackageReferenceNoVersionTypeConverter : IYamlTypeConverter
	{
		public bool Accepts(Type type)
		{
			return type == typeof(PackageReferenceNoVersion);
		}

		public object? ReadYaml(IParser parser, Type type)
		{
			var scalar = parser.Consume<Scalar>();

			return scalar.IsNull() ? null : PackageReferenceNoVersion.Parse(scalar.Value);
		}

		public void WriteYaml(IEmitter emitter, object? value, Type type)
		{
			throw new NotSupportedException();
		}
	}
}
