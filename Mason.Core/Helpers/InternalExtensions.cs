using System.Collections.Generic;
using System.Linq;
using BepInEx;
using Mason.Core.Markup;
using Mono.Cecil;
using MonoMod.Utils;
using Newtonsoft.Json;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace Mason.Core
{
	internal static class InternalExtensions
	{
		public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> @this, out TKey key, out TValue value)
		{
			key = @this.Key;
			value = @this.Value;
		}

		public static MethodDefinition GetNativeConstructor(this TypeDefinition @this) => @this.FindMethod("System.Void .ctor(System.Object,System.IntPtr)");

		public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> @this) => @this.Where(x => x is not null)!;

		public static IList<T>? Optimize<T>(this IList<T> @this) => @this.Count == 0 ? null : @this;

		public static IEnumerable<T> OrEmptyIfNull<T>(this IEnumerable<T>? @this) => @this ?? Enumerable.Empty<T>();

		public static MarkupIndex GetIndex(this IJsonLineInfo @this) => new(@this.LineNumber, @this.LinePosition);

		public static MarkupIndex GetIndex(this Mark @this) => new(@this.Line, @this.Column);

		public static MarkupIndex GetIndex(this JsonSerializationException @this) => new(@this.LineNumber, @this.LinePosition);

		public static MarkupRange GetRange(this ParsingEvent @this) => new(@this.Start.GetIndex(), @this.End.GetIndex());

		public static MarkupRange GetRange(this YamlException @this) => new(@this.Start.GetIndex(), @this.End.GetIndex());

		public static int GoodCompareTo(this System.Version @this, System.Version other)
		{
			var major = @this.Major.CompareTo(other.Major);
			if (major != 0)
				return major;

			var minor = @this.Minor.CompareTo(other.Minor);
			if (minor != 0)
				return minor;

			// It do be performant doe
			var thisBuild = @this.Build;
			if (thisBuild != -1)
			{
				var otherBuild = other.Build;
				if (otherBuild != -1)
				{
					var build = thisBuild.CompareTo(otherBuild);
					if (build != 0)
						return build;

					var thisRevision = @this.Revision;
					if (thisRevision != -1)
					{
						var otherRevision = other.Revision;
						if (other.Revision != -1)
						{
							var revision = thisRevision.CompareTo(otherRevision);
							if (revision != 0)
								return revision;
						}
					}
				}
			}

			return 0;
		}

		public static bool HasFlag(this BepInDependency.DependencyFlags @this, BepInDependency.DependencyFlags value) =>
			(@this & value) == value;
	}
}
