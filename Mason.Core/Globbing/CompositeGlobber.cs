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
			using IEnumerator<Globber> enumerator = _globs.GetEnumerator();

			IEnumerable<string> directories = new[]
			{
				directory
			};

			Globber? glob = null;
			if (enumerator.MoveNext())
				while (true)
				{
					glob = enumerator.Current!;
					if (!enumerator.MoveNext())
						break;

					Globber globC = glob;
					directories = directories.SelectMany(d => globC(d)).Where(Directory.Exists);
				}

			return glob is null ? directories : directories.SelectMany(d => glob(d));
		}
	}
}
