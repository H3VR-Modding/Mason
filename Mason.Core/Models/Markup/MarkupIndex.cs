namespace Mason.Core.Markup
{
	public readonly struct MarkupIndex
	{
		public int Line { get; }
		public int Column { get; }

		public MarkupIndex(int line, int column)
		{
			Line = line;
			Column = column;
		}
	}
}
