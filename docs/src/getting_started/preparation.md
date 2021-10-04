# Preparation

To prepare a Mason project, first navigate to where your mods are being installed. For r2modman or Thunderstore Mod Manager users, this can be found in `Settings > Locations > Browse profile folder`.  
Once you have arrived at the folder, navigate to `BepInEx/plugins` (you may have to run the game once for this folder to generate). Create a new directory, which will be the root of your Mason project.

## Populating the Project

Mason projects require some files before they can be compiled. Each of the following subsections describe paths within your Mason project.

### Project File

The project file contains the instructions that Mason should convert into runnable code. The project file is located at `project.yaml`, and must contain at least:

```yaml
version: 1
```

Wow! What an amazing project! In future examples, this will be omitted. However, **it is critical that you include the version field in every project file**. Failure to do so will result in the project not compiling.

### Thunderstore Manifest

The Thunderstore manifest contains the metadata that Thunderstore requires in every package. To avoid redundancy, Mason reads from this manifest to create metadata for your plugin. The following fields are the **bare minimum** to compile a Mason project, and are **not representative of your final manifest**:

```json
{
  "author": "AuthorName",
  "name": "ModName",
  "version_number": "1.0.0",
  "description": "This does something awesome, I only have 250 characters to explain!"
}
```

The author field is not part of the Thunderstore specification, however it is greatly recommended to include it. If not present, Mason will perform the following checks:

1. Does the project directory name have the pattern `AuthorName-ModName`?
2. Does the `ModName` of the directory match the `ModName` of the manifest?

If the checks pass, it will use the `AuthorName` from the project directory.
