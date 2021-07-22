using System;
using System.Reflection;
using Mason.Core.Markup;
using Newtonsoft.Json;

namespace Mason.Core.Parsing.Thunderstore
{
	internal class MarkedConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
		{
			// Nullability ignored: value must be a struct (Marked<T>)
			Type type = value!.GetType();
			Type valueType = type.GetGenericArguments()[0];
			MethodInfo valueGetter = type.GetProperty(nameof(Marked<object>.Value))!.GetGetMethod();

			object valueValue = valueGetter.Invoke(value, new object[0]);
			serializer.Serialize(writer, valueValue, valueType);
		}

		public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
		{
			var info = reader as IJsonLineInfo;
			Type valueType = objectType.GetGenericArguments()[0];

			MarkupIndex start = info?.GetIndex() ?? default;
			object? value = serializer.Deserialize(reader, valueType);
			MarkupIndex end = info?.GetIndex() ?? default;

			return Activator.CreateInstance(objectType, value, new MarkupRange(start, end));
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(Marked<>);
		}
	}
}
