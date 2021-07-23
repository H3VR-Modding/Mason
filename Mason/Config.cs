#pragma warning disable 8618
using System.IO;
using Mason.Core.Thunderstore;

namespace Mason.Standalone
{
	internal class Config
	{
		private static string ResolvePath(string directory, string path)
		{
			return Path.IsPathRooted(path) ? path : Path.Combine(directory, path);
		}

		public DirectoriesNode Directories { get; set; }

		public PackageReferenceNoVersion? StratumPackage { get; set; }

		public void ResolvePaths(string directory)
		{
			Directories.ResolvePaths(directory);
		}

		public class DirectoriesNode
		{
			public string Bepinex { get; set; }
			public string Managed { get; set; }

			public void ResolvePaths(string directory)
			{
				Bepinex = ResolvePath(directory, Bepinex);
				Managed = ResolvePath(directory, Managed);
			}
		}
	}
}
