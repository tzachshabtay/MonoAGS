# FAQ (Frequently Asked Questions)

## Troubleshooting

### "The version of Visual Studio is unable to open the following projects."

If you see this error when loading the projects:

![Error](images/unsupported-projects.png)

It means that Xamarin for Android/IOS is not installed.
Check out the [Getting Started](getting-started.md#compile-the-code) section for instructions on how to set up android/ios development.
If you are not interested in developing for android/ios, you can safely ignore this error.

Note that if you are developing from a Linux machine, then developing for ios/android is not available. You can ignore these error and develop your game, and then switch to a Windows/Mac machine at a time when you're interested in testing/developing mobile.

### "The current .NET SDK does not support targeting .NET Standard 2.0. ..."

Check that you have .Net Core installed. You can type `dotnet --info` from the console to verify.
If you don't have it installed, you can install it from [here](https://www.microsoft.com/net/learn/get-started).

### The solution builds fine but the project won't start

Check if you have a message similar to "you can't run that type of project". If so, it means that your startup project is currently a library and not the game itself. You can change the startup project either by right clicking the game project in the solution explorer and selecting "Set as Startup Project", or simply by selecting that project from the drop-down at the top of the screen.
