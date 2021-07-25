using System;
using System.Collections.Generic;

namespace Mason.Core.Projects.v1
{
	internal class Dependencies
	{
		// Even though we have SimpleSemVersion, plugins use System.Version
		// Hopefully, all plugins use semver, but we cannot assume that
		public Dictionary<GuidString, Marked<Version>>? Hard { get; set; }
		public HashSet<Marked<GuidString>>? Soft { get; set; }
	}
}
