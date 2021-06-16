using Mono.Cecil;
using MonoMod.Utils;

namespace Mason.Core.RefsAndDefs
{
	internal class MscorlibRefs
	{
		public MethodReference ObjectCtor { get; }

		public MethodReference CompilerGeneratedAttributeCtor { get; }

		public TypeReference IEnumeratorType { get; }

		public MethodReference IEnumerableGetEnumeratorMethod { get; }

		public TypeReference ActionType { get; }

		public MscorlibRefs(ModuleDefinition mscorlib)
		{
			ObjectCtor = mscorlib.GetType("System.Object").FindMethod("System.Void .ctor()");

			CompilerGeneratedAttributeCtor = mscorlib.GetType("System.Runtime.CompilerServices.CompilerGeneratedAttribute").FindMethod("System.Void .ctor()");

			IEnumerableGetEnumeratorMethod = mscorlib.GetType("System.Collections.IEnumerable").FindMethod("System.Collections.IEnumerator GetEnumerator()");

			IEnumeratorType = mscorlib.GetType("System.Collections.IEnumerator");

			ActionType = mscorlib.GetType("System.Action`1");
		}
	}
}
