namespace Mason.Core.RefsAndDefs
{
	internal class RootRef
	{
		public MscorlibRefs Mscorlib { get; }
		public SystemCoreRefs SystemCore { get; }
		public UnityEngineRefs UnityEngine { get; }
		public BepInExRefs BepInEx { get; }
		public StratumRefs Stratum { get; }

		public RootRef(MscorlibRefs mscorlib, SystemCoreRefs systemCore, UnityEngineRefs unityEngine, BepInExRefs bepInEx, StratumRefs stratum)
		{
			Mscorlib = mscorlib;
			SystemCore = systemCore;
			UnityEngine = unityEngine;
			BepInEx = bepInEx;
			Stratum = stratum;
		}
	}
}
