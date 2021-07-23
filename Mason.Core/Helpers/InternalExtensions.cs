using System;
using System.Collections.Generic;
using System.Linq;
using Mason.Core.Markup;
using Mono.Cecil;
using Mono.Cecil.Rocks;
using MonoMod.Utils;
using Newtonsoft.Json;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using Version = System.Version;

namespace Mason.Core
{
	internal static class InternalExtensions
	{
		public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> @this, out TKey key, out TValue value)
		{
			key = @this.Key;
			value = @this.Value;
		}

		public static MethodDefinition GetNativeConstructor(this TypeDefinition @this)
		{
			return @this.FindMethod("System.Void .ctor(System.Object,System.IntPtr)");
		}

		public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> @this)
		{
			return @this.Where(x => x != null)!;
		}

		public static IList<T>? Optimize<T>(this IList<T> @this)
		{
			return @this.Count == 0 ? null : @this;
		}

		public static IEnumerable<T> OrEmptyIfNull<T>(this IEnumerable<T>? @this)
		{
			return @this ?? Enumerable.Empty<T>();
		}

		public static MarkupIndex GetIndex(this IJsonLineInfo @this)
		{
			return new(@this.LineNumber, @this.LinePosition);
		}

		public static MarkupIndex GetIndex(this Mark @this)
		{
			return new(@this.Line, @this.Column);
		}

		public static MarkupIndex GetIndex(this JsonSerializationException @this)
		{
			return new(@this.LineNumber, @this.LinePosition);
		}

		public static MarkupRange GetRange(this ParsingEvent @this)
		{
			return new(@this.Start.GetIndex(), @this.End.GetIndex());
		}

		public static int GoodCompareTo(this Version @this, Version other)
		{
			int major = @this.Major.CompareTo(other.Major);
			if (major != 0)
				return major;

			int minor = @this.Minor.CompareTo(other.Minor);
			if (minor != 0)
				return minor;

			// It do be performant doe
			int thisBuild = @this.Build;
			if (thisBuild != -1)
			{
				int otherBuild = other.Build;
				if (otherBuild != -1)
				{
					int build = thisBuild.CompareTo(otherBuild);
					if (build != 0)
						return build;

					int thisRevision = @this.Revision;
					if (thisRevision != -1)
					{
						int otherRevision = other.Revision;
						if (other.Revision != -1)
						{
							int revision = thisRevision.CompareTo(otherRevision);
							if (revision != 0)
								return revision;
						}
					}
				}
			}

			return 0;
		}

		public static MethodReference MakeHostInstanceGeneric(this MethodReference @this, params TypeReference[] args)
		{
			MethodReference reference = new(@this.Name, @this.ReturnType, @this.DeclaringType.MakeGenericInstanceType(args))
			{
				HasThis = @this.HasThis,
				ExplicitThis = @this.ExplicitThis,
				CallingConvention = @this.CallingConvention
			};

			foreach (var parameter in @this.Parameters)
				reference.Parameters.Add(new ParameterDefinition(parameter.ParameterType));

			foreach (var parameter in @this.GenericParameters)
				reference.GenericParameters.Add(new GenericParameter(parameter.Name, reference));

			return reference;
		}

		// Allows enumerators in foreach loops
		public static IEnumerator<T> GetEnumerator<T>(this IEnumerator<T> @this)
		{
			return @this;
		}

		public static TypeDefinition GetTypeSafe(this ModuleDefinition @this, string fullName)
		{
			return @this.GetType(fullName) ?? throw new InvalidOperationException($"Type '{fullName}' not found ({@this.Assembly})");
		}

		public static MethodDefinition FindMethodSafe(this TypeDefinition @this, string id)
		{
			return @this.FindMethod(id) ?? throw new InvalidOperationException($"Method '{id}' not found in '{@this}' ({@this.Module.Assembly})");
		}

		public static PropertyDefinition FindPropertySafe(this TypeDefinition @this, string name)
		{
			return @this.FindProperty(name) ?? throw new InvalidOperationException($"Property '{name}' not found in '{@this}' ({@this.Module.Assembly})");
		}

		public static MethodDefinition GetGetMethodSafe(this PropertyDefinition @this)
		{
			return @this.GetMethod ??
			       throw new InvalidOperationException($"Property '{@this}' does not have a getter ({@this.Module.Assembly}");
		}
	}
}
