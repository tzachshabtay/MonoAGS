using System;
using AGS.API;
using AGS.Engine.Android;

namespace AGS.Engine.IOS
{
    public class IOSDevice : IDevice
    {
        public IOSDevice(IAssemblies assemblies)
        {
            DisplayDensity = 1;
            GraphicsBackend = new OpenGLESBackend();
            BitmapLoader = new IOSBitmapLoader(GraphicsBackend);
            BrushLoader = new IOSBrushLoader();
            FontLoader = new IOSFontLoader();
            ConfigFile = new AndroidEngineConfigFile();
            Assemblies = assemblies;
            FileSystem = new AndroidFileSystem();
            KeyboardState = new IOSKeyboardState();
        }

        public IAssemblies Assemblies { get; private set; }

        public IBitmapLoader BitmapLoader { get; private set; }

        public IBrushLoader BrushLoader { get; private set; }

        public IEngineConfigFile ConfigFile { get; private set; }

        public float DisplayDensity { get; private set; }

        public IFileSystem FileSystem { get; private set; }

        public IFontLoader FontLoader { get; private set; }

        public IGraphicsBackend GraphicsBackend { get; private set; }

        public IKeyboardState KeyboardState { get; private set; }
    }
}
