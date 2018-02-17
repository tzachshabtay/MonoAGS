# Getting Started

## Get the code:

As there is no release yet, the code needs to be downloaded from the [Github repository](https://github.com/tzachshabtay/MonoAGS)

If you're developing from a Windows machine, you need to install [Visual Studio 2017](https://www.visualstudio.com/downloads/) (Community edition is enough and free).

If you're developing from a Mac, you need to install [Visual Studio For Mac](https://www.xamarin.com/download) (Community edition is enough and free).

If you're developing from Linux, you need to install [Mono](http://www.mono-project.com/download/stable/#download-lin) and [MonoDevelop](http://www.monodevelop.com/download/linux/).

After installing, open Visual Studio and open the MonoAGS.sln (the solution) file that's in the root folder of the code you downloaded.

## Compile the code:

When you want to compile for development purposes, choose the Debug configuration (from the top bar), but when you compile for deploying your game, choose DesktopRelease for desktop or AndroidRelease for android. As for platform, choose "Any Cpu" for desktop and "ARM" for android (note: on Mac and Linux "Any Cpu" is actually the default option and nothing is written next to the configuration).

For setting up Android development, see [this](https://developer.xamarin.com/guides/cross-platform/getting_started/installation/windows/#Installation) guide if you're running on Windows and [this](https://developer.xamarin.com/get-started-droid/) guide if you're running on Mac (Android development is currently not possible from Linux). Then setup your device using [this](https://developer.xamarin.com/guides/android/getting_started/installation/set_up_device_for_development/) guide.

For IOS development, you need to run from Mac, see [this](https://developer.xamarin.com/guides/ios/getting_started/installation/) guide for installation, and [this](https://developer.xamarin.com/guides/ios/getting_started/installation/device_provisioning/) guide for setting up your device.
If you own both a Mac and a Windows machine and want to test IOS from your Windows machine, it's possible connecting it to the Mac machine, see [here](https://developer.xamarin.com/guides/ios/getting_started/installation/windows/) for instructions.

## Running the demo:

The game comes with a small demo which you can run to test that everything is working properly (and see a few of the features in action)- if you look at the solution explorer (the side bar) you'll see a demo folder, that contains a DemoQuest.Desktop (for the desktop build) and a DemoQuest.Droid (for the android build). Right click the project which you want to run, and select "Set as startup project". After that, press the "play" button on the top bar (the arrow pointing right) for starting the game.

## Solution structure:

The demo folder contains several projects:

*DemoQuest*- this is the project that contains the core logic of the game. It is a shared project that is used by both the desktop and android projects, so you won't have to write your code twice (or more, once more platforms are added).

*DemoQuest.Desktop*- this is the desktop project that wraps around the shared DemoQuest project and allows you to run the game on a desktop computer. If you have code that you want to run only on desktop but not on other platforms, you'll place that code in this project.

*DemoQuest.Android* this is the android project, works the same way as the desktop project, only android specific code goes here.

Besides the demo folder, there's also the Engine folder which contains *AGS.Engine*, *AGS.Engine.Desktop* and *AGS.Engine.Android* which as you can imagine follow the same pattern: *AGS.Engine* implements the core of the engine's logic while the other projects only add platform specific code.

Additionally, in the root folder of the solution, you'll also find the *AGS.API* project and the *Tests* project. The tests project is for running tests that check that the engine is functioning properly, as a game developer you shouldn't touch that project (you can delete it if you want).

The API project contain all of the definitions of what different systems and components of the game should do. This might also be referred to as contracts, or using the c# term of interfaces (it's a common convention in c# to start all interface names with an "I"). Those interfaces are then implemented by the engine. Ideally, the game is programmed against the interfaces, so plugin authors might implement those interfaces differently and your game will be able to run with those plugins as well.

## Create your own game project:

It is recommended, for now, that you modify the demo game for developing your game and not create your own project. There will be tooling for creating your game, but it's not there yet, so if you want to create a separate project, there will be some legwork involved. Here's the instructions if you do want to start from a blank page:

Right click the solution in the solution explorer, and create a new folder with your game (this is not mandatory, but recommended for better organization). Right click the folder and click to "Add a new project".
This gives you a window with selection of a lot of templates which you can use.
You'll need to do this 3 times and create 3 projects: a shared project (multi-platform), a console project (.Net, this is the desktop project, only required if you want your game to run on desktop) and an Android App (only required if you want your game to run on android).
Add references from the desktop and android projects to the shared project, AGS.API and AGS.Engine (to add references, there should be a references node under the project in the solution explorer, right click it and click on add a reference, which will show the list of project to choose from). Also add a reference to AGS.Engine.Desktop from the desktop project and to AGS.Engine.Android from the android project.
Then copy the boilerplate code from the DemoQuest.Desktop and DemoQuest.Android to your desktop and android projects and whatever code you want from the DemoQuest shared project to your shared project.

## Coding your game:

The official language for coding your game is C#. There are a lot of resources online for learning c# if needed.
While not officially supported, other languages might be used on some or all platforms (they should play nice with dot net, mono, and xamarin). F# would be the best bet as it's the modern functional language counter-part of c#, but other languages might be partially or fully supported.
See the following links:
- http://www.mono-project.com/docs/about-mono/languages/
- https://en.wikipedia.org/wiki/List_of_CLI_languages

If you have tried using the engine with an alternative language, please let us know your findings.
