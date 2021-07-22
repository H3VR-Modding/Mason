using System;

namespace Mason.Core.Thunderstore
{
	public sealed class DescriptionString : ConstrainedString<DescriptionString>
	{
		public static DescriptionString? TryParse(string value)
		{
			return value.Length > 250 ? null : new DescriptionString(value);
		}

		public static DescriptionString Parse(string value)
		{
			return TryParse(value) ?? throw new ArgumentException("Not a valid description", nameof(value));
		}

		private DescriptionString(string value) : base(value) { }
	}
}
