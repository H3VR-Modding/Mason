using Mason.Core.Markup;

namespace Mason.Core
{
	public readonly struct Marked<T>
	{
		public MarkupRange Range { get; }
		public T Value { get; }

		public Marked(T value, MarkupRange range)
		{
			Value = value;
			Range = range;
		}
	}
}
