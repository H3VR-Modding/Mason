using System.Collections.Generic;
using Mason.Core.Markup;

#pragma warning disable 8618
namespace Mason.Core.Projects.v1
{
	internal class Project
	{
		public HashSet<MarkupMessageID>? Ignore { get; set; }
		public Dependencies? Dependencies { get; set; }
		public HashSet<GuidString>? Incompatibilities { get; set; }
		public HashSet<string>? Processes { get; set; }
		public Assets? Assets { get; set; }
	}
}
