using System.IO;
using Mason.Core.Thunderstore;

namespace Mason.Core
{
	public class CompilerParameters : IHasBinaryPaths
	{
		private static readonly PackageReferenceNoVersion DefaultStratumPackage;

		static CompilerParameters()
		{
			PackageComponentString stratum = PackageComponentString.Parse("Stratum");

			DefaultStratumPackage = new PackageReferenceNoVersion(stratum, stratum);
		}

		public CompilerParameters(string managedDirectory, string bepInExDirectory)
		{
			ManagedDirectory = managedDirectory;
			BepInExDirectory = bepInExDirectory;
		}

		public string ManagedDirectory { get; }

		public string BepInExDirectory { get; }

		public PackageReferenceNoVersion StratumPackage { get; set; } = DefaultStratumPackage;

		public string StratumGUID { get; set; } = "stratum";

		string IHasBinaryPaths.Mscorlib => Path.Combine(ManagedDirectory, "mscorlib.dll");
		string IHasBinaryPaths.SystemCore => Path.Combine(ManagedDirectory, "System.Core.dll");
		string IHasBinaryPaths.UnityEngine => Path.Combine(ManagedDirectory, "UnityEngine.dll");
		string IHasBinaryPaths.BepInEx => Path.Combine(Path.Combine(BepInExDirectory, "core"), "BepInEx.dll");

		string IHasBinaryPaths.Stratum =>
			Path.Combine(Path.Combine(Path.Combine(BepInExDirectory, "plugins"), StratumPackage.ToString()), "Stratum.dll");
	}
}
