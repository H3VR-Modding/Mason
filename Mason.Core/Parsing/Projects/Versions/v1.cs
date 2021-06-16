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
using Asset = Mason.Core.Projects.v1.Asset;
using AssetPipeline = Mason.Core.Projects.v1.AssetPipeline;
using Assets = Mason.Core.Projects.v1.Assets;

namespace Mason.Core.Parsing.Projects.v1
{
	internal class V1ProjectParser : IProjectParser
	{
		private readonly GlobberFactory _globbers = new();
		private readonly IDeserializer _deserializer;

		public V1ProjectParser()
		{
			var box = new Box<IDeserializer>();

			box.Value = _deserializer = new DeserializerBuilder()
				.WithNamingConvention(UnderscoredNamingConvention.Instance)
				.WithTypeConverter(new MarkedTypeConverter(box))
				.WithTypeConverter(new VersionTypeConverter())
				.Build();
		}

		private IList<Marked<BepInDependency>> ToIR(Dependencies dependencies)
		{
			var total = new List<Marked<BepInDependency>>();

			{
				var hard = dependencies.Hard;
				if (hard is not null)
					foreach (var (guid, version) in hard)
						total.Add(new(new(guid, version.Value.ToString()), version.Range));
			}

			{
				var soft = dependencies.Soft;
				if (soft is not null)
					foreach (var guid in soft)
						total.Add(new(new BepInDependency(guid.Value, BepInDependency.DependencyFlags.SoftDependency), guid.Range));
			}

			return total;
		}

		private IEnumerable<IR.Asset> ToIR(Core.Projects.v1.Asset asset, string root, CompilerOutput output) => _globbers
			.Glob(root, asset.Path)
			.Select(x =>
			{
				var rel = Tools.RelativePath(root, x) ?? throw new Exception("Could not find relative path to globbed result.");
				output.ReferencedPaths.Add(rel);

				return new IR.Asset(rel, asset.Plugin, asset.Loader);
			});

		private IList<IR.Asset> ToIR(IEnumerable<Core.Projects.v1.Asset> assets, string root, CompilerOutput output) => assets
			.SelectMany(x => ToIR(x, root, output))
			.ToList();

		private IR.AssetPipeline ToIR(Core.Projects.v1.AssetPipeline pipeline, string root, CompilerOutput output)
		{
			var ret = new IR.AssetPipeline
			{
				Sequential = pipeline.Sequential,
				Name = pipeline.Name
			};

			var assets = pipeline.Assets;
			if (assets is not null)
				ret.Assets = ToIR(assets, root, output);

			var nested = pipeline.Nested;
			if (nested is not null)
				ret.Nested = Array.ConvertAll(nested, x => ToIR(x, root, output));

			return ret;
		}

		private IR.Assets ToIR(Core.Projects.v1.Assets source, string root, CompilerOutput output)
		{
			IR.Assets assets = new();

			{
				var setup = source.Setup;
				if (setup is not null)
					assets.Setup = ToIR(setup, root, output);
			}

			{
				var runtime = source.Runtime;
				if (runtime is not null)
					assets.Runtime = ToIR(runtime, root, output);
			}

			return assets;
		}

		public Mod? Parse(Manifest manifest, string manifestFile, IParser project, string projectFile, string directory, CompilerOutput output)
		{
			Metadata meta;
			{
				var author = manifest.Author;
				if (author is null)
				{
					var full = Path.GetFullPath(directory); // We might be given a relative path (e.g. ".", "../") which can't be split
					var split = Path.GetFileName(full).Split('-');
					if (split.Length != 2)
					{
						output.Failure(MarkupMessage.Path(manifestFile, "The author property must be present, or the directory must be named [author]-[name]."));
						return null;
					}

					var name = manifest.Name;
					if (split[1] != name.Value)
					{
						output.Failure(MarkupMessage.Path(manifestFile, $"The author property must be present, or the directory must be named [author]-[name]. Perhaps you meant to name the directory '{split[0]}-{name}'?"));
						return null;
					}

					author = split[0];
					output.Warnings.Add(MarkupMessage.Path(manifestFile, "The author of the mod was infered by the directory name. Consider adding an 'author' property."));
				}

				{
					var name = manifest.Name.Value;
					var version = manifest.VersionNumber;
					var guid = author + "-" + name;
					meta = new Metadata(new(guid, name, version.ToString()), new(author, name, version));
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

			var dependencies = template.Dependencies;
			if (dependencies is not null)
				meta.Dependencies = ToIR(dependencies);

			return new(meta)
			{
				Assets = ToIR(template.Assets, Path.Combine(directory, "resources"), output)
			};
		}
	}
}
