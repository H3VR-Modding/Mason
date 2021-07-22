using System;
using Mason.Core.Thunderstore;
using Newtonsoft.Json;

namespace Mason.Core.Parsing.Thunderstore
{
	internal class PackageReferenceConverter : JsonConverter<PackageReference>
	{
		public override void WriteJson(JsonWriter writer, PackageReference? value, JsonSerializer serializer)
		{
			writer.WriteValue(value?.ToString());
		}

		public override PackageReference? ReadJson(JsonReader reader, Type objectType, PackageReference? existingValue, bool hasExistingValue,
			JsonSerializer serializer)
		{
			Exception NewException(string message)
			{
				var line = reader as IJsonLineInfo;
				throw new JsonSerializationException(message, reader.Path, line?.LineNumber ?? 0, line?.LinePosition ?? 0, null);
			}

			if (reader.Value is not { } obj)
				throw NewException("Package references must be a string or null");

			if (obj is not string scalar)
				return null;

			string[] split = scalar.Split('-');
			if (split.Length != 3)
				throw NewException("A dependency string must be the author, name, and version, delimited by a hyphen (-)");

			if (PackageComponentString.TryParse(split[0]) is not { } author)
				throw NewException("Authors may only have the characters a-z A-Z 0-9 _ and may not start or end with _");
			if (PackageComponentString.TryParse(split[1]) is not { } name)
				throw NewException("Names may only have the characters a-z A-Z 0-9 _ and may not start or end with _");
			if (SimpleSemVersion.TryParse(split[2]) is not { } version)
				throw NewException("Versions must be 3 positive integers, delimited by .");

			return new PackageReference(author, name, version);
		}
	}
}
