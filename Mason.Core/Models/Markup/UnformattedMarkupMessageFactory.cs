namespace Mason.Core.Markup
{
	public class UnformattedMarkupMessageFactory
	{
		public UnformattedMarkupMessageFactory(char scope)
		{
			Scope = scope;
		}

		public char Scope { get; }

		public ushort Ticket { get; private set; }

		public UnformattedMarkupMessage Create(string message)
		{
			MarkupMessageID id = new(Scope, Ticket++);

			return new UnformattedMarkupMessage(message, id);
		}
	}
}
