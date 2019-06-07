using AGS.API;
using AGS.Engine.Desktop;
using Android.Content.Res;

namespace AGS.Engine.Android
{
    public class AndroidDevice : IDevice
    {
        public AndroidDevice(IAssemblies assemblies)
        {
            DisplayDensity = Resources.System.DisplayMetrics.Density;
            GraphicsBackend = new VeldridGraphics();
            BitmapLoader = new AndroidBitmapLoader(GraphicsBackend);
            BrushLoader = new AndroidBrushLoader();
            FontLoader = new AndroidFontLoader();
            ConfigFile = new AndroidEngineConfigFile();
            Assemblies = assemblies;
            FileSystem = new AndroidFileSystem();
            KeyboardState = new AndroidKeyboardState();
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
