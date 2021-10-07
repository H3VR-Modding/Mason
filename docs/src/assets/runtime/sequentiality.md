# Sequentiality

Sequentiality is the way assets are executed. Using the wrong sequentiality for the job can result in bugs traditionally called [race conditions](https://en.wikipedia.org/wiki/Race_condition).

If sequentiality allows creators to screw over their own projects, why bother with the feature at all? Consider this runtime stage:

```yaml
assets:
  runtime:
    assets:
    - path: guns/pistol
      plugin: h3vr.otherloader
      loader: item
    - path: guns/rifle
      plugin: h3vr.otherloader
      loader: item
    - path: guns/smg
      plugin: h3vr.otherloader
      loader: item
```
  
Because the default sequentiality is parallel, this runtime stage is running the assets in parallel. With parallel sequentiality, the plugin does not await assets to begin the next one. It begins `guns/pistol`, `guns/rifle`, and then `guns/smg` before any of the previous assets have completed. The assets may finish in the same order, but it is never certain. This may result in bugs if creators do not factor in parallel processing.  
Although it can cause problems, it pays dividends in accelerating the loading time. Each asset bundle is not dependent on any of the other asset bundles, so all asset bundles are able to load independently and thus simultaneously. These gun bundles may be massive, requiring solid chunks of time to decompress on a single thread. In series, these all compete for the same hardware resources and thus the total time to load is the sum of each individual load. When adjusted to be ran in parallel, though, the total loading time of the plugin is greatly reduced.

## Overriding Sequentiality

If the default, parallel sequentiality does not suit your needs, you can override it with the `sequential` property. This is especially useful for OtherLoader's on-demand loading feature:

```yaml
assets:
  runtime:
    sequential: true
    assets:
    - path: pistol
      plugin: h3vr.otherloader
      loader: item_data
    - path: late_pistol
      plugin: h3vr.otherloader
      loader: item_late
```

In this example, `pistol` begins and finishes loading before `late_pistol` begins loading. This prevents the late asset bundle, which should be loaded last, from being loaded earlier. To load the other guns in parallel in the same fashion, we need [nested pipelines](nested_pipelines.md).
