using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Mason.Core.Globbing
{
	internal class CompositeGlobber
	{
		private readonly ICollection<Globber> _globs;

		public CompositeGlobber(ICollection<Globber> globs)
		{
			_globs = globs;
		}

		public IEnumerable<string> Globber(string directory)
		{
			using var enumerator = _globs.GetEnumerator();

			IEnumerable<string> directories = new[] {directory};

			Globber? glob = null;
			if (enumerator.MoveNext())
			{
				while (true)
				{
					glob = enumerator.Current!;
					var next = enumerator.MoveNext();
					if (!next)
					{
						break;
					}

					var globC = glob;
					directories = directories.SelectMany(d => globC(d)).Where(Directory.Exists);
				}
			}

			return glob is null ? directories : directories.SelectMany(d => glob(d));
		}
	}
}
