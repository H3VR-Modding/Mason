# Mason
Mason is a compiler that compiles project directories into [BepInEx](https://github.com/BepInEx/BepInEx) plugins that utilize [Stratum](https://github.com/H3VR-Modding/Stratum) asset loading.  
This is a tool intended to be used solely by creators, not users. Please do not include this as a dependency in your Thunderstore package.

## Installation
Mason can be installed as a standalone program or a BepInEx patcher plugin.  
The standalone version allows you to run Mason anywhere on your computer at any time, without any game installed. It also has an intelligent packaging mode, which packages only files and directories that will be used into a Thunderstore mod.  
The patcher plugin version runs Mason as the game is launching. The resultant plugin is loaded shortly after Mason, and then the game starts. This produces the least time between each test of your project.

If you have never used Mason before, try using it standalone before using it as a patcher plugin.

### Standalone
The easiest way to install the standalone version is to install from NuGet, but this only works if you have the .NET SDK installed:
```bash
dotnet tool install --global Mason
```

Most people don't though, so if you are like most people, go to [the releases page](https://github.com/H3VR-Modding/Mason/releases) and download a release. Extract this to a folder of your choosing, but you will need to add it to the PATH variable. [Here's a guide on how to do that](https://www.architectryan.com/2018/03/17/add-to-the-path-on-windows-10/).  
You will also need .NET 5 installed. You may already have this because other programs can require it, but in case you do not, go to [this download page](https://dotnet.microsoft.com/download/dotnet/5.0) and download the ".NET Desktop Runtime". If you do not know what the difference between Arm64/x64/x86 is, download x64.

### Patcher Plugin
The patcher plugin can be downloaded as [a Thunderstore package](https://h3vr.thunderstore.io/package/StratumTeam/Mason_Patcher/). Using [r2modman](https://github.com/ebkr/r2modmanPlus) is the recommended way to install these packages.

## Usage
### Standalone
To use the standalone version, simply follow the guide on the [the quickstart repository](https://github.com/H3VR-Modding/Mason-Quickstart).

### Patcher Plugin
To use the patcher plugin, first navigate to your r2modman profile directory (if you are not using r2modman, this would be your game directory). Enter the `BepInEx/plugins` folder, which will house your project folder. Then, follow the guide on [the quickstart repository](https://github.com/H3VR-Modding/Mason-Quickstart) with a few minor adjustments:
- When you clone/download the files, download them to a directory within `BepInEx/plugins` folder.
- Ignore the building step; the patcher plugin does the building for you.

For more experienced users, it is recommended that you clone the repository to a directory of your choosing, then use a symlink within the `BepInEx/plugins` directory. This keeps your git projects out of your r2modman profiles, but has the same functionality.

## Versioning
The version of this repository is based on the two consumable projects: standalone and patcher plugin. It does not include the public API of the core project, which may change at any time.
