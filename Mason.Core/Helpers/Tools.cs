using System.IO;
using System.Text;

namespace Mason.Core
{
	public static class Tools
	{
		public static string? RelativePath(string @from, string to)
		{
			var builder = new StringBuilder(Path.GetFileName(to));
			var parent = Path.GetDirectoryName(to);

			while (parent is not null)
			{
				if (parent == @from)
					return builder.ToString();

				var name = Path.GetFileName(parent);

				builder
					.Insert(0, '/')
					.Insert(0, name);

				parent = Path.GetDirectoryName(parent);
			}

			return null;
		}
	}
}
