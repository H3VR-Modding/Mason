using Mason.Core.Projects;

namespace Mason.Core.Parsing.Projects
{
	internal class GuidStringTypeConverter : ConstrainedStringTypeConverter<GuidString>
	{
		protected override GuidString Parse(string scalar)
		{
			return GuidString.Parse(scalar);
		}
	}
}
