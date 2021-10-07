# Asset Management

Assets are the most important element of nearly all Mason projects. A strong understanding of assets shows a fairly good understanding of Mason.

## Lexicon

Before we continue, some definitions should be established:

A **loader** is a plugin that features some code to convert data into some effect in game. An **asset** is one or more **resources**, which are files and folders used by an asset, that are passed to a loader at a certain time. A **stage** is a Stratum process that executes every plugin, thus every asset.  
For example, OtherLoader is a loader that turns asset bundles into spawnable guns in game. VeprHunter contains an asset bundle (which has the data of the gun), which then gets passed to OtherLoader. The process of passing the asset bundle to OtherLoader is the asset, but the asset bundle itself is a resource because it is used in an asset. This entire interaction is facilitated by the runtime stage, which runs every plugin's runtime callback (thus running every plugin's runtime assets) while the game is running.

While assets and resources are closely related, it is important to note the difference so the contents of a project can be clearly and concisely explained.

## The Asset Definition

The remaining chapters of this unit cover much about assets, but not do not define them. Instead, assets an asset definition will be shown and dissected in this very section.

Yet another term to learn! Asset definition is simply the text within a project file that represents an asset; it is the way an asset is defined. Assets have the structure of:

```yaml
path: gun_bundle
plugin: h3vr.otherloader
loader: item
```

but the properties can be reordered, of course.  
An asset definition can be broken into the following components:

- `path`: the path to the resource, relative to the resources folder
- `plugin`: the BepInEx GUID of the plugin that contains the loader
- `loader`: the name of the loader
