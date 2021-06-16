using System.Collections.Generic;

namespace Mason.Core.IR
{
	internal class Assets : INopOptimizable<Assets>
	{
		public IList<Asset>? Setup { get; set; }
		public AssetPipeline? Runtime { get; set; }

		public Assets? Optimize()
		{
			var setup = Setup?.Optimize();
			var runtime = Runtime?.Optimize();

			if (setup is null && runtime is null)
				return null;

			return new()
			{
				Setup = setup,
				Runtime = runtime
			};
		}
	}
}
