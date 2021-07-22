using System;

namespace Mason.Core.Thunderstore
{
	public class PackageReference : IEquatable<PackageReference>
	{
		public PackageReference(PackageComponentString author, PackageComponentString name, SimpleSemVersion version)
		{
			Author = author;
			Name = name;
			Version = version;
		}

		public PackageComponentString Author { get; }
		public PackageComponentString Name { get; }
		public SimpleSemVersion Version { get; }

		public bool Equals(PackageReference other)
		{
			return Author == other.Author && Name == other.Name && Version == other.Version;
		}

		public override bool Equals(object? obj)
		{
			return obj is PackageReference other && Equals(other);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode = Author.GetHashCode();
				hashCode = (hashCode * 397) ^ Name.GetHashCode();
				hashCode = (hashCode * 397) ^ Version.GetHashCode();
				return hashCode;
			}
		}

		public override string ToString()
		{
			return Author + "-" + Name + "-" + Version;
		}
	}
}
