#pragma warning disable 8618
using CommandLine;

namespace Mason.Standalone
{
	[Verb("build", HelpText = "Builds the mod, which outputs the plugin file.")]
	internal class BuildOptions : Options
	{
		[Option('o', "output",
			Default = "bootstrap.dll",
			HelpText = "The path to the output file.")]
		public string Output { get; set; }
	}
}
