using Mono.Cecil;
using Mono.Cecil.Rocks;
using MonoMod.Utils;

namespace Mason.Core.RefsAndDefs
{
	internal class StratumRefs
	{
		public TypeReference StratumPluginType { get; }
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

		public StratumRefs(MscorlibRefs mscorlib, ModuleDefinition stratum)
		{
			var stratumPlugin = stratum.GetType("Stratum.StratumPlugin");

			StratumPluginType = stratumPlugin;
			StratumPluginDirectoriesGetter = stratumPlugin.FindProperty("Directories").GetMethod;

			PluginDirectoriesResourcesGetter = stratum.GetType("Stratum.IO.PluginDirectories").Resolve().FindProperty("Resources").GetMethod;

			var ext = stratum.GetType("Stratum.Assets.ExtAssetPipeline");
			AssetPipelineBuild = ext.FindMethod("Stratum.Jobs.Job`1<Stratum.Empty> Build(Stratum.Assets.AssetPipeline`1<Stratum.Empty>)");
			AssetPipelineBuildSequential = ext.FindMethod("Stratum.Jobs.Job`1<System.Collections.IEnumerator> BuildSequential(Stratum.Assets.AssetPipeline`1<System.Collections.IEnumerator>)");
			AssetPipelineBuildParallel = ext.FindMethod("Stratum.Jobs.Job`1<System.Collections.IEnumerator> BuildParallel(Stratum.Assets.AssetPipeline`1<System.Collections.IEnumerator>,Stratum.Coroutines.CoroutineStarter)");
			AssetPipelineAddNestedSequential = ext.FindMethod("Stratum.Assets.AssetPipeline`1<System.Collections.IEnumerator> AddNestedSequential(Stratum.Assets.AssetPipeline`1<System.Collections.IEnumerator>,System.Action`1<Stratum.Assets.AssetPipeline`1<System.Collections.IEnumerator>>)");
			AssetPipelineAddNestedParallel = ext.FindMethod("Stratum.Assets.AssetPipeline`1<System.Collections.IEnumerator> AddNestedParallel(Stratum.Assets.AssetPipeline`1<System.Collections.IEnumerator>,System.Action`1<Stratum.Assets.AssetPipeline`1<System.Collections.IEnumerator>>,Stratum.Coroutines.CoroutineStarter)");

			var coroutineStarter = stratum.GetType("Stratum.Coroutines.CoroutineStarter");
			CoroutineStarterType = coroutineStarter;
			CoroutineStarterCtor = coroutineStarter.GetNativeConstructor();

			var open = new OpenGenericsTypes(stratum);
			SetupGenerics = new(open, stratum.GetType("Stratum.Empty"));
			RuntimeGenerics = new(open, mscorlib.IEnumeratorType);

			var lambda = mscorlib.ActionType.MakeGenericInstanceType(RuntimeGenerics.AssetPipelineType).Resolve();
			AssetPipelineAddNestedPipelineLambdaType = lambda;
			AssetPipelineAddNestedPipelineLambdaCtor = lambda.GetNativeConstructor();
		}

		public readonly ref struct OpenGenericsTypes
		{
			public TypeReference AssetPipeline { get; }

			public TypeReference IReadOnlyStageContext { get; }

			public TypeReference IStageContextType { get; }

			public TypeReference Job { get; }

			public OpenGenericsTypes(ModuleDefinition stratum)
			{
				AssetPipeline = stratum.GetType("Stratum.Assets.AssetPipeline`1");
				IReadOnlyStageContext = stratum.GetType("Stratum.IReadOnlyStageContext`1");
				IStageContextType = stratum.GetType("Stratum.IStageContext`1");
				Job = stratum.GetType("Stratum.Jobs.Job`1");
			}
		}

		public class Generics
		{
			public TypeReference AssetPipelineType { get; }
			public MethodReference AssetPipelineCtor { get; }
			public MethodReference AssetPipelineAddAssetMethod { get; }

			public MethodReference IReadOnlyStageContextStageGetter { get; }

			public TypeReference IStageContextType { get; }

			public MethodReference JobInvokeMethod { get; }

			public Generics(OpenGenericsTypes open, TypeReference tRet)
			{
				var closed = new ClosedTypes(open, tRet);

				AssetPipelineType = closed.AssetPipeline;
				AssetPipelineCtor = closed.AssetPipeline.FindMethod("System.Void .ctor(System.IO.DirectoryInfo)");
				AssetPipelineAddAssetMethod = closed.AssetPipeline.FindMethod("Stratum.Assets.AssetPipeline`1<TRet> AddAsset(System.String,System.String,System.String)");

				IReadOnlyStageContextStageGetter = closed.IReadOnlyStageContext.FindProperty("Stage").GetMethod;

				IStageContextType = closed.IStageContextType;

				JobInvokeMethod = closed.Job.FindMethod("Stratum.Jobs.Job`1/TRet Invoke(Stratum.IStage`1<TRet>,BepInEx.Logging.ManualLogSource)");
			}

			private readonly ref struct ClosedTypes
			{
				public TypeDefinition AssetPipeline { get; }

				public TypeDefinition IReadOnlyStageContext { get; }

				public TypeDefinition IStageContextType { get; }

				public TypeDefinition Job { get; }

				public ClosedTypes(OpenGenericsTypes open, TypeReference t)
				{
					AssetPipeline = open.AssetPipeline.MakeGenericInstanceType(t).Resolve();
					IReadOnlyStageContext = open.IReadOnlyStageContext.MakeGenericInstanceType(t).Resolve();
					IStageContextType = open.IStageContextType.MakeGenericInstanceType(t).Resolve();
					Job = open.Job.MakeGenericInstanceType(t).Resolve();
				}
			}
		}
	}
}
