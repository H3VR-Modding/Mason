# Assets

Stratum plugins without assets, much like Thunderstore packages, are rarely useful. They're like a waterbottle without the water. They are also the most complex part of Stratum plugins.

This page is very YAML-heavy. If these files look alien to you, [this website](https://docs.ansible.com/ansible/latest/reference_appendices/YAMLSyntax.html) has a good breakdown of YAML syntax. It is very similar to JSON, just more terse and packs more features.

## What Is An Asset, Anyway?

As this is traversing into foreign territory, some definitions should be established:

A **loader** is a plugin that features some code to convert data into some effect in game. An **asset** is one or more **resources**, which are files and folders used by an asset, that are passed to a loader at a certain time.  
For example, OtherLoader is a loader that turns asset bundles into spawnable guns in game. VeprHunter contains an asset bundle (which has the data of the gun), which then gets passed to OtherLoader. The process of passing the asset bundle to OtherLoader is the asset. The asset bundle is a resource, because it is used in an asset.  

While assets and resources are closely related, it is important to note the difference so the contents of a project can be clearly and concisely explained.

## Adding Assets

Now that you have an understanding of assets, we can add one to our project.  

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
The runtime assets node takes a sequence of assets, which is show above after the hyphen. `path` is the path to the resource, relative to the resources folder. `plugin` is the BepInEx GUID of the plugin that contains the loader, and `loader` is the name of the loader. Together, these three data make a single asset.  

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

The most notable difference in this example is the `sequential` tag. By default, runtime assets are ran in parallel. `sequential: true` says otherwise, and loads them one at a time. If it was not specified, the late asset bundle may load before the data asset bundle!

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

When Mason reads assets, it doesn't actually add them vertabim to the assets sequence. Instead, it expands their paths to match all resources possible. In this example, `data/*` would match `data/first_gun` and `data/second_gun`. Mason then creates assets for each expanded path and adds them to the same asset sequence.  
This process means that the above project file creates an identical project file as:

```yaml
assets:
  runtime:
    sequential: true
    nested:
      - assets:
        - path: data/first_gun
          plugin: h3vr.otherloader
          loader: item_data
        - path: data/second_gun
          plugin: h3vr.otherloader
          loader: item_data
      - assets:
        - path: late/late_first_gun
          plugin: h3vr.otherloader
          loader: item_first_late
        - path: late/late_second_gun
          plugin: h3vr.otherloader
          loader: item_first_late
```

but is obviously much cleaner.

If we had not used pipelines, the data and late asset bundles would all be in the same asset sequence; the data bundles would be loaded one at a time, then the late bundles would be loaded one at a time. We don't care for them to be loaded one at a time, only that late bundles are loaded after data bundles. For this reason, we create two nested pipelines within the root pipeline and instruct the it to run sequentially.  
With these changes, the plugin first loads the data asset bundles in parallel. Once all data asset bundles have finished loading, it does the same to the late asset bundles.
