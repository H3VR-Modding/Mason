namespace Mason.Standalone
{
	internal enum ExitCode
	{
		None,
		Internal,
		Compiler,
		ManagedDirectoryNotFound,
		BepInExDirectoryNotFound,
		MissingProjectFiles,
		MissingThunderstoreFiles,
		ProjectDirectoryDoesNotExist,
		MissingConfig
	}
}
