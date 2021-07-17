using System.Collections.Generic;
using System.Linq;
using BepInEx;
using Mason.Core.Markup;
using Mono.Cecil;
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
			return @this.Where(x => x is not null)!;
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

		public static MarkupRange GetRange(this YamlException @this)
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

		public static bool HasFlag(this BepInDependency.DependencyFlags @this, BepInDependency.DependencyFlags value)
		{
			return (@this & value) == value;
		}
	}
}
