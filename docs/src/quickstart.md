---
title: Quickstart
---

## Installation

Mason can be installed as [a Thunderstore package](https://h3vr.thunderstore.io/package/Stratum/Mason). Additionally, consider making a new r2modman profile for your package to accelerate launch times and isolate errors from other mods.

## Setting Up the Project

Download [the quickstart repository](https://github.com/H3VR-Modding/Mason-Quickstart). Open this zip, then open your r2modman profile folder and navigate to `BepInEx/plugins`. Drag and drop the `Mason-Quickstart-main` folder into the plugins folder you just opened. Rename the folder to something memorable; this name won't be used by Mason.

## Editing the Project

There are 2 files that Mason uses to generate your plugin: `manifest.json` and `project.yaml`.

### `manifest.json`

`manifest.json` is a standard Thunderstore manifest with an `author` field. This is otherwise known as an r2modman-flavored manifest. This is the same file that will be used for your Thunderstore package.  
Mason uses it to generate some of your plugin's metadata:

- `author` and `name` are used to generate your plugin's GUID: `[author]-[name]`.  
- `name` is also your plugin's name.  
- `version_number` is your plugin's version.

#### Example Manifest

[!code-json[manifest.json](files/examples/manifest.json)]

produces

```text
GUID: nayr31-VeprHunter
Name: VeprHunter
Version: 1.1.0
```

### `project.yaml`

`project.yaml`, also known as the project file, is a Mason-specific file. It contains all the information that Mason cannot infer from other sources.  
It must begin with `version` field, which denotes what `project.yaml` format to use. Currently, only version 1 is available.  
There is no other mandatory data, but you probably want the `assets` field anyway. It describes how your resources should be loaded, thus becoming assets.

#### Example Project File

[!code-yaml[project.yaml](files/examples/project.yaml)]

produces a plugin that will load the `my_character` folder into TnhTweaker's `character` loader, and the `my_item` into OtherLoader's `item` loader.

## Testing the Project

Simply launch the game. Mason will compile the project to a BepInEx plugin, which will then load itself into Stratum. Now you have a Stratum plugin!

## Packaging the Project

> [!IMPORTANT]
> Remember to launch the game before packaging. Otherwise, you may package an out-of-date plugin.

To convert your project to a Thunderstore package, simply zip the following:

- `manifest.json` (Thunderstore required)
- `README.md` (Thunderstore required)
- `icon.png` (Thunderstore required)
- `bootstrap.dll` (loads your assets)
- `resources` (contains your assets' files)

If you do this regularly, consider using Mason standalone. It can pack all the files above, and only resources that are actually used, automatically.
