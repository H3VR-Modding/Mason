using System.Collections.Generic;
using Newtonsoft.Json;

namespace Mason.Core.Thunderstore
{
	[JsonObject(ItemRequired = Required.Always)]
	public class Manifest
	{
		[JsonConstructor]
		public Manifest(Marked<PackageComponentString>? author, Marked<PackageComponentString> name, SimpleSemVersion versionNumber,
			List<PackageReference> dependencies, DescriptionString description, string websiteUrl)
		{
			Author = author;
			Name = name;
			VersionNumber = versionNumber;
			Dependencies = dependencies;
			Description = description;
			WebsiteUrl = websiteUrl;
		}

		[JsonProperty(Required = Required.Default)]
		public Marked<PackageComponentString>? Author { get; set; }

		public Marked<PackageComponentString> Name { get; set; }
		public SimpleSemVersion VersionNumber { get; set; }
		public List<PackageReference> Dependencies { get; }
		public DescriptionString Description { get; set; }
		public string WebsiteUrl { get; set; }
	}
}
