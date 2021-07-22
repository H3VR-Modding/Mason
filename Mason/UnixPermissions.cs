using System;

namespace Mason.Standalone
{
	[Flags]
	public enum UnixPermissions
	{
		UserRead = 1 << 8,
		UserWrite = 1 << 7,
		UserExecute = 1 << 6,
		GroupRead = 1 << 5,
		GroupWrite = 1 << 4,
		GroupExecute = 1 << 3,
		OtherRead = 1 << 2,
		OtherWrite = 1 << 1,
		OtherExecute = 1 << 0
	}
}
