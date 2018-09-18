# Using a different IDE

If you don't want to use `Visual Studio` and don't need to target Android/IOS you can install the needed to components to compile from command-line, then you can use whatever coding environment you wish (even `Notepad`!).

Note: For Android/IOS, `Visual Studio` is currently a requirement, but you can still do all your coding for desktop on your favorite editor and switch to `Visual Studio` in the end.

## Prerequisites- On Windows

Install [Build Tools For Visual Studio 2017](https://visualstudio.microsoft.com/downloads/#build-tools-for-visual-studio-2017).
In the installer, check `.Net Core Build Tools` (under `Workloads`) and check `Nuget Package Manager` under `Individual Components`.

After installing, add the MSBuild folder to your PATH environment variable (`%programfiles(x86)%\Microsoft Visual Studio\2017\BuildTools\MSBuild\15.0\Bin`).

## Prerequisites- On Mac/Linux

Install [Mono](https://www.mono-project.com/download/stable/) (GTK# is not needed)
Add the `Mono/bin` folder to your PATH environment variable.

TBD.

## Compile from Command-Line

1. Run: `dotnet restore` (you need to do this every time you add a new package to your projects, so if you don't add anything specific you'll only need to run this once).
2. To compile, run `msbuild /p:Configuration=DesktopDebug /p:Platform="Any CPU" MonoAGS.sln` for debug, or `msbuild /p:Configuration=DesktopRelease /p:Platform="Any CPU" MonoAGS.sln` for release.

## IDEs

While using `Notepad` for writing code is fine and dandy, sometimes you want a little more from your coding environment, like debugging, code completion, syntax highlighting, etc.
For that you need an IDE. Here's some additional instructions (after you've setup the prequisites) for a few IDEs.

## VS Code

`Visual Studio Code` (not to be confused with `Visual Studio`, which is a completely different product) is an open source IDE which supports c# (and a lot of other languages) with extensions. 

Install [VS Code](https://code.visualstudio.com/download) and start it.
In `VS Code`, go to the extensions tab, and install the `C#` extension and the `Mono Debug` extension.