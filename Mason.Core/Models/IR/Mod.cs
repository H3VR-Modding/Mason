namespace Mason.Core.IR
{
	internal class Mod : IOptimizable<Mod>
	{
		public Mod(Metadata metadata)
		{
			Metadata = metadata;
		}

		public Metadata Metadata { get; }
		public Assets? Assets { get; set; }

		public Mod Optimize()
		{
			return new(Metadata.Optimize())
			{
				Assets = Assets?.Optimize()
			};
		}
	}
}
