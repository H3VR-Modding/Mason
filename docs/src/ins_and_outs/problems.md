# Problems

The problem system of Mason, which manifests itself as errors and warnings, exists to inform creators of problems that exist in their projects before they are ran. This increases the speed at which projects can be worked on, because you no longer need to wait for the game to launch before seeing if there are any problems. This system also integrates well with Visual Studio: Code, to the degree that you might not notice how it works, as discussed on [that page](../standalone/usage/visual_studio_code.md).

Consider this warning:

```text
warning at manifest.json(0,0): [C10] The author of the mod was infered by the directory name. Consider adding an 'author' property.
```

The most important element of this problem is the message:

```text
The author of the mod was infered by the directory name. Consider adding an 'author' property.
```

The message is often what you will see first, and is fairly self explanatory. It explains the problem, and attempts to give instructions on how to resolve it.  
Each message format (the message before it has any specific data inserted) has its own identifier, in this case `C10`. The character indicates which part of Mason emitted the problem (`C` = compiler, `S` = standalone, `P` = patcher), and the number is the unique ID of the mesasge format within that component of Mason.

The next datum of note is the severity. If you have any experience reading logs, this should be a breeze. Mason contains only two severities: `warning` and `error`. Warnings represent a potential future flaw, or that something can be done easier. Errors represent an issue in the project that causes it to be uncompilable.  

The last piece of information in the probem is its location. With this problem, it is `manifest.json(0,0)`. Locations come in four kinds, with each being more specific than the last:

- Global
- File/folder specific
- File range
- File point

Global errors are the rarest and are rarely due to the project itself. They are the every-day error of Mason, and are shown as `.(0,0)`.  
File/folder specific errors are due to problems with the file's presense, location, or holistic contents. They are shown as `path/to/item(0,0)`. This example is a holistic error, as there is no specific point in the file that should have the author property, but the file as a whole should contain it.  
File range errors are caused by a segment of text within the project. They take the form of `path/to/item(X1,Y1-X2,Y2)`, where X1 is the beginning line, Y1 is the beginning column, X2 is the ending line, and Y2 is the ending column.  
File point errors are mainly errors in the parsing libraries that Mason uses (code that reads JSON/YAML), which reports not ranges of text but points. They take the form of `path/to/item(X,Y)` where X is the line number and Y is the column number.
