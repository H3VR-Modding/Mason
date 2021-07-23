using System;
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
			string scalar = parser.Consume<Scalar>().Value;

			return PackageReferenceNoVersion.Parse(scalar);
		}

		public void WriteYaml(IEmitter emitter, object? value, Type type)
		{
			throw new NotSupportedException();
		}
	}
}
