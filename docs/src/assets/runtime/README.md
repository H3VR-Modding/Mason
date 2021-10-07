# The Runtime Stage

The runtime stage, being that it is not as restricted as the setup stage, has significantly more features and complexity.

## Starting from Assets

The runtime and setup stage, although very different in abilities, both share the purpose of running assets.

Assuming the loaders are available for the runtime stage as well, the same asset sequence can be used with the runtime stage:

```yaml
assets:
  runtime:
    assets:
    - path: readme.txt
      plugin: motds
      loader: init
    - path: readme confirm.txt
      plugin: motds
      loader: init
```

However, may produce a different result than the setup stage because of the **sequentiality**, which is the way the assets are executed. Setup is constrained into one sequentiality, hence it being just a list. Runtime contains two possible sequentialities, which are discussed in [the first chapter of this unit](sequentiality.md). Runtime also contains other features, such as [nested pipelines](nested_pipelines.md), which are possible because of sequentiality.
