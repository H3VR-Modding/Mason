using System;

namespace Mason.Core.Thunderstore
{
	public class PackageReference : PackageReferenceNoVersion
	{
		public new static PackageReference Parse(string value)
		{
			string[] split = value.Split('-');
			if (split.Length != 3)
				throw new FormatException("A package reference must be the author, name, and version, delimited by a hyphen (-)");

			if (PackageComponentString.TryParse(split[0]) is not { } author)
				throw new FormatException("Authors may only have the characters a-z A-Z 0-9 _ and may not start or end with an underscore (_)");
			if (PackageComponentString.TryParse(split[1]) is not { } name)
				throw new FormatException("Names may only have the characters a-z A-Z 0-9 _ and may not start or end with underscore (_)");
			if (SimpleSemVersion.TryParse(split[2]) is not { } version)
				throw new FormatException("Versions must be 3 positive integers, delimited by period (.)");

			return new PackageReference(author, name, version);
		}

		public PackageReference(PackageComponentString author, PackageComponentString name, SimpleSemVersion version) : base(author, name)
		{
			Version = version;
		}

		public SimpleSemVersion Version { get; }

		public override string ToString()
		{
			return Author + "-" + Name + "-" + Version;
		}
	}
}
