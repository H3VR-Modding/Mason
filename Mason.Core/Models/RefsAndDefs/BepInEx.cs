using System.Linq;
using Mono.Cecil;
using MonoMod.Utils;

namespace Mason.Core.RefsAndDefs
{
	internal class BepInExRefs
	{
		public MethodReference BepInPluginCtor { get; }

		public MethodReference BepInProcessCtor { get; }

		public MethodReference BepInDependencyCtorWithVersion { get; }
		public MethodReference BepInDependencyCtorWithFlags { get; }
		public TypeReference BepInDependencyDependencyFlagsType { get; }

		public MethodReference BepInIncompatibilityCtor { get; }

		public MethodReference BaseUnityPluginLoggerGetter { get; }

		public BepInExRefs(ModuleDefinition bepInEx)
		{
			BepInPluginCtor = bepInEx.GetType("BepInEx.BepInPlugin").FindMethod("System.Void .ctor(System.String,System.String,System.String)");

            BepInProcessCtor = bepInEx.GetType("BepInEx.BepInProcess").FindMethod("System.Void .ctor(System.String)");

            var bepindep = bepInEx.GetType("BepInEx.BepInDependency");
            BepInDependencyCtorWithVersion = bepindep.FindMethod("System.Void .ctor(System.String,System.String)");
            BepInDependencyCtorWithFlags = bepindep.FindMethod("System.Void .ctor(System.String,BepInEx.BepInDependency/DependencyFlags)");
            BepInDependencyDependencyFlagsType = bepindep.NestedTypes.Single(x => x.Name == "DependencyFlags");

            BepInIncompatibilityCtor = bepInEx.GetType("BepInEx.BepInIncompatibility").FindMethod("System.Void .ctor(System.String)");

            BaseUnityPluginLoggerGetter = bepInEx.GetType("BepInEx.BaseUnityPlugin").FindProperty("Logger").GetMethod;
		}
	}
}
