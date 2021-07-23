using Mono.Cecil;
using Mono.Cecil.Rocks;

// ReSharper disable InconsistentNaming

namespace Mason.Core.RefsAndDefs
{
	internal class StratumRefs
	{
		public StratumRefs(MscorlibRefs mscorlib, ModuleDefinition stratum)
		{
			TypeDefinition stratumPlugin = stratum.GetTypeSafe("Stratum.StratumPlugin");

			StratumPluginType = stratumPlugin;
			StratumPluginCtor = stratumPlugin.FindMethodSafe("System.Void .ctor()");
			StratumPluginDirectoriesGetter = stratumPlugin.FindPropertySafe("Directories").GetGetMethodSafe();

			PluginDirectoriesResourcesGetter =
				stratum.GetTypeSafe("Stratum.PluginDirectories").Resolve().FindPropertySafe("Resources").GetGetMethodSafe();

			TypeDefinition ext = stratum.GetTypeSafe("Stratum.Jobs.ExtAssetPipeline");
			AssetPipelineBuild = ext.FindMethodSafe("Stratum.Jobs.Job`1<Stratum.Empty> Build(Stratum.Jobs.AssetPipeline`1<Stratum.Empty>)");
			AssetPipelineBuildSequential =
				ext.FindMethodSafe(
					"Stratum.Jobs.Job`1<System.Collections.IEnumerator> BuildSequential(Stratum.Jobs.AssetPipeline`1<System.Collections.IEnumerator>)");
			AssetPipelineBuildParallel =
				ext.FindMethodSafe(
					"Stratum.Jobs.Job`1<System.Collections.IEnumerator> BuildParallel(Stratum.Jobs.AssetPipeline`1<System.Collections.IEnumerator>,Stratum.CoroutineStarter)");
			AssetPipelineAddNestedSequential = ext.FindMethodSafe(
				"Stratum.Jobs.AssetPipeline`1<System.Collections.IEnumerator> AddNestedSequential(Stratum.Jobs.AssetPipeline`1<System.Collections.IEnumerator>,System.Action`1<Stratum.Jobs.AssetPipeline`1<System.Collections.IEnumerator>>)");
			AssetPipelineAddNestedParallel = ext.FindMethodSafe(
				"Stratum.Jobs.AssetPipeline`1<System.Collections.IEnumerator> AddNestedParallel(Stratum.Jobs.AssetPipeline`1<System.Collections.IEnumerator>,System.Action`1<Stratum.Jobs.AssetPipeline`1<System.Collections.IEnumerator>>,Stratum.CoroutineStarter)");

			TypeDefinition coroutineStarter = stratum.GetTypeSafe("Stratum.CoroutineStarter");
			CoroutineStarterType = coroutineStarter;
			CoroutineStarterCtor = coroutineStarter.GetNativeConstructor();

			OpenGenericsTypes open = new(stratum);
			SetupGenerics = new Generics(open, stratum.GetTypeSafe("Stratum.Empty"));
			RuntimeGenerics = new Generics(open, mscorlib.IEnumeratorType);

			TypeDefinition lambda = mscorlib.ActionType.MakeGenericInstanceType(RuntimeGenerics.AssetPipelineType).Resolve();
			AssetPipelineAddNestedPipelineLambdaType = lambda;
			AssetPipelineAddNestedPipelineLambdaCtor = lambda.GetNativeConstructor();
		}

		public TypeReference StratumPluginType { get; }
		public MethodReference StratumPluginCtor { get; }
		public MethodReference StratumPluginDirectoriesGetter { get; }

		public MethodReference PluginDirectoriesResourcesGetter { get; }

		public MethodReference AssetPipelineBuild { get; }
		public MethodReference AssetPipelineBuildSequential { get; }
		public MethodReference AssetPipelineBuildParallel { get; }
		public MethodReference AssetPipelineAddNestedSequential { get; }
		public MethodReference AssetPipelineAddNestedParallel { get; }
		public TypeReference AssetPipelineAddNestedPipelineLambdaType { get; }
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
				AssetPipelineAddAssetMethod =
					open.AssetPipeline.FindMethodSafe(
							"Stratum.Jobs.AssetPipeline`1<TRet> AddAsset(System.String,System.String,System.String)")
						.MakeHostInstanceGeneric(tRet);

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
