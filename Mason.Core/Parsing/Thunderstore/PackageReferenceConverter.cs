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

		public override PackageReference? ReadJson(JsonReader reader, Type objectType, PackageReference? existingValue,
			bool hasExistingValue, JsonSerializer serializer)
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

			try
			{
				return PackageReference.Parse(scalar);
			}
			catch (FormatException e)
			{
				throw NewException(e.Message);
			}
		}
	}
}
