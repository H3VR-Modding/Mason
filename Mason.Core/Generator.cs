using System.Collections.Generic;
using System.Linq;
using BepInEx;
using Mason.Core.IR;
using Mason.Core.RefsAndDefs;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using MonoMod.Utils;

namespace Mason.Core
{
	internal class Generator
	{
		private const MethodAttributes PublicOverrideAttributes =
			MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual;

		private const MethodAttributes CtorAttributes =
			MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName;

		private readonly RootRef _refs;

		private readonly IAssemblyResolver _resolver;

		public Generator(IAssemblyResolver resolver, RootRef refs)
		{
			_resolver = resolver;
			_refs = refs;
		}

		public AssemblyDefinition Generate(Mod mod)
		{
			return new Scoped(this, mod).Generate();
		}

		private struct Scoped
		{
			private readonly RootRef _refs;
			private readonly Mod _mod;

			private readonly ModuleDefinition _module;
			private readonly TypeDefinition _type;

			private readonly Stack<int> _nestedIndices;

			public Scoped(Generator generator, Mod mod)
			{
				_refs = generator._refs;
				_mod = mod;

				BepInPlugin plugin = mod.Metadata.Plugin;
				AssemblyNameDefinition name = new(plugin.GUID, plugin.Version);
				var asm = AssemblyDefinition.CreateAssembly(name, plugin.GUID, new ModuleParameters
				{
					AssemblyResolver = generator._resolver,
					Runtime = TargetRuntime.Net_2_0
				});

				_module = asm.MainModule;

				TypeReference @base = _module.ImportReference(_refs.Stratum.StratumPluginType);
				_type = new TypeDefinition(null, "Plugin", TypeAttributes.Public | TypeAttributes.Sealed, @base);
				_module.Types.Add(_type);

				_nestedIndices = new Stack<int>();
			}

			private void EmitStartCoroutineReference(ILProcessor il, MethodDefinition ctor, ref FieldDefinition? startCoroutine)
			{
				if (startCoroutine == null)
				{
					// First call

					startCoroutine = new FieldDefinition("_startCoroutine", FieldAttributes.Private | FieldAttributes.InitOnly,
						_module.ImportReference(_refs.Stratum.CoroutineStarterType));

					_type.Fields.Add(startCoroutine);

					Collection<Instruction> ctorIl = ctor.Body.Instructions;

					int BeforeRet()
					{
						return ctorIl.Count - 1;
					}

					ctorIl.Insert(BeforeRet(), Instruction.Create(OpCodes.Ldarg_0));
					ctorIl.Insert(BeforeRet(), Instruction.Create(OpCodes.Dup));
					ctorIl.Insert(BeforeRet(), Instruction.Create(OpCodes.Ldftn, _module.ImportReference(_refs.UnityEngine.StartCoroutineMethod)));
					ctorIl.Insert(BeforeRet(), Instruction.Create(OpCodes.Newobj, _module.ImportReference(_refs.Stratum.CoroutineStarterCtor)));
					ctorIl.Insert(BeforeRet(), Instruction.Create(OpCodes.Stfld, startCoroutine));
				}

				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Ldfld, startCoroutine);
			}

			private void EmitAssetPipelineCtor(ILProcessor il, StratumRefs.Generics tRet)
			{
				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Call, _module.ImportReference(_refs.Stratum.StratumPluginDirectoriesGetter));
				il.Emit(OpCodes.Callvirt, _module.ImportReference(_refs.Stratum.PluginDirectoriesResourcesGetter));
				il.Emit(OpCodes.Newobj, _module.ImportReference(tRet.AssetPipelineCtor));
			}

			private void EmitAsset(ILProcessor il, StratumRefs.Generics tRet, Asset asset)
			{
				il.Emit(OpCodes.Ldstr, asset.Plugin);
				il.Emit(OpCodes.Ldstr, asset.Loader);
				il.Emit(OpCodes.Ldstr, asset.Path);
				il.Emit(OpCodes.Callvirt, _module.ImportReference(tRet.AssetPipelineAddAssetMethod));
			}

