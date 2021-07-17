using System.Collections.Generic;
using System.IO;

namespace Mason.Core.Globbing
{
	internal static class StatelessGlobbers
	{
		public static IEnumerable<string> Globstar(string directory)
		{
			yield return directory;

			foreach (string item in Directory.GetFiles(directory))
				yield return item;

			foreach (string item in Directory.GetDirectories(directory))
			foreach (string subitem in Globstar(item))
				yield return subitem;
		}

		public static IEnumerable<string> Current(string directory)
		{
			yield return directory;
		}
	}
}
