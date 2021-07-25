using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;

namespace Mason.Standalone
{
	internal static class ZipUtils
	{
		public static int ExternalAttributes = (int) (UnixPermissions.UserRead | UnixPermissions.UserWrite | UnixPermissions.GroupRead |
		                                              UnixPermissions.GroupWrite | UnixPermissions.OtherRead) << 16;

		private static void Shrink(this StringBuilder @this, int length)
		{
			@this.Remove(@this.Length - length, length);
		}

		public static async Task AddFile(this ZipArchive @this, string file, string name, CompressionLevel level)
		{
			ZipArchiveEntry entry = @this.CreateEntry(name, level);
			entry.LastWriteTime = File.GetLastWriteTime(file);
			entry.ExternalAttributes = ExternalAttributes;

			await using FileStream src = File.OpenRead(file);
			await using Stream dest = entry.Open();

			await src.CopyToAsync(dest);
		}

		public static async Task AddDirectory(this ZipArchive @this, string directory, CompressionLevel level, StringBuilder builder,
			HashSet<string> entries)
		{
			foreach (string file in Directory.EnumerateFiles(directory))
			{
				string name = Path.GetFileName(file);
				builder.Append(name);

				var fullName = builder.ToString();
				if (!entries.Contains(fullName))
				{
					entries.Add(fullName);
					await @this.AddFile(file, fullName, level);
				}

				builder.Shrink(name.Length);
			}

			foreach (string subdirectory in Directory.EnumerateDirectories(directory))
			{
				string name = Path.GetFileName(subdirectory);
				builder.Append(name).Append('/');

				var fullName = builder.ToString();
				if (!entries.Contains(fullName))
				{
					entries.Add(fullName);
					@this.CreateEntry(fullName);
					await @this.AddDirectory(subdirectory, level, builder, entries);
				}

				builder.Shrink(name.Length + 1);
			}
		}
	}
}
