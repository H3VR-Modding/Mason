using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx;
using Mason.Core.Globbing;
using Mason.Core.IR;
using Mason.Core.Markup;
using Mason.Core.Projects;
using Mason.Core.Projects.v1;
using Mason.Core.Thunderstore;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Asset = Mason.Core.IR.Asset;
using AssetPipeline = Mason.Core.IR.AssetPipeline;
using Assets = Mason.Core.IR.Assets;
using Version = System.Version;

namespace Mason.Core.Parsing.Projects.v1
{
	internal class V1ProjectParser : IProjectParser
	{
		private readonly IDeserializer _deserializer;
		private readonly GlobberFactory _globbers = new();

		public V1ProjectParser()
		{
			Box<IDeserializer> box = new();

			box.Value = _deserializer = new DeserializerBuilder()
				.WithNamingConvention(UnderscoredNamingConvention.Instance)
				.WithTypeConverter(new MarkedTypeConverter(box))
				.WithTypeConverter(new GuidStringTypeConverter())
				.Build();
		}

		public ParserOutput Parse(UnparsedProject project)
		{
			return new Scoped(this, project).Parse();
		}

		private class Scoped
		{
			private readonly IDeserializer _deserializer;
			private readonly GlobberFactory _globbers;
			private readonly UnparsedProject _project;
			private readonly HashSet<string> _referencedPaths;
			private readonly List<MarkupMessage> _warnings;

			public Scoped(V1ProjectParser global, UnparsedProject project)
			{
				(_deserializer, _globbers) = (global._deserializer, global._globbers);
				_project = project;
				_warnings = new List<MarkupMessage>();
				_referencedPaths = new HashSet<string>();
			}

			public ParserOutput Parse()
			{
				Metadata meta = ManifestToIR();

				Project yaml;
				try
				{
					yaml = _deserializer.Deserialize<Project>(_project.Parser);
				}
				catch (YamlException e)
				{
					throw new CompilerException(MarkupMessage.File(_project.Path, e.GetRange(), e.Message), meta.Package);
				}

				meta.Dependencies = DependenciesToIR(yaml.Dependencies);
				meta.Incompatibilities = IncompatibilitiesToIR(yaml.Incompatibilities);
				meta.Processes = ProcessesToIR(yaml.Processes);

				Mod mod = new(meta)
				{
					Assets = AssetsToIR(yaml.Assets)
				};

				return new ParserOutput(mod, _warnings, _referencedPaths);
			}

			private PackageComponentString GetAuthor()
			{
				// Full path because we might be given a relative path (e.g. ".", "../") which can't be split
				string full = Path.GetFullPath(_project.Directory);
				string[] split = Path.GetFileName(full).Split('-');
				if (split.Length != 2)
					throw new CompilerException(MarkupMessage.Path(_project.ManifestPath,
						"The author property must be present, or the directory must be named [author]-[name]."));

				Marked<PackageComponentString> name = _project.Manifest.Name;
				if (split[1] != name.Value)
					throw new CompilerException(MarkupMessage.Path(_project.ManifestPath,
						$"The author property must be present, or the directory must be named [author]-[name]. Perhaps you meant to name the directory '{split[0]}-{name}'?"));

				if (PackageComponentString.TryParse(split[0]) is not { } author)
					throw new CompilerException(MarkupMessage.Path(_project.Directory,
						"Author (inferred by directory) may only contain the characters a-z A-Z 0-9 _ and cannot start or end with _"));

				_warnings.Add(MarkupMessage.Path(_project.ManifestPath,
					"The author of the mod was infered by the directory name. Consider adding an 'author' property."));

				return author;
			}

			private Metadata ManifestToIR()
			{
				Manifest manifest = _project.Manifest;

				PackageComponentString author = manifest.Author?.Value ?? GetAuthor();
				PackageComponentString name = manifest.Name.Value;
				SimpleSemVersion version = manifest.VersionNumber;

				BepInPlugin plugin = new(author + "-" + name, name, version.ToString());
				PackageReference package = new(author, name, version);


				return new Metadata(plugin, package);
			}

