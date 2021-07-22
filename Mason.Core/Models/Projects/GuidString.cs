using System;
using System.Text.RegularExpressions;
using Mason.Core.Thunderstore;

namespace Mason.Core.Projects
{
	internal class GuidString : ConstrainedString<GuidString>
	{
		private static readonly Regex Filter = new(@"^[a-zA-Z0-9\._\-]+$");

		public static GuidString? TryParse(string value)
		{
			return !Filter.IsMatch(value) ? null : new GuidString(value);
		}

		public static GuidString Parse(string value)
		{
			return TryParse(value) ?? throw new ArgumentException("Value is not a valid GUID", nameof(value));
		}

		private GuidString(string value) : base(value) { }
	}
}
