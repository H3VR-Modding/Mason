using System;

namespace Mason.Core.Thunderstore
{
	internal readonly struct PackageReferenceNoVersion : IEquatable<PackageReferenceNoVersion>
	{
		public string Author { get; }
		public string Name { get; }

		public PackageReferenceNoVersion(string author, string name)
		{
			Author = author;
			Name = name;
		}

		public bool Equals(PackageReferenceNoVersion other)
		{
			return Author == other.Author && Name == other.Name;
		}

		public override bool Equals(object? obj)
		{
			return obj is PackageReferenceNoVersion other && Equals(other);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (Author.GetHashCode() * 397) ^ Name.GetHashCode();
			}
		}

		public override string ToString()
		{
			return Author + "-" + Name;
		}

		public static implicit operator PackageReferenceNoVersion(PackageReference @this)
		{
			return new(@this.Author, @this.Name);
		}
	}
}
