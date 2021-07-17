using System.Collections.Generic;
using System.Linq;

namespace Mason.Core.IR
{
	internal class AssetPipeline : INopOptimizable<AssetPipeline>
	{
		public bool Sequential { get; set; }
		public string? Name { get; set; }
		public IList<AssetPipeline> Nested { get; set; } = new AssetPipeline[0];
		public IList<Asset> Assets { get; set; } = new Asset[0];

		public AssetPipeline? Optimize()
		{
			if (Assets.Count > 0)
				return this;

			List<AssetPipeline> buffer = Nested
				.Select(x => x.Optimize())
				.WhereNotNull()
				.ToList();

			return buffer.Count switch
			{
				0 => null,
				1 => buffer[0],
				_ => this
			};
		}
	}
}
