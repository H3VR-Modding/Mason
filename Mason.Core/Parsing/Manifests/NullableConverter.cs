using System;
using Newtonsoft.Json;

namespace Mason.Core.Parsing.Thunderstore
{
	public class NullableConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
		{
			throw new NotSupportedException();
		}

		public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.Null)
				return Activator.CreateInstance(objectType);

			Type valueType = objectType.GetGenericArguments()[0];
			object? value = serializer.Deserialize(reader, valueType);
			return Activator.CreateInstance(objectType, value);
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(Nullable<>);
		}
	}
}