			private IList<BepInDependency>? DependenciesToIR(Dependencies? dependencies)
			{
				if (dependencies == null)
					return null;

				List<BepInDependency> total = new();
				HashSet<GuidString> used = new();

				foreach ((GuidString guid, Marked<Version> version) in dependencies.Hard.OrEmptyIfNull())
				{
					if (guid == Compiler.StratumGUID)
					{
						Version? depVersion = version.Value;
						switch (depVersion.GoodCompareTo(Compiler.MinimumStratumVersion))
						{
							case 1:
								_warnings.Add(MarkupMessage.File(_project.Path, version.Range,
									$"Stratum dependency with a minimum version ({depVersion}) greater than required ({Compiler.MinimumStratumVersion})"));
								break;
							case 0:
								_warnings.Add(MarkupMessage.File(_project.Path, version.Range, "Redundant Stratum dependency"));
								break;
							case -1:
								throw new CompilerException(MarkupMessage.File(_project.Path, version.Range,
									$"Stratum dependency with a minimum version ({depVersion}) less than required ({Compiler.MinimumStratumVersion})"));
							default:
								throw new ArgumentOutOfRangeException();
						}
					}

					used.Add(guid);
					total.Add(new BepInDependency(guid, version.Value.ToString()));
				}

				used.Add(Compiler.StratumGUID);
				total.Add(new BepInDependency(Compiler.StratumGUID, Compiler.MinimumStratumVersion.ToString()));

				foreach (Marked<GuidString> guid in dependencies.Soft.OrEmptyIfNull())
				{
					GuidString value = guid.Value;
					if (used.Contains(value))
						throw new CompilerException(MarkupMessage.File(_project.Path, guid.Range,
							"Soft dependency is already a hard dependency"));

					total.Add(new BepInDependency(value, BepInDependency.DependencyFlags.SoftDependency));
				}

				return total;
			}

			private IList<BepInIncompatibility>? IncompatibilitiesToIR(List<GuidString>? incompatibilities)
			{
				return incompatibilities?.ConvertAll(guid => new BepInIncompatibility(guid));
			}

			private IList<BepInProcess>? ProcessesToIR(List<string>? processes)
			{
				return processes?.ConvertAll(process => new BepInProcess(process));
			}

			private IEnumerable<Asset> AssetToIR(Core.Projects.v1.Asset asset, string resources)
			{
				Marked<string> path = asset.Path;
				using IEnumerator<string> enumerator = _globbers.Glob(resources, path.Value).GetEnumerator();

				if (!enumerator.MoveNext())
				{
					_warnings.Add(MarkupMessage.File(_project.Path, path.Range, "Path matched no handles"));
					yield break;
				}

				Asset ToAsset(string globbed)
				{
					string rel = Tools.RelativePath(resources, globbed) ??
					             throw new Exception("Could not find relative path to globbed result.");
					_referencedPaths.Add(rel);

					return new Asset(rel, asset.Plugin, asset.Loader);
				}

				yield return ToAsset(enumerator.Current!);

				foreach (string globbed in enumerator)
					yield return ToAsset(globbed);
			}

			private IList<Asset> AssetsToIR(IEnumerable<Core.Projects.v1.Asset> assets, string resources)
			{
				return assets
					.SelectMany(x => AssetToIR(x, resources))
					.ToList();
			}

			private AssetPipeline AssetPipelineToIR(Core.Projects.v1.AssetPipeline pipeline, string resources)
			{
				AssetPipeline ret = new()
				{
					Sequential = pipeline.Sequential,
					Name = pipeline.Name
				};

				Core.Projects.v1.Asset[]? assets = pipeline.Assets;
				if (assets != null)
					ret.Assets = AssetsToIR(assets, resources);

				Core.Projects.v1.AssetPipeline[]? nested = pipeline.Nested;
				if (nested != null)
					ret.Nested = Array.ConvertAll(nested, x => AssetPipelineToIR(x, resources));

				return ret;
			}

			private Assets? AssetsToIR(Core.Projects.v1.Assets? source)
			{
				if (source == null)
					return null;

				string resources = Path.Combine(_project.Directory, Compiler.ResourcesDirectory);

				if (!Directory.Exists(resources))
					throw new CompilerException(MarkupMessage.Path(resources, "No resources found"));

				Assets assets = new();

				{
					Core.Projects.v1.Asset[]? setup = source.Setup;
					if (setup != null)
						assets.Setup = AssetsToIR(setup, resources);
				}

				{
					Core.Projects.v1.AssetPipeline? runtime = source.Runtime;
					if (runtime != null)
						assets.Runtime = AssetPipelineToIR(runtime, resources);
				}

				return assets;
			}
		}
	}
}
