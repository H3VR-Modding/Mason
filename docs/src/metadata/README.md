# Metadata

Mason makes available the rich metadata options of BepInEx plugins. Currently, the metadata available are:

- [**Dependencies**](dependencies.md) upon other plugins
- [**Incompatibilities**](incompatibilities.md) with other plugins
- [**Process**](processes.md) name restrictions

## Finding the GUID

Metadata that reference other plugins, such as dependencies and incompatibilities, require the GUID of other plugins. If you are not directly told the GUID of a plugin, how are you supposed to find it?

Plugins compiled using Mason always have a GUID with the format of `Author-Name`. This is *almost always* identical to the dependency string on Thunderstore, but without the version. For example, `nayr31-VeprHunter-1.0.0` would be `nayr31-VeprHunter`. The only instances where this is not the case is when the team name (uploader) of the package is not the same as the `author` field of the project's manifest, and is usually caused by reuploads and project forks.  
Consider a project, `MedicBag`, which is originally created by Dallas. Dallas uses `Dallas` as his Thunderstore team name and `author` field. The Thunderstore dependency string would be `Dallas-MedicBag-x.y.z` and the GUID would be `Dallas-MedicBag`. If `Hoxton` uploaded his edit of the project, the project would have the dependency string `Hoxton-MedicBag-x.y.z`. Given that Hoxton did not change the `author` field and recompile the project, the GUID of his mod would remain `Dallas-MedicBag`.

Plugins created from scratch (i.e. not using Mason) can have whatever they wish as their GUID. There are few restrictions on the format, and you will need the plugin creator to specify what the GUID is. They may provide documentation on the Thunderstore page, GitHub README, or other means.  
