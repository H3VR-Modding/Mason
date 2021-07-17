using Mono.Cecil;
using MonoMod.Utils;

// ReSharper disable InconsistentNaming

namespace Mason.Core.RefsAndDefs
{
	internal class MscorlibRefs
	{
		public MscorlibRefs(ModuleDefinition mscorlib)
		{
			ObjectCtor = mscorlib.GetType("System.Object").FindMethod("System.Void .ctor()");

			CompilerGeneratedAttributeCtor = mscorlib.GetType("System.Runtime.CompilerServices.CompilerGeneratedAttribute")
				.FindMethod("System.Void .ctor()");

			IEnumerableGetEnumeratorMethod = mscorlib.GetType("System.Collections.IEnumerable")
				.FindMethod("System.Collections.IEnumerator GetEnumerator()");

			IEnumeratorType = mscorlib.GetType("System.Collections.IEnumerator");

			ActionType = mscorlib.GetType("System.Action`1");
		}

		public MethodReference ObjectCtor { get; }

		public MethodReference CompilerGeneratedAttributeCtor { get; }

		public TypeReference IEnumeratorType { get; }

		public MethodReference IEnumerableGetEnumeratorMethod { get; }

		public TypeReference ActionType { get; }
	}
}
