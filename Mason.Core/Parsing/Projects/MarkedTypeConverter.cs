using System;
using Mason.Core.Markup;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Mason.Core.Parsing.Projects
{
	internal class MarkedTypeConverter : IYamlTypeConverter
	{
		private readonly Box<IDeserializer> _deserializer;

		public MarkedTypeConverter(Box<IDeserializer> deserializer)
		{
			_deserializer = deserializer;
		}

		public bool Accepts(Type type)
		{
			return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Marked<>);
		}

		public object? ReadYaml(IParser parser, Type type)
		{
			MarkParser marker = new(parser);
			object? value = _deserializer.Value.Deserialize(marker, type.GetGenericArguments()[0]);

			return Activator.CreateInstance(type, value, new MarkupRange(marker.Start.GetIndex(), marker.End.GetIndex()));
		}

		public void WriteYaml(IEmitter emitter, object? value, Type type)
		{
			throw new NotSupportedException();
		}

		private class MarkParser : IParser
		{
			private readonly IParser _parser;

			public MarkParser(IParser parser)
			{
				_parser = parser;

				Current = parser.Current;
				Start = Current?.Start ?? Mark.Empty;
				End = Start;
			}

			public Mark Start { get; }

			public Mark End { get; private set; }

			public ParsingEvent? Current { get; private set; }

			public bool MoveNext()
			{
				End = Current?.End ?? Mark.Empty;

				bool ret = _parser.MoveNext();
				Current = _parser.Current;

				return ret;
			}
		}
	}
}
