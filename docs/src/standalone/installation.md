---
title: Standalone Installation
---

## Installing

### Installing with .NET SDK

If you develop using .NET languages (C#/F#/VB.NET), you likely have the .NET SDK installed. Installing Mason is extremely trivial in these cases. Simply run

```bash
dotnet tool install --global Mason
```

to install Mason and allow it to be used in any folder (hence `global`).

### Installing without .NET SDK

If you do not have the .NET SDK, you can still install Mason, but it is a little more difficult.  

Visit [the releases page](https://github.com/H3VR-Modding/Mason/releases) and download the standalone zip that corresponds to your operating system. Extract this zip to a directory and add this directory to your PATH. To do this on Windows, see [this wikiHow article](https://www.wikihow.com/Change-the-PATH-Environment-Variable-on-Windows).

## Updating

### Updating with .NET SDK

Run

```bash
dotnet tool update --global Mason
```

### Updating without .NET SDK

Delete all files and folders from the directory you installed Mason to. Download the latest version of Mason, and extract that to the same directory.
