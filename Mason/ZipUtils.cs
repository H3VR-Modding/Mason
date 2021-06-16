using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Mason.Standalone
{
	internal static class ZipUtils
	{
		private static void Shrink(this StringBuilder @this, int length) => @this.Remove(@this.Length - length, length);

		public static void AddFile(this ZipArchive @this, string file, string name, CompressionLevel level)
		{
			var entry = @this.CreateEntry(name, level);
			entry.LastWriteTime = File.GetLastWriteTime(file);

			using var src = File.OpenRead(file);
			using var dest = entry.Open();

			src.CopyTo(dest);
		}

	    public static void AddDirectory(this ZipArchive @this, string directory, CompressionLevel level, StringBuilder builder, HashSet<string> entries)
	    {
		    foreach (var file in Directory.EnumerateFiles(directory))
		    {
			    var name = Path.GetFileName(file);
			    builder.Append(name);

			    var fullName = builder.ToString();
			    if (!entries.Contains(fullName))
			    {
				    entries.Add(fullName);
				    @this.AddFile(file, fullName, level);
			    }

			    builder.Shrink(name.Length);
		    }

		    foreach (var subdirectory in Directory.EnumerateDirectories(directory))
		    {
			    var name = Path.GetFileName(subdirectory);
			    builder.Append(name).Append('/');

			    var fullName = builder.ToString();
			    if (!entries.Contains(fullName))
			    {
				    entries.Add(fullName);
				    @this.CreateEntry(fullName);
				    @this.AddDirectory(subdirectory, level, builder, entries);
			    }

			    builder.Shrink(name.Length + 1);
		    }
	    }
	}
}
