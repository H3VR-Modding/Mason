using Mono.Cecil;
using MonoMod.Utils;

namespace Mason.Core.RefsAndDefs
{
	internal class UnityEngineRefs
	{
		public UnityEngineRefs(ModuleDefinition unityEngine)
		{
			StartCoroutineMethod = unityEngine.GetType("UnityEngine.MonoBehaviour")
				.FindMethod("UnityEngine.Coroutine StartCoroutine(System.Collections.IEnumerator)");
		}

		public MethodReference StartCoroutineMethod { get; }
	}
}
