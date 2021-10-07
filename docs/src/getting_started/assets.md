# Assets

Stratum plugins without assets, much like Thunderstore packages, are rarely useful. They're like a waterbottle without the water. They are also the most complex part of Stratum plugins.

This page is very YAML-heavy. If these files look alien to you, [this website](https://docs.ansible.com/ansible/latest/reference_appendices/YAMLSyntax.html) has a good breakdown of YAML syntax. It is very similar to JSON, just more terse and packs more features.

## Adding Assets  

First, create a `resources` folder within your project. This is where all your resources will reside.

### One Asset

Assuming we have a gun within the asset bundle `my_gun` that we would like OtherLoader to load instantly, simply place the asset bundle in the resources folder. Using this simple `project.yaml` would cause it to load:

```yaml
assets:
  runtime:
    assets:
      - path: my_gun
        plugin: h3vr.otherloader
        loader: item
```

In this example, the first `assets` node declares the assets of the entire plugin. The second `assets` node narrows it down to just the assets of the runtime stage.  
The runtime assets node takes a sequence of assets, which is show above after the hyphen. `path` is the path to the file, relative to the resources folder. `plugin` is the BepInEx GUID of the plugin that contains the loader, and `loader` is the name of the loader. Together, these three data make a single asset.  

### Multiple Assets

Say you are using OtherLoader's new on-demand loading feature. This requires two assets: one for the data asset bundle, and one for the late asset bundle. This example will use `my_gun` and `late_my_gun` correspondingly:

```yaml
assets:
  runtime:
    sequential: true
    assets:
    - path: my_gun
      plugin: h3vr.otherloader
      loader: item_data
    - path: late_my_gun
      plugin: h3vr.otherloader
      loader: item_first_late
```

The most notable difference in this example is the `sequential` tag. By default, runtime assets are ran in parallel. `sequential: true` specifies that it should load in series, causing them to be loaded one at a time. If it was not specified, the late asset bundle may load before the data asset bundle!

### Multiple Assets in Distinct Stages

Again with OtherLoader's new on-demand loading feature: if you have multiple asset bundles for the data or late loader, you must use nested pipelines. A pipeline is just a collection of subpipelines and assets to run. In fact, the `runtime` node is actually a pipeline.  
The necessary project file looks slightly different, but reads approximately the same:

```yaml
assets:
  runtime:
    sequential: true
    nested:
    - assets:
      - path: data/*
        plugin: h3vr.otherloader
        loader: item_data
    - assets:
      - path: late/*
        plugin: h3vr.otherloader
        loader: item_first_late
```

This project file is intended for use with such a resources folder as:

```text
resources
 ├── data
 │   ├── first_gun
 │   └── second_gun
 └── late
     ├── late_first_gun
     └── late_second_gun
```

This is also the most detailed example, so it may be helpful to see it in the context of the rest of the project file. The following example is a complete project file, contrary to the warning at the beginning of the book:

```yaml
version: 1
dependencies:
  hard:
    h3vr.otherloader: 1.0.0
assets:
  runtime:
    sequential: true
    nested:
    - assets:
      - path: data/*
        plugin: h3vr.otherloader
        loader: item_data
    - assets:
      - path: late/*
        plugin: h3vr.otherloader
        loader: item_first_late
```

---

For further explanation of assets, see [the assets unit](../assets/index.md). This page specifically discusses [the runtime stage](../assets/runtime/index.md)
