using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Mason.Core.Globbing
{
	/// <summary>
	///     Creates <see cref="Globber" />s, and can be configured to allow/disallow certain globs
	/// </summary>
	internal class GlobberFactory
	{
		/// <summary>
		///     Custom behaviours that should be applied, given that a handle's name matches exactly
		/// </summary>
		public Dictionary<string, SpecialNameEntry> SpecialNames { get; } = new()
		{
			["**"] = SpecialNameEntry.Allowed(StatelessGlobbers.Globstar)
		};

		/// <summary>
		///     Regular expressions to be applied, given that part of a handle's name matches the filter
		/// </summary>
		public List<NameReplacementEntry> NameReplacements { get; } = new()
		{
			NameReplacementEntry.Allowed(new Regex(@"(?<!\\)\[!(.+)-(.+)(?<!\\)\]"), @"[!{0}-{1}]"),
			NameReplacementEntry.Allowed(new Regex(@"(?<!\\)\[(.+)-(.+)(?<!\\)\]"), @"[{0}-{1}]"),
			NameReplacementEntry.Allowed(new Regex(@"(?<!\\)\[\!(.+)(?<!\\)\]"), @"[!{0}]"),
			NameReplacementEntry.Allowed(new Regex(@"(?<!\\)\[(.+)(?<!\\)\]"), @"[{0}]"),
			NameReplacementEntry.Allowed(new Regex(@"(?<!\\)\*"), @".*"),
			NameReplacementEntry.Allowed(new Regex(@"(?<!\\)\?"), @".")
		};

		private Globber? FromSegment(string name)
		{
			if (string.Empty == name.Trim())
				throw new ArgumentException("Path segment was whitespace", nameof(name));

			if (!SpecialNames.TryGetValue(name, out SpecialNameEntry type))
				return new NameGlobber(name, NameReplacements).Globber;

			if (!type.IsAllowed)
				throw new ArgumentException($"'{name}' globbing is disabled", nameof(name));

			return type.Globber;
		}

		/// <summary>
		///     Creates a <see cref="Globber" /> from a path. May return <see langword="null" />, in which case it should be treated as a no-op
		///     (returns the directory that it is passed).
		/// </summary>
		/// <param name="path">A glob-enabled path, which may contain path separators</param>
		public Globber? Create(string path)
		{
			if (path.Length == 0)
				return null;

			string[] split = path.Split('/');
			int count = split.Length;
			List<Globber> globbers = new(count);

			int countOneOff = count - 1;
			for (var i = 0; i < countOneOff; ++i)
			{
				Globber? globber = FromSegment(split[i]);

				if (globber != null)
					globbers.Add(globber);
			}

			{
				Globber? globber = split[countOneOff] switch
				{
					"" => StatelessGlobbers.Current,
					{ } name => FromSegment(name)
				};

				if (globber != null)
					globbers.Add(globber);
			}

			return globbers.Count switch
			{
				0 => null,
				1 => globbers[0],
				_ => new CompositeGlobber(globbers).Globber
			};
		}

		/// <summary>
		///     Enumerates over all the handles that match the path.
		///     If you are calling multiple times with the same path, use <seealso cref="Create" /> to create a reusable, efficient glob method.
		/// </summary>
		/// <param name="directory">The directory to apply the path to</param>
		/// <param name="path">The path which may contain globs and path separators</param>
		public IEnumerable<string> Glob(string directory, string path)
		{
			Globber? glob = Create(path);
			if (glob == null)
			{
				yield return directory;
				yield break;
			}

			foreach (string match in glob(directory))
				yield return match;
		}

		/// <summary>
		///     A glob that may or may not be allowed and applied, given that a handle's name matches exactly
		/// </summary>
		public readonly struct SpecialNameEntry
		{
			/// <summary>
			///     A <see cref="SpecialNameEntry" /> that is not allowed to be used. Use of this entry will cause an exception.
			/// </summary>
			public static SpecialNameEntry Disallowed { get; } = new(false, null);

			/// <summary>
			///     Creates a <see cref="SpecialNameEntry" /> that is allowed to be used
			/// </summary>
			/// <param name="globber">The glob to apply, if the handle matches. If <see langword="null" />, the current directory is yielded.</param>
			public static SpecialNameEntry Allowed(Globber? globber)
			{
				return new(true, globber);
			}

			/// <summary>
			///     Whether or not this entry is allowed. If it is not, then paths utilizing this will throw an exception.
			/// </summary>
			public bool IsAllowed { get; }

			/// <summary>
			///     The glob to apply, if the handle matches. If <see langword="null" />, the current directory is yielded.
			/// </summary>
			public Globber? Globber { get; }

			private SpecialNameEntry(bool isAllowed, Globber? globber)
			{
				IsAllowed = isAllowed;
				Globber = globber;
			}
		}

		/// <summary>
		///     A regular expression that may be allowed and applied, given that part of a handle's name matches the filter
		/// </summary>
		public readonly struct NameReplacementEntry
		{
			/// <summary>
			///     A <see cref="SpecialNameEntry" /> that is not allowed to be used. Use of this entry will cause an exception.
			/// </summary>
			/// <param name="filter">The filter to compare handle names to</param>
			public static NameReplacementEntry Disallowed(Regex filter)
			{
				return new(filter, null);
			}

			/// <summary>
			///     Creates a <see cref="SpecialNameEntry" /> that is allowed to be used
			/// </summary>
			/// <param name="filter">The filter to compare handle names to</param>
			/// <param name="replacement">
			///     The template to replace what is matched in the filter. This can be in
			///     <see cref="string.Format(string, object[])" /> format, where regex capturing groups' values are supplied
			/// </param>
			public static NameReplacementEntry Allowed(Regex filter, string replacement)
			{
				return new(filter, replacement);
			}

			/// <summary>
			///     Whether or not this entry is allowed. If it is not, then paths utilizing this will throw an exception.
			/// </summary>
			public bool IsAllowed => Replacement != null;

			/// <summary>
			///     The filter to compare handle names to
			/// </summary>
			public Regex Filter { get; }

			/// <summary>
			///     The template to replace what is matched in the filter. This can be in <see cref="string.Format(string, object[])" /> format, where
			///     regex capturing groups' values are supplied
			/// </summary>
			public string? Replacement { get; }

			private NameReplacementEntry(Regex filter, string? replacement)
			{
				Filter = filter;
				Replacement = replacement;
			}

			/// <summary>
			///     Checks if this entry is allowed, and populates the replacement if so
			/// </summary>
			/// <param name="replacement"></param>
			/// <returns></returns>
			public bool MatchAllowed([NotNullWhen(true)] out string? replacement)
			{
				replacement = Replacement;
				return IsAllowed;
			}
		}
	}
}
