namespace Mason.Core.Markup
{
	public class MarkupLocation
	{
		public MarkupLocation(string path, MarkupIndex start, MarkupIndex? end = null)
		{
			Path = path;
			Start = start;
			End = end;
		}

		public string Path { get; }

		public MarkupIndex Start { get; }
		public MarkupIndex? End { get; }
	}
}
