# Converting Deli Mods

As Mason + Stratum is the replacement for Deli, creators should convert their Deli mods to Stratum plugins when they find it is possible. Conversion is performed automatically through Deliter (`/dɪˈliːtər/`), which converts all Deli mods possible within the current profile. Mods may be converted by hand, but Deliter is faster.

## Installing Deliter

Deliter can be be found on [Thunderstore](https://h3vr.thunderstore.io/package/Stratum/Deliter/) or [GitHub](https://github.com/H3VR-Modding/Deliter/releases/latest).

## Converting

Install all the mods you wish to convert, then launch the game. Deliter will convert all it can to Stratum plugins, even if it cannot convert the entire mod. Once the preloader finishes, it is done.  
The directory of a delited mod should look similar to:

```text
SomeMod
 ├── resources
 ├── AMod.deli.bak
 ├── bootstrap.dll
 ├── config.yaml
 ├── icon.png
 ├── manifest.json
 ├── project.yaml
 └── README.md
```

`AMod.deli.bak` is just a copy of the original mod file, in case the original mod is not available on Thunderstore for whatever reason.  
The original file, in this example `AMod.deli`, may actually still exist under some circumstances. This happens when a mod cannot be fully converted, but is instead partially or not at all converted. To check if the mod was converted at all, simply view if there are any assets in the project file. No assets means no conversion. In these scenarios, mods still require Deli.

## Repackaging

As Deliter simply generates a Mason project, the steps listed on [the packaging page of Getting Started](getting_started/packaging.md) can be followed with a few additions.  

The most important thing to do when repackaging a delited mod is: **do not include the `.bak` file**. This is the original Deli mod, which is already on Thunderstore within an older package version. It serves no functional purpose and can be omitted.

Partial conversions also need to be cared for more than complete conversions. You *can* package the partially converted Deli mod with no edits, but that could as much as double the size of the package.  
If possible, skim through the manifest file of the Deli mod and check what files/folders are still used. Deliter is not smart enough to delete them, so you must do this manually.  
The same goes for the resources folder; using the project file as a guide, trim any excess resources. This will result in a smaller Thunderstore package. For more advanced users, [the standalone section](standalone/README.md) details automatic packaging which will do this process for you.
