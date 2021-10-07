# Globbing

Globbing, or the use of wildcards, is a feature you've likely used many times already. It allows multiple things, in this case files and folders, to be specified by one string. Currently, globbing is only enabled for the `path` property of an asset definition:

```yaml
assets:
  runtime:
    assets:
    - path: guns/*
      plugin: h3vr.otherloader
      loader: item
```

Any time a path separator (`/`) is used, the result of the glob is filtered to only the directories of the previous glob.

## A Runtime Forewarning

Globbing can be another catalyst of runtime bugs. Consider:

```yaml
assets:
  runtime:
    sequential: true
    assets:
    - path: guns/data/*
      plugin: h3vr.otherloader
      loader: item_data
    - path: guns/late/*
      plugin: h3vr.otherloader
      loader: item_late
```

This project file appears to load all data asset bundles parallel, then late asset bundles in parallel. However, all a glob does is expand; an asset definition with a glob in a list is converted into many assets in the same list. This project would load all data asset bundles and late asset bundles in series.

To resolve this problem, simply create a nested pipeline for each asset definition:

```yaml
assets:
  runtime:
    sequential: true
    nested:
    - assets:
      - path: guns/data/*
        plugin: h3vr.otherloader
        loader: item_data
    - assets:
      - path: guns/late/*
        plugin: h3vr.otherloader
        loader: item_late
```

In this case, the globs will expand within the nested pipelines. This would load all data asset bundles in parallel, wait for all to complete, then begin to load all late asset bundles.

## Glob Types

Mason supports many globs, which may be used to your delight.

### Name

Name globs are globs that match files or folders within the directory, and may be composited together within one path segment (text between path separators). Literals (plain text) also fall under this category.  
A composite name glob may be `[cb]at*`, which would match `cat`, `bat`, `batmobile`, `cathode tube` and more, but not `rat`, `brat`, `latissimus`, and many others.

Some name globs do not require any data to fulfill their purpose:

| Glob | Purpose                      |
| ---- | ---------------------------- |
| `*`  | Matches 0 or more characters |
| `?`  | Matches exactly 1 character  |

while others do:

| Glob     | Purpose                                                         | Does Match | Does Not Match |
| -------- | --------------------------------------------------------------- | ---------- | -------------- |
| `[bc]`   | Matches any character within the brackets                       | `b`, `c` | `a`, `d` .. `z`, `A` .. `Z`, `0` .. `9` and more |
| `[!bc]`  | Matches any character *except* those within the brackets        | `a`, `d` .. `z`, `A` .. `Z`, `0` .. `9` and more | `b`, `c` |
| `[0-5]`  | Matches any character between the two characters                | `0` .. `5` | `6` .. `9`, `a` .. `z`, `A` .. `Z` and more |
| `[!0-5]` | Matches any character *except* those between the two characters | `6` .. `9`, `a` .. `z`, `A` .. `Z` and more | `0` .. `5` |

### Globstar (`**`)

Globstar matches every file and empty folder within the current folder and any subfolders, as well as the current folder itself. This is often used to glob any file within a folder with a certain name, regardless of which directory it is in. As an example, `gun/**/data/*` would match `gun/data/something`, `gun/blue/data/anything`, `gun/green/chartreuse/data/you name it`

## Escape Characters

Globs can be escaped, causing the glob to become a literal. This can be done by putting a backslash (`\`) before the brackets. For example, `[brackets].txt` will only match `b.txt`, `r.txt`, `a.txt`, and so on, but `\[brackets\].txt` will only match the file named `[brackets].txt`.  
Be careful with escaping both brackets, as `\[brackets].txt` will merely prefix the glob with a backslash. Similarly, `[brackets\].txt` does not escape the glob, but adds backslash as a possible character that can be matched. As a backslash cannot be used in a file/folder name on Windows, both of the errorful globs will match nothing.
