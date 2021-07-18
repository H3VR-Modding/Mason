using System.IO;
using System.Text;

namespace Mason.Core
{
	public static class Tools
	{
		public static string? RelativePath(string from, string to)
		{
			StringBuilder builder = new(Path.GetFileName(to));
			string? parent = Path.GetDirectoryName(to);

			while (parent != null)
			{
				if (parent == from)
					return builder.ToString();

				string name = Path.GetFileName(parent);

				builder
					.Insert(0, '/')
					.Insert(0, name);

				parent = Path.GetDirectoryName(parent);
			}

			return null;
		}
	}
}
