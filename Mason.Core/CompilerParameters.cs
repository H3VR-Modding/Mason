using System.IO;

namespace Mason.Core
{
	public class CompilerParameters : IHasBinaryPaths
	{
		public string ManagedDirectory { get; }

		public string BepInExDirectory { get; }

		public string StratumPackage { get; set; } = "StratumTeam-Stratum";

		string IHasBinaryPaths.Mscorlib => Path.Combine(ManagedDirectory, "mscorlib.dll");
		string IHasBinaryPaths.SystemCore => Path.Combine(ManagedDirectory, "System.Core.dll");
		string IHasBinaryPaths.UnityEngine => Path.Combine(ManagedDirectory, "UnityEngine.dll");
		string IHasBinaryPaths.BepInEx => Path.Combine(Path.Combine(BepInExDirectory, "core"), "BepInEx.dll");
		string IHasBinaryPaths.Stratum => Path.Combine(Path.Combine(Path.Combine(BepInExDirectory, "plugins"), StratumPackage), "Stratum.dll");

		public CompilerParameters(string managedDirectory, string bepInExDirectory)
		{
			ManagedDirectory = managedDirectory;
			BepInExDirectory = bepInExDirectory;
		}
	}
}
