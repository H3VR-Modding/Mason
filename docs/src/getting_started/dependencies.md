# Dependencies

While the project does run and have assets, its lack of declared depenendencies makes it vulnerable to a dependency-not-found error. Adding dependencies to your project ensures all dependencies are present and loaded before your plugin runs.

To add a dependency to the project, use the `dependency` mapping. Another mapping, `hard`, is needed to specify that it is an absolute requirement:

```yaml
dependencies:
  hard:
    h3vr.otherloader: 1.0.0
```

It's also good practice to **depend on the the lowest possible version that has your loaders**. This allows users to revert the loader plugin for whatever purpose is necessary, most notably it breaking.
