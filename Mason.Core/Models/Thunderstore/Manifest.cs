using System;
using Newtonsoft.Json;

namespace Mason.Core.Thunderstore
{
	[JsonObject(ItemRequired = Required.Always)]
	internal class Manifest
	{
		[JsonProperty(Required = Required.Default)]
		public string? Author { get; }
		public Marked<string> Name { get; }
		public Version VersionNumber { get; }
		public PackageReference[] Dependencies { get; }
		public string Description { get; }
		public string WebsiteUrl { get; }

		[JsonConstructor]
		public Manifest(string? author, Marked<string> name, Version versionNumber, PackageReference[] dependencies, string description,
			string websiteUrl)
		{
			Author = author;
			Name = name;
			VersionNumber = versionNumber;
			Dependencies = dependencies;
			Description = description;
			WebsiteUrl = websiteUrl;
		}
	}
}
