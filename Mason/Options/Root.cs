#pragma warning disable 8618
using CommandLine;

namespace Mason.Standalone
{
	internal abstract class Options
	{
		[Option('c', "config",
			Default = "config.yaml",
			HelpText = "The path to the config file")]
		public string Config { get; set; }

		[Value(0,
			MetaName = "DIR",
			Default = ".",
			HelpText = "The directory to operate on",
			Required = false)]
		public string Directory { get; set; }
	}
}
