# Compilation

Compilation turns a Mason project into something runnable. We haven't added any assets yet, but that is OK. Mason can compile an empty Stratum plugin, and that's all you need to learn on this page.

## How to Compile

To compile your project, simply launch the game modded. BepInEx will trigger Mason, which will then sift through all projects and compile them. The resulting project directory should look something like:

```text
MyProject
 ├── bootstrap.dll
 ├── config.yaml
 ├── manifest.json
 └── project.yaml
```

If not, something probably went wrong. Check your console/log for any errors from Mason. They may be global, thus preventing Mason from compiling any projects. They may also be specific to your project, in which case they would detail what you did wrong.

## What Each File Is

Mason produces two files as a result of compilation: `bootstrap.dll` and `config.yaml`. These files serve two very distinct purposes.

### `bootstrap.dll`

`bootstrap.dll`, or the bootstrap file, is the code that gets run by BepInEx. This is the living embodiment of your project, and what makes it work. The bootstrap file does not require any of the other files present in order to function.

### `config.yaml`

`config.yaml` is a special file intended for use by standalone Mason installations. It is detailed further in [the standalone unit](../standalone/index.md), but for now it can be ignored.

## Running the Result

If you have no errors, the project will run itself! The bootstrap file will be naturally discovered and executed by BepInEx, resulting in the project being ran. Currently, it has no assets, so nothing is performed.
