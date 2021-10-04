# Incompatibilities

Incompatibilities are a metadatum much like the opposite of a hard dependency: they prevent your plugin from loading if the specified plugin is present. You may want to do this if a plugin obsoletes another plugin, and you want the user to notice and uninstall the old version.

## Example

```yaml
incompatibilities:
- MyAuthor-OlderModName
```

This example prevents the plugin from loading if `MyAuthor-OlderModName` is present.
