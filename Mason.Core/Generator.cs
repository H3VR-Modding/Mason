using System.Collections.Generic;
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
		private const TypeAttributes PluginAttributes = TypeAttributes.Public | TypeAttributes.Sealed;

		private const TypeAttributes CompilerGeneratedAttributes = TypeAttributes.NestedPrivate | TypeAttributes.Sealed |
		                                                           TypeAttributes.Serializable | TypeAttributes.BeforeFieldInit;

		private const MethodAttributes PublicOverrideAttributes =
			MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual;

		private const MethodAttributes LambdaMethodAttributes = MethodAttributes.Assembly | MethodAttributes.HideBySig;

		private const FieldAttributes LambdaCacheAttributes = FieldAttributes.Public | FieldAttributes.Static | FieldAttributes.InitOnly;

		private const MethodAttributes CctorAttributes =
			MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.Static | MethodAttributes.SpecialName |
			MethodAttributes.RTSpecialName;

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

			private CgCache? _cg;
			private Stack<int>? _nestedIndices;
			private FieldDefinition? _coroutineCache;

			private CgCache Cg
			{
				get
				{
					if (_cg == null)
					{
						TypeReference @void = _module.TypeSystem.Void;

						TypeDefinition type = new(null, "<>c", CompilerGeneratedAttributes);
						type.CustomAttributes.Add(
							new CustomAttribute(_module.ImportReference(_refs.Mscorlib.CompilerGeneratedAttributeCtor)));

						FieldDefinition cache = new("<>Instance", LambdaCacheAttributes, type);
						type.Fields.Add(cache);

						MethodDefinition ctor = new(".ctor", CtorAttributes, @void);
						{
							ILProcessor il = ctor.Body.GetILProcessor();

							il.Emit(OpCodes.Ldarg_0);
							il.Emit(OpCodes.Call, _module.ImportReference(_refs.Mscorlib.ObjectCtor));
							il.Emit(OpCodes.Ret);
						}

						MethodDefinition cctor = new(".cctor", CctorAttributes, @void);
						{
							ILProcessor il = cctor.Body.GetILProcessor();

							il.Emit(OpCodes.Newobj, ctor);
							il.Emit(OpCodes.Stsfld, cache);
							il.Emit(OpCodes.Ret);
						}

						type.Methods.Add(cctor);
						type.Methods.Add(ctor);

						_type.NestedTypes.Add(type);

						_cg = new CgCache(type, cache);
					}

					return _cg;
				}
			}

			private Stack<int> NestedIndices => _nestedIndices ??= new Stack<int>();

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
				_type = new TypeDefinition(null, "Plugin", PluginAttributes, @base);
				_module.Types.Add(_type);

				_cg = null;
				_nestedIndices = null;
				_coroutineCache = null;
			}

			private void EmitStartCoroutineNewobj(ILProcessor il)
			{
				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Ldftn, _module.ImportReference(_refs.UnityEngine.StartCoroutineMethod));
				il.Emit(OpCodes.Newobj, _module.ImportReference(_refs.Stratum.CoroutineStarterCtor));
			}

			private void EmitStartCoroutineReference(ILProcessor il)
			{
				if (_coroutineCache == null)
				{
					// First call

					_coroutineCache = new FieldDefinition("<>CoroutineStarter", FieldAttributes.Public | FieldAttributes.Static,
						_module.ImportReference(_refs.Stratum.CoroutineStarterType));

					Cg.Type.Fields.Add(_coroutineCache);

					EmitStartCoroutineNewobj(il);
					il.Emit(OpCodes.Dup);
					il.Emit(OpCodes.Stsfld, _coroutineCache);
				}
				else
				{
					il.Emit(OpCodes.Ldsfld, _coroutineCache);
				}
			}

			private void EmitStartCoroutineReferenceLast(ILProcessor il)
			{
				if (_coroutineCache == null)
					// No need to store the last call
					EmitStartCoroutineNewobj(il);
				else
					il.Emit(OpCodes.Ldsfld, _coroutineCache);
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
				il.Emit(OpCodes.Ldstr, asset.Path);
				il.Emit(OpCodes.Ldstr, asset.Plugin);
				il.Emit(OpCodes.Ldstr, asset.Loader);
				il.Emit(OpCodes.Call, _module.ImportReference(tRet.AssetPipelineAddAssetMethod));
			}

			private void EmitAssetPipelineExecutor(ILProcessor il, StratumRefs.Generics tRet)
			{
				il.Emit(OpCodes.Ldarg_1);
				il.Emit(OpCodes.Callvirt, _module.ImportReference(tRet.IReadOnlyStageContextStageGetter));
				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Call, _module.ImportReference(_refs.BepInEx.BaseUnityPluginLoggerGetter));
				il.Emit(OpCodes.Callvirt, _module.ImportReference(tRet.JobInvokeMethod));
			}

			private void EmitNestedPipeline(ILProcessor il, AssetPipeline pipeline)
			{
				var name = "<OnRuntime>";
				foreach (int index in NestedIndices)
					name += "_" + index;

				MethodDefinition lambda;
				{
					TypeReference @void = _module.TypeSystem.Void;
					lambda = new MethodDefinition(name, LambdaMethodAttributes, @void);
					lambda.Parameters.Add(new ParameterDefinition("pipeline", ParameterAttributes.None,
						_module.ImportReference(_refs.Stratum.RuntimeGenerics.AssetPipelineType)));

					{
						ILProcessor lambdaIL = lambda.Body.GetILProcessor();

						lambdaIL.Emit(OpCodes.Ldarg_1);
						EmitAssetPipelineBody(lambdaIL, pipeline);
						lambdaIL.Emit(OpCodes.Pop);
						lambdaIL.Emit(OpCodes.Ret);
					}

					Cg.Type.Methods.Add(lambda);
				}

				il.Emit(OpCodes.Ldsfld, Cg.Cache);
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

					EmitStartCoroutineReference(il);
				}

				il.Emit(OpCodes.Call, _module.ImportReference(adder));
			}

			private void EmitAssetPipelineBody(ILProcessor il, AssetPipeline pipeline)
			{
				var i = 0;
				foreach (AssetPipeline nested in pipeline.Nested)
				{
					NestedIndices.Push(i++);
					EmitNestedPipeline(il, nested);
					NestedIndices.Pop();
				}

				foreach (Asset asset in pipeline.Assets)
					EmitAsset(il, _refs.Stratum.RuntimeGenerics, asset);
			}

			private void AddCtor()
			{
				MethodDefinition method = new(".ctor", CtorAttributes, _module.TypeSystem.Void);

				ILProcessor il = method.Body.GetILProcessor();

				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Call, _module.ImportReference(_refs.Stratum.StratumPluginCtor));
				il.Emit(OpCodes.Ret);

				_type.Methods.Add(method);
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
				}

				il.Emit(OpCodes.Ret);

				_type.Methods.Add(method);
			}

			private void AddRuntimeMethod()
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
					EmitAssetPipelineCtor(il, generics);

					EmitAssetPipelineBody(il, pipeline);

					if (pipeline.Sequential)
					{
						il.Emit(OpCodes.Call, _module.ImportReference(_refs.Stratum.AssetPipelineBuildSequential));
					}
					else
					{
						EmitStartCoroutineReferenceLast(il);
						il.Emit(OpCodes.Call, _module.ImportReference(_refs.Stratum.AssetPipelineBuildParallel));
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
							new(str, source.DependencyGUID), new(str, source.MinimumVersion.ToString())
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
				AddCtor();
				AddSetupMethod();
				AddRuntimeMethod();

				// If you see another mscorlib reference:
				// All our libs depend on mscorlib with a "" Culture, but Mono.Cecil uses a null Culture internally. It's
				// AssemblyNameReference equality method thinks they are not equal, so it adds another mscorlib reference. We could submit
				// an issue, but net35 support has been dropped, so any fix would not work for us.

				return _module.Assembly;
			}

			private class CgCache
			{
				public CgCache(TypeDefinition type, FieldReference cache)
				{
					Type = type;
					Cache = cache;
				}

				public TypeDefinition Type { get; }
				public FieldReference Cache { get; }
			}
		}
	}
}
