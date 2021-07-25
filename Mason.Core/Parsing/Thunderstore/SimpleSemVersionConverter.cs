using System;
using Mason.Core.Thunderstore;
using Newtonsoft.Json;

namespace Mason.Core.Parsing.Thunderstore
{
	internal class SimpleSemVersionConverter : JsonConverter<SimpleSemVersion>
	{
		public override void WriteJson(JsonWriter writer, SimpleSemVersion? value, JsonSerializer serializer)
		{
			writer.WriteValue(value?.ToString());
		}

		public override SimpleSemVersion? ReadJson(JsonReader reader, Type objectType, SimpleSemVersion? existingValue,
			bool hasExistingValue, JsonSerializer serializer)
		{
			Exception NewException(string message)
			{
				var line = reader as IJsonLineInfo;
				throw new JsonSerializationException(message, reader.Path, line?.LineNumber ?? 0, line?.LinePosition ?? 0, null);
			}

			if (reader.Value is not { } obj)
				throw NewException("SemVersions must be a string or null");

			if (obj is not string scalar)
				return null;

			if (SimpleSemVersion.TryParse(scalar) is not { } version)
				throw NewException("Versions must be 3 positive integers, delimited by .");

			return version;
		}
	}
}
