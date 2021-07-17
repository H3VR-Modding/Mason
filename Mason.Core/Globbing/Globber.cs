using System.Collections.Generic;

namespace Mason.Core.Globbing
{
	/// <summary>
	///     A method which enumerates over handles with specific qualities
	/// </summary>
	/// <param name="directory">The directory whose children should be enumerated</param>
	internal delegate IEnumerable<string> Globber(string directory);
}
