using Mason.Core.Markup;

namespace Mason.Core
{
	internal readonly struct Marked<T>
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
