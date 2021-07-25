using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using Mason.Core;
using Mason.Core.Thunderstore;
using Mono.Cecil;

namespace Mason.Patcher
{
	public static class Entrypoint
	{
		private static readonly ManualLogSource Logger = BepInEx.Logging.Logger.CreateLogSource("Mason Patcher");

		[SuppressMessage("ReSharper", "UnusedMember.Global")]
		public static IEnumerable<string> TargetDLLs => Enumerable.Empty<string>();

		[SuppressMessage("ReSharper", "UnusedMember.Global")]
		public static void Patch(AssemblyDefinition asm)
		{
			Logger.LogWarning("There aren't be any assemblies to patch, but BepInEx called the patch function. Assembly: " + asm);
		}

		[SuppressMessage("ReSharper", "UnusedMember.Global")]
		public static void Finish()
		{
			try
			{
				Run();
			}
			catch (Exception e)
			{
				Logger.LogFatal("A project-agnostic error has occured. No additional projects will be compiled: " + e);
			}
		}

		private static void Run()
		{
			Logger.LogInfo("Bootstrapping Mason");

			ConfigFile config = new(Path.Combine(Paths.ConfigPath, "mason.cfg"), false);
			PackageReferenceNoVersion stratumPackage;
			try
			{
				string scalar = config.Bind("General", "StratumPackage", "Stratum-Stratum").Value;
				stratumPackage = PackageReferenceNoVersion.Parse(scalar);
			}
			catch (FormatException e)
			{
				Logger.LogFatal("Misformatted Stratum package name: " + e.Message);
				return;
			}

			Logger.LogDebug("Read config");

			CompilerParameters parameters = new(Paths.ManagedPath, Paths.BepInExRootPath)
			{
				StratumPackage = stratumPackage
			};
			Logger.LogDebug("Created parameters");
			Compiler compiler = new(parameters);
			Logger.LogDebug("Constructed compiler");

			Patcher patcher = new(compiler, Logger);
			Logger.LogDebug("Constructed patcher");

			patcher.Run();
			Logger.LogMessage("Compilation complete");
		}
	}
}
