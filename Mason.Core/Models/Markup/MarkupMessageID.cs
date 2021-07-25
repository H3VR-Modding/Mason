using System;

namespace Mason.Core.Markup
{
	public readonly struct MarkupMessageID : IEquatable<MarkupMessageID>
	{
		public MarkupMessageID(char scope, ushort number)
		{
			Scope = scope;
			Number = number;
		}

		public char Scope { get; }

		public ushort Number { get; }

		public override string ToString()
		{
			return Scope.ToString() + Number;
		}

		public bool Equals(MarkupMessageID other)
		{
			return Scope == other.Scope && Number == other.Number;
		}

		public override bool Equals(object? obj)
		{
			return obj is MarkupMessageID other && Equals(other);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (Scope.GetHashCode() * 397) ^ Number.GetHashCode();
			}
		}
	}
}
