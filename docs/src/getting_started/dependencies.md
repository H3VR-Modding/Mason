# Dependencies

While the project does run and have assets, its lack of declared depenendencies makes it vulnerable to a dependency-not-found error. Adding dependencies to your project ensures all dependencies are present and loaded before your plugin runs.

To add a dependency to the project, use the `dependency` mapping. Another mapping, `hard`, is needed to specify that it is an absolute requirement. Within this mapping, the globally unique identifier (GUID) of the plugin is used as the key, and the minimum version required is the value. For OtherLoader, dependencies in the project file would look like:

```yaml
dependencies:
  hard:
    h3vr.otherloader: 1.0.0
```

`h3vr.otherloader` because OtherLoader's BepInEx GUID is such. Most loaders will have their GUID listed in their documentation, Thunderstore page, or GitHub README.

---

For more information on dependencies, see [their chapter](../metadata/dependencies.md).
