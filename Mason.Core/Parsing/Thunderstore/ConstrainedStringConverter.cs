using System;
using Mason.Core.Thunderstore;
using Newtonsoft.Json;

namespace Mason.Core.Parsing.Thunderstore
{
	internal abstract class ConstrainedStringConverter<T> : JsonConverter<T> where T : ConstrainedString<T>
	{
		public override void WriteJson(JsonWriter writer, T? value, JsonSerializer serializer)
		{
			writer.WriteValue(value?.ToString());
		}

		public override T? ReadJson(JsonReader reader, Type objectType, T? existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			Exception NewException(string message)
			{
				var line = reader as IJsonLineInfo;
				throw new JsonSerializationException(message, reader.Path, line?.LineNumber ?? 0, line?.LinePosition ?? 0, null);
			}

			if (reader.Value is not { } obj)
				throw NewException($"{typeof(T)} values must be a string or null");

			return obj is string scalar ? Parse(scalar, NewException) : null;
		}

		protected abstract T Parse(string scalar, Func<string, Exception> exception);
	}
}
