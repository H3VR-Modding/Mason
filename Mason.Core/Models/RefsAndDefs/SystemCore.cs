using Mono.Cecil;
using MonoMod.Utils;

namespace Mason.Core.RefsAndDefs
{
	internal class SystemCoreRefs
	{
		public SystemCoreRefs(ModuleDefinition systemCore)
		{
			EnumerableEmptyMethod = systemCore.GetType("System.Linq.Enumerable")
				.FindMethod("System.Collections.Generic.IEnumerable`1<TResult> Empty<TResult>()");
		}

		public MethodReference EnumerableEmptyMethod { get; }
	}
}
