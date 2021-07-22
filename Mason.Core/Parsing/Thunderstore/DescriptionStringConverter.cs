using System;
using Mason.Core.Thunderstore;

namespace Mason.Core.Parsing.Thunderstore
{
	internal class DescriptionStringConverter : ConstrainedStringConverter<DescriptionString>
	{
		protected override DescriptionString Parse(string scalar, Func<string, Exception> exception)
		{
			if (DescriptionString.TryParse(scalar) is not { } constrained)
				throw exception("Descriptions may only be 250 characters long");

			return constrained;
		}
	}
}
