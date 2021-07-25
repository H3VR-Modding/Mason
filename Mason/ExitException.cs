using System;
using Mason.Core.Markup;

namespace Mason.Standalone
{
	internal class ExitException : Exception
	{
		public ExitException(ExitCode code, MarkupMessage? markup = null)
		{
			Code = code;
			Markup = markup;
		}

		public ExitCode Code { get; }
		public MarkupMessage? Markup { get; }
	}
}
