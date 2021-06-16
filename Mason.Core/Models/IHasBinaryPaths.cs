namespace Mason.Core
{
	internal interface IHasBinaryPaths
	{
		string Mscorlib { get; }

		string SystemCore { get; }

		string UnityEngine { get; }

		string BepInEx { get; }

		string Stratum { get; }
	}
}
