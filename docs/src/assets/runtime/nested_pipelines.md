# Nested Pipelines

We've covered sequentiality, which allows loading individual assets before others, but what if you wish to load entire batches of assets before others? That's where you need nested pipelines.

Nested pipelines have the same syntax as the runtime stage. In fact, the runtime stage *is* a pipeline! Nested pipelines are just a list of pipelines that are executed before the assets, in the same fashion as the assets. For example:

```yaml
assets:
  runtime:
    sequential: true
    nested:
    - assets:
      - path: rifle
        plugin: h3vr.otherloader
        loader: item_data
      - path: pistol
        plugin: h3vr.otherloader
        loader: item_data
    - assets:
      - path: late_rifle
        plugin: h3vr.otherloader
        loader: item_late
      - path: pistol
        plugin: h3vr.otherloader
        loader: item_late
```

In this example, the nested pipelines act like substages; all the assets of the first pipeline finish loading before any of the assets of the second pipeline begin.  

Sometimes, you have many assets that can be loaded in parallel but some assets that must be loaded in series. Nested pipelines allow these two to be combined into one project:

```yaml
assets:
  runtime:
    nested:
    - sequential: true
      assets:
      - path: readme.txt
        plugin: motds
        loader: init
      - path: readme confirm.txt
        plugin: motds
        loader: init
    assets:
    - path: rifle
      plugin: h3vr.otherloader
      loader: item
    - path: pistol
      plugin: h3vr.otherloader
      loader: item
```

This example begins loading both guns at the same time, as well as the nested pipeline. The nested pipeline loads both readme files, but does so one at a time.
