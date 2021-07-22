using System;
using System.IO;
using Mason.Core.Markup;
using Mason.Core.Thunderstore;

namespace Mason.Core
{
	public class CompilerException : Exception
	{
		internal CompilerException(MarkupMessage markup, PackageReference? package = null) : base(markup.ToString(Path.GetFullPath))
		{
			Markup = markup;
			Package = package;
		}

		public MarkupMessage Markup { get; }
		public PackageReference? Package { get; }
	}
}
