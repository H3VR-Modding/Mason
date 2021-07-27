using Mono.Cecil;

// ReSharper disable InconsistentNaming

namespace Mason.Core.RefsAndDefs
{
	internal class MscorlibRefs
	{
		public MscorlibRefs(ModuleDefinition mscorlib)
		{
			IEnumerableGetEnumeratorMethod = mscorlib.GetTypeSafe("System.Collections.IEnumerable")
				.FindMethodSafe("System.Collections.IEnumerator GetEnumerator()");

			IEnumeratorType = mscorlib.GetTypeSafe("System.Collections.IEnumerator");

			ActionType = mscorlib.GetTypeSafe("System.Action`1");
		}

		public TypeReference IEnumeratorType { get; }

		public MethodReference IEnumerableGetEnumeratorMethod { get; }

		public TypeReference ActionType { get; }
	}
}
