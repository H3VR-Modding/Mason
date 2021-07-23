using System.Linq;
using Mono.Cecil;
using MonoMod.Utils;

namespace Mason.Core.RefsAndDefs
{
	internal class BepInExRefs
	{
		public BepInExRefs(ModuleDefinition bepInEx)
		{
			BepInPluginCtor = bepInEx.GetTypeSafe("BepInEx.BepInPlugin")
				.FindMethodSafe("System.Void .ctor(System.String,System.String,System.String)");

			BepInProcessCtor = bepInEx.GetTypeSafe("BepInEx.BepInProcess").FindMethodSafe("System.Void .ctor(System.String)");

			TypeDefinition bepindep = bepInEx.GetTypeSafe("BepInEx.BepInDependency");
			BepInDependencyCtorWithVersion = bepindep.FindMethodSafe("System.Void .ctor(System.String,System.String)");
			BepInDependencyCtorWithFlags = bepindep.FindMethodSafe("System.Void .ctor(System.String,BepInEx.BepInDependency/DependencyFlags)");

			BepInDependencyDependencyFlagsType = bepInEx.GetTypeSafe("BepInEx.BepInDependency/DependencyFlags");

			BepInIncompatibilityCtor = bepInEx.GetTypeSafe("BepInEx.BepInIncompatibility").FindMethodSafe("System.Void .ctor(System.String)");

			BaseUnityPluginLoggerGetter = bepInEx.GetTypeSafe("BepInEx.BaseUnityPlugin").FindProperty("Logger").GetMethod;
		}

		public MethodReference BepInPluginCtor { get; }

		public MethodReference BepInProcessCtor { get; }

		public MethodReference BepInDependencyCtorWithVersion { get; }
		public MethodReference BepInDependencyCtorWithFlags { get; }

		public TypeReference BepInDependencyDependencyFlagsType { get; }

		public MethodReference BepInIncompatibilityCtor { get; }

		public MethodReference BaseUnityPluginLoggerGetter { get; }
	}
}
