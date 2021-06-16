using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Mason.Core.Markup;
using Mason.Core.Thunderstore;

namespace Mason.Core
{
	internal class CompilerOutput : ICompilerOutput
	{
		private bool? _successful;
		private MarkupMessage? _error;
		private PackageReference? _package;

		public List<MarkupMessage> Warnings { get; } = new();
		IList<MarkupMessage> ICompilerOutput.Warnings => Warnings;
		public HashSet<string> ReferencedPaths { get; } = new();
		IEnumerable<string> ICompilerOutput.ReferencedPaths => ReferencedPaths;

		public void Success(PackageReference package)
		{
			_package = package;
			_successful = true;
		}

		public void Failure(MarkupMessage error)
		{
			_error = error;
			_successful = false;
		}

		public bool MatchSuccess([NotNullWhen(false)] out MarkupMessage? error, [NotNullWhen(true)] out PackageReference? package)
		{
			error = _error;
			package = _package;
			return _successful ?? throw new InvalidOperationException("Success indicator was not set. This is indicative that something has gone horribly wrong.");
		}
	}
}
