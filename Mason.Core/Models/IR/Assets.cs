using System.Collections.Generic;

namespace Mason.Core.IR
{
	internal class Assets : INopOptimizable<Assets>
	{
		public IList<Asset>? Setup { get; set; }
		public AssetPipeline? Runtime { get; set; }

		public Assets? Optimize()
		{
			IList<Asset>? setup = Setup?.Optimize();
			AssetPipeline? runtime = Runtime?.Optimize();

			if (setup == null && runtime == null)
				return null;

			return new Assets
			{
				Setup = setup,
				Runtime = runtime
			};
		}
	}
}
