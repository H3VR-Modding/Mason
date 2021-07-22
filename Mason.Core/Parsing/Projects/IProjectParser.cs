using Mason.Core.Projects;

namespace Mason.Core.Parsing.Projects
{
	internal interface IProjectParser
	{
		ParserOutput Parse(UnparsedProject project);
	}
}
