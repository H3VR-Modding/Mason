# Introduction

Mason is a compiler that creates Stratum plugins from a project directory. The resulting DLL fits into the BepInEx ecosystem identically to that of a hand-written DLL; it boasts all the same features as a handwritten BepInEx plugin, such as dependencies, incompatibilities, and process name restrictions, with much less of a hassle to setup and maintain.

The Book of Masonry is the documentation for Mason, and intends to detail every feature in a human-readable form. Some of these features are located in the project and config files, whose schemas you can find [here](https://github.com/H3VR-Modding/Mason/tree/main/schemas).

Contributions to the book are always welcome, even ones as small as correcting a typo or simplifying a confusing sentence. You can make a suggestion directly to a contributor (such as Ash), create an issue on the repository, or click the notepad in the top right corner of the chapter to edit a file and make a pull request.

## As A Book

As this documentation site most closely resembles a book, a few expectations should be established.

Terminology used in books has been adopted. Individual web pages, such as this introduction page, are referred to as **chapters**. Within chapters, headings denote **sections**. A group of similar chapters comprises a **unit**. These distinctions were made to avoid confusion between "section" in the sense of a heading and "section" in the sense of a chapter.

To avoid repetitive content, later chapters assume you have read the former chapters. If a former chapter references content in a later chapter, it will be noted.

## Disclaimers

This book contains many examples of Mason project file features. To keep things concise, only relevant lines are displayed. Other information, most notably the `version: 1` at the beginning, may be required but not shown.
