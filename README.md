# MonoAGS
AGS (Adventure Game Studio) reimagined in Mono

### Builds:

| Windows            | Android            | Linux            | Mac            |
|--------------------|--------------------|------------------|----------------|
| [![Windows][1]][3] | [![Android][2]][3] | [![Linux][4]][6] | [![Mac][5]][6] |

[1]: https://appveyor-matrix-badges.herokuapp.com/repos/tzachshabtay/MonoAGS/branch/master/2
[2]: https://appveyor-matrix-badges.herokuapp.com/repos/tzachshabtay/MonoAGS/branch/master/3
[3]: https://ci.appveyor.com/project/tzachshabtay/monoags
[4]: https://travis-matrix-badges.herokuapp.com/repos/tzachshabtay/MonoAGS/branches/master/1
[5]: https://travis-matrix-badges.herokuapp.com/repos/tzachshabtay/MonoAGS/branches/master/2
[6]: https://travis-ci.org/tzachshabtay/MonoAGS


Code Coverage:

[![Coverage Status](https://coveralls.io/repos/tzachshabtay/MonoAGS/badge.svg?branch=master&service=github)](https://coveralls.io/github/tzachshabtay/MonoAGS?branch=master)

### Compiling the code:

#### On Windows:

Install Visual Studio 2017 Community, make sure to install both the dot net framework workflow and the mobile workflow.
Right click the solution and build.

To be able to build for Android, follow the tutorial here for setting up: https://developer.xamarin.com/guides/cross-platform/getting_started/installation/windows/#Installation
You'll also need to setup your Android device for testing purposes, see here for instructions:  https://developer.xamarin.com/guides/android/getting_started/installation/set_up_device_for_development/.
Once you're setup, select the `AndroidRelease` configuration and the `ARM` architecture (both should be available from the drop downs at the top bar), and then you can build and run.

Please note that you will not be able to run IOS without a Mac, but if you own a Mac, it's possible to connect the Mac to the Windows machine, and run from Windows, see here for setup: https://developer.xamarin.com/guides/ios/getting_started/installation/windows/.

To run the unit tests, please install the NUnit 2 Test Adapter (available from Visual Studio, tools menu -> Extensions and Updates... -> Online).

#### On Mac:

Install Visual Studio for Mac Community Edition.
Right click the solution and build.

To be able to build for Android, follow the tutorial here for setting up: https://developer.xamarin.com/get-started-droid/
You'll also need to setup your Android device for testing purposes, see here for instructions:  https://developer.xamarin.com/guides/android/getting_started/installation/set_up_device_for_development/.
Once you're setup, select the `AndroidRelease` configuration and the `ARM` architecture (both should be available from the drop downs at the top bar), and then you can build and run.

To be able to build for IOS, follow the tutorial here for setting up: https://developer.xamarin.com/guides/ios/getting_started/installation/
You'll also need to setup your Android device for testing purposes, see here for instructions:  https://developer.xamarin.com/guides/ios/getting_started/installation/device_provisioning/.

#### On Linux:

Install Mono and MonoDevelop.
Right click the solution and build.

It's not currently possible compiling for Android & IOS from Linux.

### Documentation

Documentation is available at: https://tzachshabtay.github.io/MonoAGS/
