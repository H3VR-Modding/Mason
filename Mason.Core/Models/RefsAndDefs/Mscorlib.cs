using Mono.Cecil;

// ReSharper disable InconsistentNaming

namespace Mason.Core.RefsAndDefs
{
	internal class MscorlibRefs
	{
		public MscorlibRefs(ModuleDefinition mscorlib)
		{
			ObjectCtor = mscorlib.GetTypeSafe("System.Object").FindMethodSafe("System.Void .ctor()");

			CompilerGeneratedAttributeCtor = mscorlib.GetTypeSafe("System.Runtime.CompilerServices.CompilerGeneratedAttribute")
				.FindMethodSafe("System.Void .ctor()");

			IEnumerableGetEnumeratorMethod = mscorlib.GetTypeSafe("System.Collections.IEnumerable")
				.FindMethodSafe("System.Collections.IEnumerator GetEnumerator()");

			IEnumeratorType = mscorlib.GetTypeSafe("System.Collections.IEnumerator");

			ActionType = mscorlib.GetTypeSafe("System.Action`1");
		}

		public MethodReference ObjectCtor { get; }

		public MethodReference CompilerGeneratedAttributeCtor { get; }

		public TypeReference IEnumeratorType { get; }

		public MethodReference IEnumerableGetEnumeratorMethod { get; }

		public TypeReference ActionType { get; }
	}
}
