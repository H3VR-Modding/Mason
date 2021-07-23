namespace Mason.Standalone
{
	internal enum ExitCode
	{
		Ok,
		Internal,
		Compiler,
		ManagedDirectoryNotFound,
		BepInExDirectoryNotFound,
		MissingProjectFiles,
		MissingThunderstoreFiles,
		ProjectDirectoryDoesNotExist,
		MissingConfig,
		InvalidConfig
	}
}
