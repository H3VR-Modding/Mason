using System;
using System.IO;
using Mono.Cecil;

namespace Mason.Core.RefsAndDefs
{
	internal class RootRefsOwner : IDisposable
	{
		private readonly ModuleDefinition _bepInEx;
		private readonly ModuleDefinition _mscorlib;
		private readonly ModuleDefinition _stratum;
		private readonly ModuleDefinition _systemCore;
		private readonly ModuleDefinition _unityEngine;

		public RootRefsOwner(IHasBinaryPaths paths, IAssemblyResolver resolver)
		{
			ModuleDefinition Read(string path)
			{
				if (!File.Exists(path))
					throw new FileNotFoundException("Required assembly not found: " + path);

				return ModuleDefinition.ReadModule(path, new ReaderParameters
				{
					AssemblyResolver = resolver
				});
			}

			_mscorlib = Read(paths.Mscorlib);
			_systemCore = Read(paths.SystemCore);
			_unityEngine = Read(paths.UnityEngine);
			_bepInEx = Read(paths.BepInEx);
			_stratum = Read(paths.Stratum);

			MscorlibRefs mscorlib = new(_mscorlib);
			Refs = new RootRef(mscorlib, new SystemCoreRefs(_systemCore), new UnityEngineRefs(_unityEngine), new BepInExRefs(_bepInEx),
				new StratumRefs(mscorlib, _stratum));
		}

		public RootRef Refs { get; }

		public void Dispose()
		{
			_mscorlib.Dispose();
			_systemCore.Dispose();
			_unityEngine.Dispose();
			_bepInEx.Dispose();
			_stratum.Dispose();
		}
	}
}
