using System.Collections.Generic;
using System.IO;

namespace Mason.Core.Globbing
{
	internal static class StatelessGlobbers
	{
		public static IEnumerable<string> Globstar(string directory)
		{
			yield return directory;

			foreach (var item in Directory.GetFiles(directory))
				yield return item;

			foreach (var item in Directory.GetDirectories(directory))
			foreach (var subitem in Globstar(item))
				yield return subitem;
		}

		public static IEnumerable<string> Current(string directory)
		{
			yield return directory;
		}
	}
}
