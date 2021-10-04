# Visual Studio: Code

Mason integrates fairly nicely with Visual Studio: Code. With the full suite of options, Code offers `manifest.json` and `project.yaml` IntelliSense (auto-completion with descriptions and examples), build and pack tasks, and errors appear directly within the text files. The necessary files can be found in [the quickstart repository](https://github.com/H3VR-Modding/Mason-Quickstart/tree/main/.vscode), but you might as well just download the entire repository if you choose to use the files.

Tasks are organized as build tasks, meaning `CTRL + SHIFT + B` (by default) prompts you with a list of options. Running one of them will either succeed, or bring focus to the problems panel. Problems will be underlined when possible in files, and **Mason must be re-ran to update its reported problems**.
