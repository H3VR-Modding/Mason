# The Setup Stage

Setup assets follow the WYSIWYG principle. They have no quirks and use a format extremely similar to Deli, in which assets are placed in a sequence:

```yaml
assets:
  setup:
  - path: readme.txt
    plugin: motds
    loader: init
  - path: readme confirm.txt
    plugin: motds
    loader: init
```

In this case, the `readme.txt` asset runs to completion before the `readme confirm.txt` asset begins. When the second asset finishes, the plugin's setup callback is complete.
