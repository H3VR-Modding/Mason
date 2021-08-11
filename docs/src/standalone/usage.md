---
title: Standalone Usage
---

## Prerequisites

- [Standalone Mason](installation.md): why else would you be here?
- [git](https://git-scm.com): clones the quickstart project
- [Visual Studio Code](https://code.visualstudio.com/): good text editor that provides autocompletion and error checking to JSON files
- [VSCode YAML extension by Red Hat](https://marketplace.visualstudio.com/items?itemName=redhat.vscode-yaml): provides autocompletion and error checking to YAML files

## Preparing the Project

Create a folder where you want to store your project. Enter the folder and right click on the empty space. Click `Git Bash Here`, then run the following command:

```bash
git clone --recurse-submodules https://github.com/H3VR-Modding/Mason-Quickstart . && code . && exit
```

This will download the quickstart repository and the DLL references Mason needs when running in standalone mode, open VSCode, then close the command line.

> [!IMPORTANT]
> If you download the repository from the GitHub website, it will not include the DLL references and Mason will error. You must clone the repository.

## Editing the Project

Editing works identically to a project compiled using the patcher plugin. Because you have VSCode, however, the `manifest.json` and `project.yaml` files have IntelliSense. This grants the files documentation when hovering over fields, autocompletion, and validation.

## Testing the Project

Testing can be done two ways.  
The simplest method is to package the project and import the resultant package into r2modman. This requires little technical knowledge but isn't fully automatic and takes some time.  
The automated but more esoteric option is to symlink the project folder to a folder within your `BepInEx/plugins`. Install Mason from Thunderstore, which will compile your project as the game launches.

> [!WARNING]
> If you choose to symlink your project, ensure there is no `bootstrap.dll` file in your `out` folder. BepInEx might load it, causing the the auto-generated one to be skipped.

## Packaging the Project

Click `Terminal > Run Build Task` at the top of VSCode (or press `CTRL + SHIFT + B`) to pack your mod. A zip file with the appropriate name should appear in the `out` folder
