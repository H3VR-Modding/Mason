using Mason.Core.IR;
using Mason.Core.Thunderstore;
using YamlDotNet.Core;

namespace Mason.Core.Parsing.Projects
{
	internal interface IProjectParser
	{
		Mod? Parse(Manifest manifest, string manifestFile, IParser project, string projectFile, string directory, CompilerOutput output);
	}
}
