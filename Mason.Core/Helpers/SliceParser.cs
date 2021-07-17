using System;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace Mason.Core
{
	internal class SliceParser : IParser
	{
		private readonly IParser _parser;
		private ushort _stack = 1;

		private byte _state;

		public SliceParser(IParser parser)
		{
			_parser = parser;
		}

		public ParsingEvent? Current { get; private set; }

		public bool MoveNext()
		{
			switch (_state)
			{
				case 0:
					Current = new MappingStart();
					++_state;
					return true;
				case 1:
					Current = _parser.Current;

					switch (Current)
					{
						case MappingEnd when --_stack == 0:
							++_state;
							return true;
						case MappingStart:
							++_stack;
							break;
					}

					if (!_parser.MoveNext())
						throw new InvalidOperationException("Parser unexpectedly ended.");

					return true;
				case 2:
					Current = new MappingEnd();
					++_state;
					return false;
				default:
					return false;
			}
		}
	}
}
