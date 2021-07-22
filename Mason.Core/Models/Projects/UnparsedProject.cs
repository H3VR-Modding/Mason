using Mason.Core.Thunderstore;
using YamlDotNet.Core;

namespace Mason.Core.Projects
{
	internal readonly struct UnparsedProject
	{
		public Manifest Manifest { get; }
		public IParser Parser { get; }
		public string Directory { get; }
		public string Path { get; }
		public string ManifestPath { get; }

		public UnparsedProject(Manifest manifest, IParser parser, string directory, string path, string manifestPath)
		{
			Manifest = manifest;
			Parser = parser;
			Path = path;
			Directory = directory;
			ManifestPath = manifestPath;
		}

		public UnparsedProject WithParser(IParser parser)
		{
			return new(Manifest, parser, Directory, Path, ManifestPath);
		}
	}
}
