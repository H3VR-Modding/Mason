# Packaging

Now that the mod is complete, it can be packaged! Be sure to run Mason before you package to ensure the bootstrap file is fully up to date with your project file and resources, and finish up any Thunderstore-required files.

Packaging a Stratum plugin is a little more complicated than a Deli mod, but it's just as easy. Our current project directory is as follows:

```text
MyProject
 ├── resources
 │   └── my_gun
 ├── bootstrap.dll
 ├── config.yaml
 ├── icon.png
 ├── manifest.json
 ├── project.yaml
 └── README.md
```

The Thunderstore-required files must stay where they are, so we can add them like usual. Similarly, the bootstrap file is in the correct location for the package.  
The config and project files can be ignored, as they are only used to generate the bootstrap file. They may also be included in the package, but serve no meaningful purpose.  
This leaves only the resources folder. Because we require the resources folder to not be flattened, we must follow [r2modman's style](https://github.com/ebkr/r2modmanPlus/wiki/Structuring-your-Thunderstore-package) of packaging and place the resources folder within a `plugin` folder.

With all of these factors considered, the following should be what your mod structure looks like:

```text
MyMod
 ├── plugins
 │   └── resources
 │       └── my_gun
 ├── bootstrap.dll
 ├── icon.png
 ├── manifest.json
 └── README.md
```

Zip the contents of the folder and it's ready to upload!
