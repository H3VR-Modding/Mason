using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Mason.Core.Markup;
using Mason.Core.Thunderstore;

namespace Mason.Core
{
	public interface ICompilerOutput
	{
		IList<MarkupMessage> Warnings { get; }
		IEnumerable<string> ReferencedPaths { get; }

		bool MatchSuccess([MaybeNullWhen(true)] out MarkupMessage error, [NotNullWhen(true)] out PackageReference? package);
	}
}
