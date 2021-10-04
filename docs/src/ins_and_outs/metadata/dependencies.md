# Dependencies

Dependencies are a metadatum that instructs BepInEx on what plugins should load before your plugin, and asserts the version of the other plugins.  
They come in two forms: hard and soft.

## Hard Dependencies

Hard dependencies are the most common form of dependency and probably what you think of when you hear the word "dependency." A hard dependency requires another plugin to be present, and requires that it is at least a certain version. Given that these criteria are satisfied, your plugin will load after the other plugin. In 99% of cases (source: me), a hard dependency is what you need.

### Hard Example

```yaml
dependencies:
  hard:
    h3vr.otherloader: 1.0.0
```

This requires that OtherLoader 1.0.0 is present, and causes it to load before our plugin.

## Soft Dependencies

Soft dependencies are the long lost brothers of hard dependencies. A soft dependency checks if a plugin is present, and if so, causes it to load before your plugin, but does not cause your plugin to fail if it is not present. Soft dependencies have niche use cases, so much so that I cannot think of a reason to use one in the context of Mason.

### Soft Example

```yaml
dependencies:
  soft:
  - wristimate
```

If Wristimate is present, the plugin will load after it. I don't know why you would want to do this, but I can't think of any good examples.
