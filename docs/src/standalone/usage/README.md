# Usage

Before calling standalone Mason, you must add a `config.yaml` file to your project directory. This file instructs Mason on where it can find the DLLs it needs, as it does not know where the game files are like the patcher plugin does. This file may appear as:

```yaml
directories:
  bepinex: C:\Users\<user>\AppData\Roaming\Local\r2modmanPlus-local\profiles\H3VR\<profile>\BepInEx
  managed: C:\Program Files (x86)\Steam\steamapps\common\H3VR\h3vr_Data\Managed
```

Additionally, this BepInEx directory must have Stratum installed.
