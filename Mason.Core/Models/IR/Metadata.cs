using System.Collections.Generic;
using BepInEx;
using Mason.Core.Thunderstore;

namespace Mason.Core.IR
{
	internal class Metadata : IOptimizable<Metadata>
	{
		public Metadata(BepInPlugin plugin, PackageReference package)
		{
			Plugin = plugin;
			Package = package;
		}

		public BepInPlugin Plugin { get; }
		public PackageReference Package { get; }

		public IList<BepInProcess>? Processes { get; set; }
		public IList<Marked<BepInDependency>>? Dependencies { get; set; }
		public IList<BepInIncompatibility>? Incompatibilities { get; set; }

		public Metadata Optimize()
		{
			return new(Plugin, Package)
			{
				Processes = Processes?.Optimize(),
				Dependencies = Dependencies?.Optimize(),
				Incompatibilities = Incompatibilities?.Optimize()
			};
		}
	}
}
