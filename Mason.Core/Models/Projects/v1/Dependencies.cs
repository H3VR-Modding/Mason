using System;
using System.Collections.Generic;

namespace Mason.Core.Projects.v1
{
	internal class Dependencies
	{
		public Dictionary<string, Marked<Version>>? Hard { get; set; }
		public Marked<string>[]? Soft { get; set; }
	}
}
