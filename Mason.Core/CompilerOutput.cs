using System;
using System.Collections.Generic;
using System.IO;
using Mason.Core.Markup;
using Mason.Core.Parsing.Projects;
using Mason.Core.Thunderstore;

namespace Mason.Core
{
	public class CompilerOutput : IDisposable
	{
		internal CompilerOutput(MemoryStream bootstrap, Manifest manifest, ParserOutput parserOutput)
		{
			Bootstrap = bootstrap;
			Manifest = manifest;
			Warnings = parserOutput.Warnings;
			ReferencedPaths = parserOutput.ReferencedPaths;
			Package = parserOutput.Mod.Metadata.Package;
			IgnoredMessages = parserOutput.IgnoredMessages;
		}

		public MemoryStream Bootstrap { get; }
		public IList<MarkupMessage> Warnings { get; }
		public IEnumerable<string> ReferencedPaths { get; }
		public ICollection<MarkupMessageID>? IgnoredMessages { get; }
		public Manifest Manifest { get; }
		public PackageReference Package { get; }

		public void Dispose()
		{
			Bootstrap.Dispose();
		}
	}
}
