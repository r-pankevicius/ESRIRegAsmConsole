# ESRIRegAsmConsole
ESRIRegAsm as a proper console app.

## What is an inproper console app?

The one that you can use GUI version only:
![](./assets/Arguments_Arcgis_10_7.png "Screen if running ESRIRegAsm.exe without arguments")

/s (silent) switch looks promising but you don't get error codes back (ERRORLEVEL).

/e (display error console) looks promising but console is opened in new window and is not redirected to the main console output.

## What is a proper console app?

The one you can use in a batch file without pressing OK button multiple times in a row, stop immediately when the error occurs,
continue with other tasks if everything is OK.

These things were too difficult to understand for hard working ESRI programmers because they were too busy pressing OK buttons.
