using System.Collections.Generic;
using Mason.Core.IR;
using Mason.Core.Markup;

namespace Mason.Core.Parsing.Projects
{
	internal readonly struct ParserOutput
	{
		public Mod Mod { get; }
		public IList<MarkupMessage> Warnings { get; }
		public IEnumerable<string> ReferencedPaths { get; }

		public ParserOutput(Mod mod, IList<MarkupMessage> warnings, IEnumerable<string> referencedPaths)
		{
			Mod = mod;
			Warnings = warnings;
			ReferencedPaths = referencedPaths;
		}
	}
}
