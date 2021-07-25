using System;

namespace Mason.Core.Thunderstore
{
	public sealed class SimpleSemVersion : IEquatable<SimpleSemVersion>, IComparable<SimpleSemVersion>
	{
		public static SimpleSemVersion? TryParse(string value)
		{
			string[] split = value.Split('.');
			if (split.Length != 3)
				return null;

			if (!ushort.TryParse(split[0], out ushort major) || !ushort.TryParse(split[1], out ushort minor) ||
			    !ushort.TryParse(split[2], out ushort patch))
				return null;

			return new SimpleSemVersion(major, minor, patch);
		}

		private readonly ushort _major;
		private readonly ushort _minor;
		private readonly ushort _patch;

		private SimpleSemVersion(ushort major, ushort minor, ushort patch)
		{
			_major = major;
			_minor = minor;
			_patch = patch;
		}

		public override bool Equals(object? obj)
		{
			return ReferenceEquals(this, obj) || (obj is SimpleSemVersion other && Equals(other));
		}

		public bool Equals(SimpleSemVersion other)
		{
			return _major == other._major && _minor == other._minor && _patch == other._patch;
		}

		public int CompareTo(SimpleSemVersion other)
		{
			int major = _major.CompareTo(other._major);
			if (major != 0)
				return major;

			int minor = _minor.CompareTo(other._minor);
			if (minor != 0)
				return minor;

			return _patch.CompareTo(other._patch);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode = _major.GetHashCode();
				hashCode = (hashCode * 397) ^ _minor.GetHashCode();
				hashCode = (hashCode * 397) ^ _patch.GetHashCode();
				return hashCode;
			}
		}

		public override string ToString()
		{
			return _major + "." + _minor + "." + _patch;
		}

		public static bool operator ==(SimpleSemVersion lhs, SimpleSemVersion rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(SimpleSemVersion lhs, SimpleSemVersion rhs)
		{
			return !(lhs == rhs);
		}

		public static bool operator >(SimpleSemVersion lhs, SimpleSemVersion rhs)
		{
			return lhs._major > rhs._major || lhs._minor > rhs._minor || lhs._patch > rhs._patch;
		}

		public static bool operator >=(SimpleSemVersion lhs, SimpleSemVersion rhs)
		{
			return lhs._major >= rhs._major || lhs._minor >= rhs._minor || lhs._patch >= rhs._patch;
		}

		public static bool operator <(SimpleSemVersion lhs, SimpleSemVersion rhs)
		{
			return lhs._major < rhs._major || lhs._minor < rhs._minor || lhs._patch < rhs._patch;
		}

		public static bool operator <=(SimpleSemVersion lhs, SimpleSemVersion rhs)
		{
			return lhs._major <= rhs._major || lhs._minor <= rhs._minor || lhs._patch <= rhs._patch;
		}
	}
}
