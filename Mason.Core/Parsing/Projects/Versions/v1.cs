using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx;
using Mason.Core.Globbing;
using Mason.Core.IR;
using Mason.Core.Markup;
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
				.WithTypeConverter(new VersionTypeConverter())
				.Build();
		}

		public Mod? Parse(Manifest manifest, string manifestFile, IParser project, string projectFile, string directory,
			CompilerOutput output)
		{
			Metadata meta;
			{
				string? author = manifest.Author?.Value;
				if (author == null)
				{
					string full = Path.GetFullPath(directory); // We might be given a relative path (e.g. ".", "../") which can't be split
					string[] split = Path.GetFileName(full).Split('-');
					if (split.Length != 2)
					{
						output.Failure(MarkupMessage.Path(manifestFile,
							"The author property must be present, or the directory must be named [author]-[name]."));
						return null;
					}

					Marked<string> name = manifest.Name;
					if (split[1] != name.Value)
					{
						output.Failure(MarkupMessage.Path(manifestFile,
							$"The author property must be present, or the directory must be named [author]-[name]. Perhaps you meant to name the directory '{split[0]}-{name}'?"));
						return null;
					}

					author = split[0];
					if (!Compiler.ComponentRegex.IsMatch(author))
					{
						output.Failure(MarkupMessage.Path(directory, "Author (inferred by directory) may only contain the characters a-z A-Z 0-9 _ and cannot start or end with _"));
						return null;
					}

					output.Warnings.Add(MarkupMessage.Path(manifestFile,
						"The author of the mod was infered by the directory name. Consider adding an 'author' property."));
				}

				{
					string name = manifest.Name.Value;
					Version version = manifest.VersionNumber;
					string guid = author + "-" + name;
					meta = new Metadata(new BepInPlugin(guid, name, version.ToString()), new PackageReference(author, name, version));
				}
			}

			Template template;
			try
			{
				template = _deserializer.Deserialize<Template>(project);
			}
			catch (YamlException e)
			{
				output.Failure(MarkupMessage.File(projectFile, e.GetRange(), e.Message));
				return null;
			}

			Dependencies? dependencies = template.Dependencies;
			if (dependencies != null)
				meta.Dependencies = ToIR(dependencies);

			return new Mod(meta)
			{
				Assets = ToIR(template.Assets, Path.Combine(directory, "resources"), output)
			};
		}

		private IList<Marked<BepInDependency>> ToIR(Dependencies dependencies)
		{
			List<Marked<BepInDependency>> total = new();

			{
				Dictionary<string, Marked<Version>>? hard = dependencies.Hard;
				if (hard != null)
					foreach ((string guid, Marked<Version> version) in hard)
						total.Add(new Marked<BepInDependency>(new BepInDependency(guid, version.Value.ToString()), version.Range));
			}

			{
				Marked<string>[]? soft = dependencies.Soft;
				if (soft != null)
					foreach (Marked<string> guid in soft)
						total.Add(new Marked<BepInDependency>(
							new BepInDependency(guid.Value, BepInDependency.DependencyFlags.SoftDependency), guid.Range));
			}

			return total;
		}

		private IEnumerable<Asset> ToIR(Core.Projects.v1.Asset asset, string root, CompilerOutput output)
		{
			return _globbers
				.Glob(root, asset.Path)
				.Select(x =>
				{
					string rel = Tools.RelativePath(root, x) ?? throw new Exception("Could not find relative path to globbed result.");
					output.ReferencedPaths.Add(rel);

					return new Asset(rel, asset.Plugin, asset.Loader);
				});
		}

		private IList<Asset> ToIR(IEnumerable<Core.Projects.v1.Asset> assets, string root, CompilerOutput output)
		{
			return assets
				.SelectMany(x => ToIR(x, root, output))
				.ToList();
		}

		private AssetPipeline ToIR(Core.Projects.v1.AssetPipeline pipeline, string root, CompilerOutput output)
		{
			AssetPipeline ret = new()
			{
				Sequential = pipeline.Sequential,
				Name = pipeline.Name
			};

			Core.Projects.v1.Asset[]? assets = pipeline.Assets;
			if (assets != null)
				ret.Assets = ToIR(assets, root, output);

			Core.Projects.v1.AssetPipeline[]? nested = pipeline.Nested;
			if (nested != null)
				ret.Nested = Array.ConvertAll(nested, x => ToIR(x, root, output));

			return ret;
		}

		private Assets ToIR(Core.Projects.v1.Assets source, string root, CompilerOutput output)
		{
			Assets assets = new();

			{
				Core.Projects.v1.Asset[]? setup = source.Setup;
				if (setup != null)
					assets.Setup = ToIR(setup, root, output);
			}

			{
				Core.Projects.v1.AssetPipeline? runtime = source.Runtime;
				if (runtime != null)
					assets.Runtime = ToIR(runtime, root, output);
			}

			return assets;
		}
	}
}