			private void EmitAssetPipelineExecutor(ILProcessor il, StratumRefs.Generics tRet)
			{
				il.Emit(OpCodes.Ldarg_1);
				il.Emit(OpCodes.Callvirt, _module.ImportReference(tRet.IReadOnlyStageContextStageGetter));
				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Call, _module.ImportReference(_refs.BepInEx.BaseUnityPluginLoggerGetter));
				il.Emit(OpCodes.Callvirt, _module.ImportReference(tRet.JobInvokeMethod));
			}

			private void EmitNestedPipeline(ILProcessor il, AssetPipeline pipeline, MethodDefinition ctor,
				ref FieldDefinition? startCoroutine)
			{
				var name = _nestedIndices.Aggregate("<OnRuntime>", (current, index) => current + ("_" + index));

				MethodDefinition lambda;
				{
					TypeReference @void = _module.TypeSystem.Void;
					lambda = new MethodDefinition(name, MethodAttributes.Assembly | MethodAttributes.HideBySig, @void);
					lambda.Parameters.Add(new ParameterDefinition("pipeline", ParameterAttributes.None,
						_module.ImportReference(_refs.Stratum.RuntimeGenerics.AssetPipelineType)));

					{
						ILProcessor lambdaIL = lambda.Body.GetILProcessor();

						lambdaIL.Emit(OpCodes.Ldarg_1);
						EmitAssetPipelineBody(lambdaIL, pipeline, ctor, ref startCoroutine);
						lambdaIL.Emit(OpCodes.Pop);
						lambdaIL.Emit(OpCodes.Ret);
					}

					_type.Methods.Add(lambda);
				}

				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Ldftn, lambda);
				il.Emit(OpCodes.Newobj, _module.ImportReference(_refs.Stratum.AssetPipelineAddNestedPipelineLambdaCtor));

				MethodReference adder;
				if (pipeline.Sequential)
				{
					adder = _refs.Stratum.AssetPipelineAddNestedSequential;
				}
				else
				{
					adder = _refs.Stratum.AssetPipelineAddNestedParallel;

					EmitStartCoroutineReference(il, ctor, ref startCoroutine);
				}

				il.Emit(OpCodes.Callvirt, _module.ImportReference(adder));
			}

			private void EmitAssetPipelineBody(ILProcessor il, AssetPipeline pipeline, MethodDefinition ctor,
				ref FieldDefinition? startCoroutine)
			{
				if (pipeline.Name is { } name)
				{
					il.Emit(OpCodes.Ldstr, name);
					il.Emit(OpCodes.Callvirt, _refs.Stratum.PipelineWithName);
				}

				var i = 0;
				foreach (AssetPipeline nested in pipeline.Nested)
				{
					_nestedIndices.Push(i++);
					EmitNestedPipeline(il, nested, ctor, ref startCoroutine);
					_nestedIndices.Pop();
				}

				foreach (Asset asset in pipeline.Assets)
					EmitAsset(il, _refs.Stratum.RuntimeGenerics, asset);
			}

			private MethodDefinition AddCtor()
			{
				MethodDefinition method = new(".ctor", CtorAttributes, _module.TypeSystem.Void);

				ILProcessor il = method.Body.GetILProcessor();

				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Call, _module.ImportReference(_refs.Stratum.StratumPluginCtor));
				il.Emit(OpCodes.Ret);

				_type.Methods.Add(method);

				return method;
			}

			private void AddSetupMethod()
			{
				StratumRefs.Generics generics = _refs.Stratum.SetupGenerics;

				MethodDefinition method = new("OnSetup", PublicOverrideAttributes, _module.TypeSystem.Void);
				method.Parameters.Add(new ParameterDefinition("ctx", ParameterAttributes.None,
					_module.ImportReference(generics.IStageContextType)));

				ILProcessor il = method.Body.GetILProcessor();
				IList<Asset>? assets = _mod.Assets?.Setup;

				if (assets != null)
				{
					EmitAssetPipelineCtor(il, generics);

					foreach (Asset asset in assets)
						EmitAsset(il, generics, asset);

					il.Emit(OpCodes.Call, _module.ImportReference(_refs.Stratum.AssetPipelineBuild));
					EmitAssetPipelineExecutor(il, generics);
					il.Emit(OpCodes.Pop);
				}

				il.Emit(OpCodes.Ret);

				_type.Methods.Add(method);
			}

			private void AddRuntimeMethod(MethodDefinition ctor)
			{
				StratumRefs.Generics generics = _refs.Stratum.RuntimeGenerics;

				MethodDefinition method = new("OnRuntime", PublicOverrideAttributes,
					_module.ImportReference(_refs.Mscorlib.IEnumeratorType));
				method.Parameters.Add(new ParameterDefinition("ctx", ParameterAttributes.None,
					_module.ImportReference(generics.IStageContextType)));

				ILProcessor il = method.Body.GetILProcessor();
				AssetPipeline? pipeline = _mod.Assets?.Runtime;

				if (pipeline != null)
				{
					FieldDefinition? startCoroutine = null;

					EmitAssetPipelineCtor(il, generics);

					EmitAssetPipelineBody(il, pipeline, ctor, ref startCoroutine);

					if (pipeline.Sequential)
					{
						il.Emit(OpCodes.Callvirt, _module.ImportReference(_refs.Stratum.AssetPipelineBuildSequential));
					}
					else
					{
						EmitStartCoroutineReference(il, ctor, ref startCoroutine);
						il.Emit(OpCodes.Callvirt, _module.ImportReference(_refs.Stratum.AssetPipelineBuildParallel));
					}

					EmitAssetPipelineExecutor(il, generics);
				}
				else
				{
					il.Emit(OpCodes.Call, _module.ImportReference(_refs.SystemCore.EnumerableEmptyMethod));
					il.Emit(OpCodes.Callvirt, _module.ImportReference(_refs.Mscorlib.IEnumerableGetEnumeratorMethod));
				}

				il.Emit(OpCodes.Ret);

				_type.Methods.Add(method);
			}

			private void AddMetadata()
			{
				BepInExRefs refs = _refs.BepInEx;
				TypeReference str = _module.TypeSystem.String;
				Metadata metadata = _mod.Metadata;
				Collection<CustomAttribute> attr = _type.CustomAttributes;

				{
					BepInPlugin source = metadata.Plugin;
					CustomAttribute dest = new(_module.ImportReference(refs.BepInPluginCtor));

					dest.ConstructorArguments.Add(new CustomAttributeArgument(str, source.GUID));
					dest.ConstructorArguments.Add(new CustomAttributeArgument(str, source.Name));
					dest.ConstructorArguments.Add(new CustomAttributeArgument(str, source.Version.ToString()));

					attr.Add(dest);
				}


				foreach (BepInProcess source in metadata.Processes.OrEmptyIfNull())
				{
					CustomAttribute dest = new(_module.ImportReference(refs.BepInProcessCtor));

					dest.ConstructorArguments.Add(new CustomAttributeArgument(str, source.ProcessName));

					attr.Add(dest);
				}

				foreach (BepInDependency source in metadata.Dependencies.OrEmptyIfNull())
				{
					MethodReference ctor;
					CustomAttributeArgument[] args;

					if (source.Flags == BepInDependency.DependencyFlags.HardDependency)
					{
						ctor = refs.BepInDependencyCtorWithVersion;

						args = new CustomAttributeArgument[]
						{
							new(str, source.DependencyGUID),
							new(str, source.MinimumVersion.ToString())
						};
					}
					else
					{
						ctor = refs.BepInDependencyCtorWithFlags;

						args = new CustomAttributeArgument[]
						{
							new(str, source.DependencyGUID),
							new(_module.ImportReference(refs.BepInDependencyDependencyFlagsType), source.Flags)
						};
					}

					CustomAttribute dest = new(_module.ImportReference(ctor));
					dest.ConstructorArguments.AddRange(args);

					attr.Add(dest);
				}

				foreach (BepInIncompatibility source in metadata.Incompatibilities.OrEmptyIfNull())
				{
					CustomAttribute dest = new(_module.ImportReference(refs.BepInIncompatibilityCtor));

					dest.ConstructorArguments.Add(new CustomAttributeArgument(str, source.IncompatibilityGUID));

					attr.Add(dest);
				}
			}

			public AssemblyDefinition Generate()
			{
				AddMetadata();

				MethodDefinition ctor = AddCtor();
				AddSetupMethod();
				AddRuntimeMethod(ctor);

				// If you see another mscorlib reference:
				// All our libs depend on mscorlib with a "" Culture, but Mono.Cecil uses a null Culture internally. It's
				// AssemblyNameReference equality method thinks they are not equal, so it adds another mscorlib reference. We could submit
				// an issue, but net35 support has been dropped, so any fix would not work for us.

				return _module.Assembly;
			}
		}
	}
}
