# Features

Standalone Mason, being that it is its own application, has some added features compared to the patcher plugin.

## Automatic Packaging

As covered in [the Getting Started article](../getting_started/packaging.md), packaging can be a bit tedious because of all the factors to consider, compared to Deli's simple "zip everything in the folder" doctrine. Also, it can be easy to forget to run Mason before packaging, causing you to package an old bootstrap file. Automatic packaging allows you to put in the same amount of effort as you did with Deli, but yield a more efficient result.

The pack command, `mason pack`, first begins compilation. When Mason compiles a project, it must enumerate through each asset definition to execute the `path` glob. Mason stores the matching files/folders in a buffer. For example, a project with this structure:

```text
MyProject
 ├── resources
 │   ├── data
 │   │   ├── first_gun
 │   │   └── second_gun
 │   ├── banking info
 │   │   ├── birthday.txt
 │   │   ├── credit card.txt
 │   │   ├── mother's maiden name.txt
 │   │   └── social security number.txt
 │   └── late
 │       ├── late_first_gun
 │       └── late_second_gun
 ├── bootstrap.dll
 ├── config.yaml
 ├── icon.png
 ├── manifest.json
 ├── project.yaml
 └── README.md
```

and these assets:

```yaml
assets:
  runtime:
    sequential: true
    nested:
      - assets:
        - path: data/*
          plugin: h3vr.otherloader
          loader: item_data
      - assets:
        - path: late/*
          plugin: h3vr.otherloader
          loader: item_first_late
```

will only use the following resources:

```text
 .
 ├── data
 │   ├── first_gun
 │   └── second_gun
 └── late
     ├── late_first_gun
     └── late_second_gun
```

The stored paths are then used to determine which files are added to a Thunderstore package. The result of this example would be:

```text
 .
 ├── plugins
 │   ├── resources
 │   │   ├── data
 │   │   │   ├── first_gun
 │   │   │   └── second_gun
 │   │   └── late
 │   │       ├── late_first_gun
 │   │       └── late_second_gun
 │   └── bootstrap.dll
 ├── icon.png
 ├── manifest.json
 └── README.md
```

As you can see, only the necessary files are packaged. It also places the resources folder within the `plugins` folder for you. All from running one command!
