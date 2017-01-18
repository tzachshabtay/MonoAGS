using AGS.API;

namespace AGS.Engine.Desktop
{
    public class DesktopDevice : IDevice
    {
        private static DesktopFontFamilyLoader _fontFamilyLoader; //Must stay in memory

        public DesktopDevice()
        {
            FileSystem = new DesktopFileSystem();
            Assemblies = new DesktopAssemblies();
            _fontFamilyLoader = new DesktopFontFamilyLoader(new ResourceLoader(FileSystem, Assemblies));
            GraphicsBackend = new OpenGLBackend();
            BitmapLoader = new DesktopBitmapLoader(GraphicsBackend);
            BrushLoader = new DesktopBrushLoader();
            FontLoader = new DesktopFontLoader(_fontFamilyLoader);
            ConfigFile = new DesktopEngineConfigFile();
            KeyboardState = new DesktopKeyboardState();
        }

        public IAssemblies Assemblies { get; private set; }

        public IBitmapLoader BitmapLoader { get; private set; }

        public IBrushLoader BrushLoader { get; private set; }

        public IEngineConfigFile ConfigFile { get; private set; }

        public float DisplayDensity { get { return 1f; } }

        public IFileSystem FileSystem { get; private set; }

        public IFontLoader FontLoader { get; private set; }

        public IGraphicsBackend GraphicsBackend { get; private set; }

        public IKeyboardState KeyboardState { get; private set; }
    }
}
