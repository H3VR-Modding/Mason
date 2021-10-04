# Processes

Processes are a metadatum that only allows the plugin to be loaded in certain games/applications. **You should only use this if your loaders are available for multiple games**. Given that the loaders are only available for one game and that you list the loaders as dependencies, it follows that your plugin will only ever be able to run on that game because otherwise the loaders would not be present.

## Example

```yaml
processes:
- h3vr.exe
```

This prevents the plugin from running on any game *except for* H3VR.
