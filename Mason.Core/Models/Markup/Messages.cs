namespace Mason.Core.Markup
{
	internal static class Messages
	{
		static Messages()
		{
			UnformattedMarkupMessageFactory factory = new('C');

			ManifestFailedDeserialization = factory.Create("{0}");
			ManifestNull = factory.Create("Thunderstore manifest files cannot have a null body.");
			ProjectVersionMissing = factory.Create("Project files must begin with the '{0}' property");
			ProjectVersionNonNumeric = factory.Create("The '{0}' property must have a numeric value.");
			ProjectVersionUnsupported = factory.Create("Version {0} is not supported by this version of Mason.");
			ProjectMultipleDocuments = factory.Create("Project files may contain only one document");
			ProjectFailedDeserialization = factory.Create("{0}");
			UnknownAuthor = factory.Create("The author property must be present, or the directory must be named [author]-[name].");
			DiscrepantAuthor =
				factory.Create(
					"The author property must be present, or the directory must be named [author]-[name]. Perhaps you meant to name the directory '{0}-{1}'?");
			InferredAuthorInvalid =
				factory.Create(
					"Author (inferred by directory) may only contain the characters a-z A-Z 0-9 _ and cannot start or end with _");
			InferredAuthorSuccessful =
				factory.Create("The author of the mod was infered by the directory name. Consider adding an 'author' property.");
			StratumDependencySuperior =
				factory.Create("Stratum dependency with a minimum version ({0}) greater than required ({1})");
			StratumDependencyRedundant = factory.Create("Redundant Stratum dependency");
			StratumDependencyInferior =
				factory.Create("Stratum dependency with a minimum version ({0}) less than required ({1})");
			SoftDependencyIsHardDependency = factory.Create("Soft dependency is already a hard dependency");
			UnmatchedGlob = factory.Create("Glob matched no handles");
			ResourceDirectoryMissing = factory.Create("No resources found");
		}

		public static UnformattedMarkupMessage ManifestFailedDeserialization { get; }
		public static UnformattedMarkupMessage ManifestNull { get; }

		public static UnformattedMarkupMessage ProjectVersionMissing { get; }
		public static UnformattedMarkupMessage ProjectVersionNonNumeric { get; }
		public static UnformattedMarkupMessage ProjectVersionUnsupported { get; }
		public static UnformattedMarkupMessage ProjectMultipleDocuments { get; }
		public static UnformattedMarkupMessage ProjectFailedDeserialization { get; }
		public static UnformattedMarkupMessage UnknownAuthor { get; }
		public static UnformattedMarkupMessage DiscrepantAuthor { get; }
		public static UnformattedMarkupMessage InferredAuthorInvalid { get; }
		public static UnformattedMarkupMessage InferredAuthorSuccessful { get; }
		public static UnformattedMarkupMessage StratumDependencySuperior { get; }
		public static UnformattedMarkupMessage StratumDependencyRedundant { get; }
		public static UnformattedMarkupMessage StratumDependencyInferior { get; }
		public static UnformattedMarkupMessage SoftDependencyIsHardDependency { get; }
		public static UnformattedMarkupMessage UnmatchedGlob { get; }
		public static UnformattedMarkupMessage ResourceDirectoryMissing { get; }
	}
}
