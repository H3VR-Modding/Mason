using System;
using System.Text.RegularExpressions;

namespace Mason.Core.Thunderstore
{
	public sealed class PackageComponentString : ConstrainedString<PackageComponentString>
	{
		private static readonly Regex Filter = new("^[a-zA-Z0-9](?:[a-zA-Z0-9_]*[a-zA-Z0-9])?$");

		public static PackageComponentString? TryParse(string value)
		{
			return !Filter.IsMatch(value) ? null : new PackageComponentString(value);
		}

		public static PackageComponentString Parse(string value)
		{
			return TryParse(value) ?? throw new ArgumentException("Not a valid package component", nameof(value));
		}

		private PackageComponentString(string value) : base(value) { }
	}
}
