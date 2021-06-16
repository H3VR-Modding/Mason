namespace Mason.Core.IR
{
	internal class Mod : IOptimizable<Mod>
	{
		public Metadata Metadata { get; }
		public Assets? Assets { get; set; }

		public Mod(Metadata metadata) => Metadata = metadata;

		public Mod Optimize() => new(Metadata.Optimize())
		{
			Assets = Assets?.Optimize()
		};
	}
}
