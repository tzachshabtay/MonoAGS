using System;
using System.IO;

namespace AGS.Editor.Desktop
{
    //code from: https://github.com/maca88/AsyncGenerator/pull/65/files#diff-5467dd994d7e4cd45d7811d2b22d82d9R104
    public static class MSBuildPath
    {
        /// <summary>
        /// Setup the environment in order MSBuild to work
        /// </summary>
        public static void Setup()
        {
#if NETCOREAPP2_0
            SetupMsBuildPath(GetNetCoreMsBuildPath);
#else
            if (IsMono)
            {
                SetupMsBuildPath(() =>
                {
                    return GetMonoMsBuildPath(monoDir =>
                    {
                        Environment.SetEnvironmentVariable("MSBuildExtensionsPath", Path.Combine(monoDir, "xbuild"));
                    });
                });
                return;
            }
#endif
        }

        public static bool IsMono => Type.GetType("Mono.Runtime") != null;

        public static string GetNetCoreMsBuildPath()
        {
            // Get the sdk path by using the .NET Core runtime assembly location
            // Default locations:
            // Windows -> C:\Program Files\dotnet\shared\Microsoft.NETCore.App\2.0.0\System.Private.CoreLib.dllz
            // Linux -> /usr/share/dotnet/shared/Microsoft.NETCore.App/2.0.0/System.Private.CoreLib.dll
            // OSX -> /usr/local/share/dotnet/shared/Microsoft.NETCore.App/2.0.0/System.Private.CoreLib.dll
            // MSBuild.dll is then located:
            // Windows -> C:\Program Files\dotnet\sdk\2.0.0\MSBuild.dll
            // Linux -> /usr/share/dotnet/sdk/2.0.0/MSBuild.dll
            // OSX -> /usr/local/share/dotnet/sdk/2.0.0/MSBuild.dll

            var assembly = typeof(System.Runtime.GCSettings).Assembly;
            var assemblyDirectory = Path.GetDirectoryName(assembly.Location);
            var directoryInfo = new DirectoryInfo(assemblyDirectory);
            var netCoreVersion = directoryInfo.Name; // e.g. 2.0.0
                                                     // Get the dotnet folder
            var dotnetFolder = directoryInfo.Parent.Parent.Parent.FullName;
            // MSBuild should be located at dotnet/sdk/{version}/MSBuild.dll
            var msBuildPath = Path.Combine(dotnetFolder, "sdk", netCoreVersion, "MSBuild.dll");
            return File.Exists(msBuildPath) ? msBuildPath : null;
        }

        public static string GetMonoMsBuildPath(Action<string> monoDirectoryAction = null)
        {
            // Get the sdk path by using the Mono runtime assembly location
            // Default locations:
            // Windows -> C:\Program Files (x86)\Mono\lib\mono\4.5\mscorlib.dll
            // Linux -> /usr/lib/mono/4.5/mscorlib.dll
            // OSX -> /Library/Frameworks/Mono.framework/Versions/5.2.0/lib/mono/4.5/mscorlib.dll
            // MSBuild.dll is then located:
            // Windows -> C:\Program Files (x86)\Mono\lib\mono\msbuild\15.0\bin\MSBuild.dll
            // Linux -> /usr/lib/mono/msbuild/15.0/bin/MSBuild.dll
            // OSX -> /Library/Frameworks/Mono.framework/Versions/5.2.0/lib/mono/msbuild/15.0/bin/MSBuild.dll

            var assembly = typeof(System.Runtime.GCSettings).Assembly;
            var assemblyDirectory = Path.GetDirectoryName(assembly.Location);
            var directoryInfo = new DirectoryInfo(assemblyDirectory).Parent; // get mono directory
            monoDirectoryAction?.Invoke(directoryInfo.FullName);
            var msBuildPath = Path.Combine(directoryInfo.FullName, "msbuild", "15.0", "bin", "MSBuild.dll");
            return File.Exists(msBuildPath) ? msBuildPath : null;
        }

        private static void SetupMsBuildPath(Func<string> getMsBuildPathFunc)
        {
            var msbuildPath = Environment.GetEnvironmentVariable("MSBUILD_EXE_PATH");
            if (!string.IsNullOrEmpty(msbuildPath) && File.Exists(msbuildPath))
            {
                return;
            }
            msbuildPath = getMsBuildPathFunc();
            if (msbuildPath == null)
            {
                throw new InvalidOperationException(
                    "Environment variable MSBUILD_EXE_PATH is not set or is set incorrectly. " +
                    "Please set MSBUILD_EXE_PATH to point at MSBuild.dll.");
            }
            Environment.SetEnvironmentVariable("MSBUILD_EXE_PATH", msbuildPath);
        }
    }
}