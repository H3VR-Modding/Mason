namespace Mason.Core.Markup
{
	public class UnformattedMarkupMessage
	{
		public UnformattedMarkupMessage(string content, MarkupMessageID id)
		{
			Content = content;
			ID = id;
		}

		public string Content { get; }

		public MarkupMessageID ID { get; }

		public override string ToString()
		{
			return "[" + ID + "] " + Content;
		}
	}
}
