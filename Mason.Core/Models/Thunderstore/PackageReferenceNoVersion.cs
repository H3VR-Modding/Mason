using System;

namespace Mason.Core.Thunderstore
{
	public class PackageReferenceNoVersion
	{
		public static PackageReferenceNoVersion Parse(string value)
		{
			string[] split = value.Split('-');
			if (split.Length != 2)
				throw new FormatException("A package reference must be the author, name, and version, delimited by a hyphen (-)");

			if (PackageComponentString.TryParse(split[0]) is not { } author)
				throw new FormatException(
					"Authors may only have the characters a-z A-Z 0-9 _ and may not start or end with an underscore (_)");
			if (PackageComponentString.TryParse(split[1]) is not { } name)
				throw new FormatException("Names may only have the characters a-z A-Z 0-9 _ and may not start or end with underscore (_)");

			return new PackageReferenceNoVersion(author, name);
		}

		public PackageReferenceNoVersion(PackageComponentString author, PackageComponentString name)
		{
			Author = author;
			Name = name;
		}

		public PackageComponentString Author { get; }
		public PackageComponentString Name { get; }

		public override string ToString()
		{
			return Author + "-" + Name;
		}
	}
}
