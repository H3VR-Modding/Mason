using System.Collections.Generic;

#pragma warning disable 8618
namespace Mason.Core.Projects.v1
{
	internal class Project
	{
		public Dependencies? Dependencies { get; set; }
		public List<GuidString>? Incompatibilities { get; set; }
		public List<string>? Processes { get; set; }
		public Assets? Assets { get; set; }
	}
}
