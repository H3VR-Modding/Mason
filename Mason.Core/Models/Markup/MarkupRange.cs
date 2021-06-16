namespace Mason.Core.Markup
{
	public readonly struct MarkupRange
	{
		public MarkupIndex Start { get; }
		public MarkupIndex End { get; }

		public MarkupRange(MarkupIndex start, MarkupIndex end)
		{
			Start = start;
			End = end;
		}
	}
}
