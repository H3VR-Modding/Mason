namespace Mason.Core.Projects.v1
{
	internal class AssetPipeline
	{
		public bool Sequential { get; set; }
		public string? Name { get; set; }
		public AssetPipeline[]? Nested { get; set; }
		public Asset[]? Assets { get; set; }
	}
}
