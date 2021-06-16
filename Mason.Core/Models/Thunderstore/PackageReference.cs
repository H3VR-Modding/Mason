using System;

namespace Mason.Core.Thunderstore
{
	public readonly struct PackageReference : IEquatable<PackageReference>
	{
		public string Author { get; }
		public string Name { get; }
		public Version Version { get; }

		public PackageReference(string author, string name, Version version)
		{
			Author = author;
			Name = name;
			Version = version;
		}

		public bool Equals(PackageReference other) => Author == other.Author && Name == other.Name && Version.Equals(other.Version);

		public override bool Equals(object? obj) => obj is PackageReference other && Equals(other);

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = Author.GetHashCode();
				hashCode = (hashCode * 397) ^ Name.GetHashCode();
				hashCode = (hashCode * 397) ^ Version.GetHashCode();
				return hashCode;
			}
		}

		public override string ToString() => Author + "-" + Name + "-" + Version.ToString(3);
	}
}
