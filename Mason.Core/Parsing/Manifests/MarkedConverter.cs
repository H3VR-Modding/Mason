using System;
using Mason.Core.Markup;
using Newtonsoft.Json;

namespace Mason.Core.Parsing.Thunderstore
{
	internal class MarkedConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
		{
			throw new NotSupportedException();
		}

		public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
		{
			var info = reader as IJsonLineInfo;

			MarkupIndex start = info?.GetIndex() ?? default;
			object? value = serializer.Deserialize(reader, objectType.GetGenericArguments()[0]);
			MarkupIndex end = info?.GetIndex() ?? default;

			return Activator.CreateInstance(objectType, value, new MarkupRange(start, end));
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(Marked<>);
		}
	}
}
