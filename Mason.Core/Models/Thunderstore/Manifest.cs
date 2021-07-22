using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Mason.Core.Thunderstore
{
	[JsonObject(ItemRequired = Required.Always)]
	public class Manifest
	{
		[JsonConstructor]
		public Manifest(Marked<string>? author, Marked<string> name, Version versionNumber, List<PackageReference> dependencies,
			string description, string websiteUrl)
		{
			Author = author;
			Name = name;
			VersionNumber = versionNumber;
			Dependencies = dependencies;
			Description = description;
			WebsiteUrl = websiteUrl;
		}

		[JsonProperty(Required = Required.Default)]
		public Marked<string>? Author { get; set; }

		public Marked<string> Name { get; set; }
		public Version VersionNumber { get; set; }
		public List<PackageReference> Dependencies { get; }
		public string Description { get; set; }
		public string WebsiteUrl { get; set; }
	}
}
