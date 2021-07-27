using Mono.Cecil;
using Mono.Cecil.Rocks;

// ReSharper disable InconsistentNaming

namespace Mason.Core.RefsAndDefs
{
	internal class StratumRefs
	{
		public StratumRefs(MscorlibRefs mscorlib, ModuleDefinition stratum)
		{
			OpenGenericsTypes open = new(stratum);
			SetupGenerics = new Generics(open, stratum.GetTypeSafe("Stratum.Empty"));
			RuntimeGenerics = new Generics(open, mscorlib.IEnumeratorType);

			TypeDefinition stratumPlugin = stratum.GetTypeSafe("Stratum.StratumPlugin");

			StratumPluginType = stratumPlugin;
			StratumPluginCtor = stratumPlugin.FindMethodSafe("System.Void .ctor()");
			StratumPluginDirectoriesGetter = stratumPlugin.FindPropertySafe("Directories").GetGetMethodSafe();

			PluginDirectoriesResourcesGetter =
				stratum.GetTypeSafe("Stratum.PluginDirectories").Resolve().FindPropertySafe("Resources").GetGetMethodSafe();

			PipelineWithName = stratum.GetTypeSafe("Stratum.Jobs.Pipeline`2")
				.FindMethodSafe("Stratum.Jobs.Pipeline`2/TSelf WithName(System.String)");

			TypeDefinition ext = stratum.GetTypeSafe("Stratum.Jobs.ExtPipeline");
			AssetPipelineBuild = ext.FindMethodSafe("Stratum.Jobs.Job`1<Stratum.Empty> Build<TSelf>(TSelf)")
				.MakeGenericMethod(SetupGenerics.AssetPipelineType);
			AssetPipelineBuildSequential =
				ext.FindMethodSafe("Stratum.Jobs.Job`1<System.Collections.IEnumerator> BuildSequential<TSelf>(TSelf)")
					.MakeGenericMethod(RuntimeGenerics.AssetPipelineType);
			AssetPipelineBuildParallel =
				ext.FindMethodSafe("Stratum.Jobs.Job`1<System.Collections.IEnumerator> BuildParallel<TSelf>(TSelf,Stratum.CoroutineStarter)")
					.MakeGenericMethod(RuntimeGenerics.AssetPipelineType);
			AssetPipelineAddNestedSequential = ext.FindMethodSafe("TSelf AddNestedSequential<TSelf>(TSelf,System.Action`1<TSelf>)")
				.MakeGenericMethod(RuntimeGenerics.AssetPipelineType);
			AssetPipelineAddNestedParallel = ext.FindMethodSafe(
					"TSelf AddNestedParallel<TSelf>(TSelf,System.Action`1<TSelf>,Stratum.CoroutineStarter)")
				.MakeGenericMethod(RuntimeGenerics.AssetPipelineType);

			TypeDefinition coroutineStarter = stratum.GetTypeSafe("Stratum.CoroutineStarter");
			CoroutineStarterType = coroutineStarter;
			CoroutineStarterCtor = coroutineStarter.GetNativeConstructor();

			AssetPipelineAddNestedPipelineLambdaCtor = mscorlib.ActionType.MakeGenericInstanceType(RuntimeGenerics.AssetPipelineType)
				.Resolve().GetNativeConstructor();
		}

		public TypeReference StratumPluginType { get; }
		public MethodReference StratumPluginCtor { get; }
		public MethodReference StratumPluginDirectoriesGetter { get; }

		public MethodReference PluginDirectoriesResourcesGetter { get; }

		public MethodReference PipelineWithName { get; }

		public MethodReference AssetPipelineBuild { get; }
		public MethodReference AssetPipelineBuildSequential { get; }
		public MethodReference AssetPipelineBuildParallel { get; }
		public MethodReference AssetPipelineAddNestedSequential { get; }
		public MethodReference AssetPipelineAddNestedParallel { get; }
		public MethodReference AssetPipelineAddNestedPipelineLambdaCtor { get; }

		public MethodReference CoroutineStarterCtor { get; }
		public TypeReference CoroutineStarterType { get; }

		public Generics SetupGenerics { get; }
		public Generics RuntimeGenerics { get; }

		public readonly ref struct OpenGenericsTypes
		{
			public TypeDefinition AssetPipeline { get; }

			public TypeDefinition IReadOnlyStageContext { get; }

			public TypeDefinition IStageContextType { get; }

			public TypeDefinition Job { get; }

			public OpenGenericsTypes(ModuleDefinition stratum)
			{
				AssetPipeline = stratum.GetTypeSafe("Stratum.Jobs.AssetPipeline`1");
				IReadOnlyStageContext = stratum.GetTypeSafe("Stratum.IReadOnlyStageContext`1");
				IStageContextType = stratum.GetTypeSafe("Stratum.IStageContext`1");
				Job = stratum.GetTypeSafe("Stratum.Jobs.Job`1");
			}
		}

		public class Generics
		{
			public Generics(OpenGenericsTypes open, TypeReference tRet)
			{
				AssetPipelineType = open.AssetPipeline.MakeGenericInstanceType(tRet);
				AssetPipelineCtor = open.AssetPipeline.FindMethodSafe("System.Void .ctor(System.IO.DirectoryInfo)")
					.MakeHostInstanceGeneric(tRet);
				AssetPipelineAddAssetMethod = open.AssetPipeline.BaseType.Resolve()
					.FindMethodSafe("Stratum.Jobs.AssetPipeline`2/TSelf AddAsset(System.String,System.String,System.String)")
					.MakeHostInstanceGeneric(tRet, AssetPipelineType);

				IReadOnlyStageContextStageGetter = open.IReadOnlyStageContext.FindPropertySafe("Stage").GetGetMethodSafe()
					.MakeHostInstanceGeneric(tRet);

				IStageContextType = open.IStageContextType.MakeGenericInstanceType(tRet);

				JobInvokeMethod =
					open.Job.FindMethodSafe("Stratum.Jobs.Job`1/TRet Invoke(Stratum.IStage`1<TRet>,BepInEx.Logging.ManualLogSource)")
						.MakeHostInstanceGeneric(tRet);
			}

			public TypeReference AssetPipelineType { get; }
			public MethodReference AssetPipelineCtor { get; }
			public MethodReference AssetPipelineAddAssetMethod { get; }

			public MethodReference IReadOnlyStageContextStageGetter { get; }

			public TypeReference IStageContextType { get; }

			public MethodReference JobInvokeMethod { get; }
		}
	}
}
