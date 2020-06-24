# Dependencies

`MonoAGS` currently depends on the following projects:

### [Mono](http://www.mono-project.com/)

This is the runtime used to run the game on Mac, Linux and mobile operating systems.

### [Dot Net Framework](https://docs.microsoft.com/en-us/dotnet/framework/)

This is the runtime used to run the game on Windows OS.

### [Xamarin](https://www.xamarin.com)

Xamrion.Android and Xamarin.IOS are c# bindings for the Android and IOS APIs.

### [OpenTK](https://github.com/opentk/opentk)

A cross-platform binding around OpenGL and OpenAL for displaying graphics and playing audio.
OpenGL ES 2.0 is used for graphics on mobile platforms.

### [SDL2](https://www.libsdl.org/)

An optional dependency that OpenTK can depend on for windowing and input services (not currently used by default as OpenTK also provides native windowing and input services, but might used in the future). If you're intersted in using SDL2, read [here](https://github.com/opentk/opentk-dependencies).

### [OpenAL Soft](http://kcat.strangesoft.net/openal.html)

A software implementation of the OpenAL API, used to support OpenAL on mobile platforms.

### [AutoFac](https://github.com/autofac/Autofac)

Inversion-of-control framework for injecting interfaces at runtime.

### Protobuf.Net

Serialization framework used for saving and loading games.

### [Fody.PropertyChanged](https://github.com/Fody/PropertyChanged)

Used to inject property notifications code into components.

### [NVorbis](https://github.com/ioctlLR/NVorbis)

Used to support playing OGG Vorbis files (audio file format).

### [CSFlac](https://github.com/tzachshabtay/CSFlac)

Used to support playing FLAC files (audio file format).

## Editor only dependencies

### [GuiLabs.Undo](https://github.com/KirillOsenkov/Undo)

Framework for supporting undo/redo operations.

### [Font Awesome](https://fontawesome.com)

Font used to display icons in the editor.

### [Humanizer](https://github.com/Humanizr/Humanizer)

Used by the editor to show human friendly names for properties from code.

### [Fira Sans](https://github.com/mozilla/Fira)

The font used by the editor.

## Development only dependencies

### [NUnit](http://nunit.org/)

A Unit Test framework.

### [Moq](https://github.com/moq/moq4)

Allows to easily mock interfaces, used for unit tests.

### [OpenCover](https://github.com/OpenCover/opencover)

Analyzed the unit tests to create code coverage reports

### [Coveralls.net](https://github.com/csMACnz/coveralls.net)

Uploads code coverage reports to the coveralls website for online viewing.

### [Travis](https://travis-ci.org/)

Automated CI build for Mac & Linux.

### [AppVeyor](https://www.appveyor.com/)

Automated CI build for Windows & Android, also used to build the documentation website.

### [DocFX](https://dotnet.github.io/docfx/)

Used to generate the documentation website.

### [Nerdbank.GitVersioning](https://github.com/AArnott/Nerdbank.GitVersioning)

Used to auto-generate a version number.

### [Travis Matrix Badges](https://github.com/bjfish/travis-matrix-badges)

A small service to display Travis CI build badges on Github.

### [AppVeyor Matrix Badges](https://github.com/tzachshabtay/appveyor-matrix-badges)

A small service to display Appveyor CI build badges on Github.
