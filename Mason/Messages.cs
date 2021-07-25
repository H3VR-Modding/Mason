using Mason.Core.Markup;

namespace Mason.Standalone
{
	internal static class Messages
	{
		static Messages()
		{
			UnformattedMarkupMessageFactory factory = new('S');

			UnhandledException = factory.Create("An unhandled exception occured: {0}");
			ProjectDirectoryNotFound = factory.Create("The project directory does not exist");
			ConfigFileNotFound = factory.Create("The configuration file does not exist");
			BepInExDirectoryNotFound = factory.Create("The BepInEx directory does not exist");
			ManagedDirectoryNotFound = factory.Create("Could not find the Markup directory");
			ConfigFailedDeserialization = factory.Create("{0}");
			ThunderstoreFileNotFound = factory.Create("The missing file is required for a Thunderstore package");
			MissingProjectFile = factory.Create("{0}");
		}

		public static UnformattedMarkupMessage UnhandledException { get; }
		public static UnformattedMarkupMessage ProjectDirectoryNotFound { get; }
		public static UnformattedMarkupMessage ConfigFileNotFound { get; }
		public static UnformattedMarkupMessage BepInExDirectoryNotFound { get; }
		public static UnformattedMarkupMessage ManagedDirectoryNotFound { get; }
		public static UnformattedMarkupMessage ConfigFailedDeserialization { get; }
		public static UnformattedMarkupMessage ThunderstoreFileNotFound { get; }
		public static UnformattedMarkupMessage MissingProjectFile { get; }
	}
}
