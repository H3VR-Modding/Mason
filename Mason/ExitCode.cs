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
		ProjectDirectoryNotFound,
		ConfigFileNotFound,
		InvalidConfig
	}
}
