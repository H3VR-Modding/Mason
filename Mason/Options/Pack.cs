#pragma warning disable 8618
using System.Diagnostics.CodeAnalysis;
using System.IO.Compression;
using CommandLine;

namespace Mason.Standalone
{
	[Verb("pack", HelpText = "Builds and zips the mod, which outputs the mod ready for upload to Thunderstore.")]
	internal class PackOptions : Options
	{
		// We WANT goofy naming
		[SuppressMessage("ReSharper", "InconsistentNaming")]
		public enum FriendlyCompressionLevel
		{
			best = 0,
			fast = 1,
			none = 2
		}

		[Option('o', "output",
			Default = "mod.zip",
			HelpText = "The path to the output file.")]
		public string Output { get; set; }

		[Option('z', "compression", Default = FriendlyCompressionLevel.best,
			HelpText = "The amount of compression that should be performed. Options are: best, fast, none")]
		public FriendlyCompressionLevel FriendlyCompression { get; set; }

		public CompressionLevel Compression => (CompressionLevel) FriendlyCompression;
	}
}
