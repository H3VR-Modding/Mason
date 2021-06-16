using System;
using System.IO;
using Mono.Cecil;

namespace Mason.Core.RefsAndDefs
{
	internal class RootRefsOwner : IDisposable
	{
		private readonly ModuleDefinition _mscorlib;
		private readonly ModuleDefinition _systemCore;
		private readonly ModuleDefinition _unityEngine;
		private readonly ModuleDefinition _bepInEx;
		private readonly ModuleDefinition _stratum;

		public RootRef Refs { get; }

		public RootRefsOwner(IHasBinaryPaths paths, IAssemblyResolver resolver)
		{
			ModuleDefinition Read(string path)
			{
				if (!File.Exists(path))
					throw new FileNotFoundException("Required assembly not found: " + path);

				return ModuleDefinition.ReadModule(path, new()
				{
					AssemblyResolver = resolver
				});
			}

			_mscorlib = Read(paths.Mscorlib);
			_systemCore = Read(paths.SystemCore);
			_unityEngine = Read(paths.UnityEngine);
			_bepInEx = Read(paths.BepInEx);
			_stratum = Read(paths.Stratum);

			var mscorlib = new MscorlibRefs(_mscorlib);
			Refs = new(mscorlib, new(_systemCore), new (_unityEngine), new(_bepInEx), new(mscorlib, _stratum));
		}

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
