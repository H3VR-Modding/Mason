using System;
using Mason.Core.Thunderstore;

namespace Mason.Core.Parsing.Thunderstore
{
	internal class PackageComponentStringConverter : ConstrainedStringConverter<PackageComponentString>
	{
		protected override PackageComponentString Parse(string scalar, Func<string, Exception> exception)
		{
			if (PackageComponentString.TryParse(scalar) is not { } constrained)
				throw exception("Package components may only have the characters a-z A-Z 0-9 _ and may not start or end with _");

			return constrained;
		}
	}
}
