using System;

namespace Mason.Core.Thunderstore
{
	public abstract class ConstrainedString<T> : IEquatable<T> where T : ConstrainedString<T>
	{
		protected ConstrainedString(string value)
		{
			Value = value;
		}

		public string Value { get; }

		private bool Equals(ConstrainedString<T> other)
		{
			return Value == other.Value;
		}

		public bool Equals(T other)
		{
			return Equals((ConstrainedString<T>) other);
		}

		public override bool Equals(object? obj)
		{
			return ReferenceEquals(this, obj) || (obj is ConstrainedString<T> other && Equals(other));
		}

		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}

		public override string ToString()
		{
			return Value;
		}

		public static implicit operator string(ConstrainedString<T> @this)
		{
			return @this.Value;
		}

		public static bool operator ==(ConstrainedString<T> lhs, ConstrainedString<T> rhs)
		{
			return lhs.Value == rhs.Value;
		}

		public static bool operator !=(ConstrainedString<T> lhs, ConstrainedString<T> rhs)
		{
			return !(lhs == rhs);
		}
	}
}
