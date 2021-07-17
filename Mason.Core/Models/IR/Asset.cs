namespace Mason.Core.IR
{
	internal class Asset
	{
		public Asset(string path, string plugin, string loader)
		{
			Path = path;
			Plugin = plugin;
			Loader = loader;
		}

		public string Path { get; }
		public string Plugin { get; }
		public string Loader { get; }
	}
}
