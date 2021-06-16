using System;
using Mason.Core.Thunderstore;
using Newtonsoft.Json;

namespace Mason.Core.Parsing.Thunderstore
{
	internal class PackageReferenceConverter : JsonConverter<PackageReference>
	{
		public override void WriteJson(JsonWriter writer, PackageReference value, JsonSerializer serializer) =>
			throw new NotSupportedException();

		public override PackageReference ReadJson(JsonReader reader, Type objectType, PackageReference existingValue, bool hasExistingValue,
			JsonSerializer serializer)
		{
			var line = reader as IJsonLineInfo;

			Exception NewException(string message) => throw new JsonSerializationException(message, reader.Path, line?.LineNumber ?? 0, line?.LinePosition ?? 0, null);

			var scalar = reader.Value as string ?? throw NewException("Dependencies cannot be null.");
			var split = scalar.Split('-');
			if (split.Length != 3)
				throw NewException("A dependency string must be the author, name, and version, delimited by a hyphen (-)");

			return new(split[0], split[1], new(split[2]));
		}
	}
}
