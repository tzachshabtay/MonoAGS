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
            _fontFamilyLoader = new DesktopFontFamilyLoader(getResourceLoader());
            GraphicsBackend = new OpenGLBackend();
            BitmapLoader = new DesktopBitmapLoader(GraphicsBackend);
            BrushLoader = new DesktopBrushLoader();
            FontLoader = new DesktopFontLoader(_fontFamilyLoader);
            ConfigFile = new DesktopEngineConfigFile();
            KeyboardState = new DesktopKeyboardState();
        }

        public IAssemblies Assemblies { get; }

        public IBitmapLoader BitmapLoader { get; }

        public IBrushLoader BrushLoader { get; }

        public IEngineConfigFile ConfigFile { get; }

        public float DisplayDensity => 1f;

        public IFileSystem FileSystem { get; }

        public IFontLoader FontLoader { get; }

        public IGraphicsBackend GraphicsBackend { get; }

        public IKeyboardState KeyboardState { get; }

        private IResourceLoader getResourceLoader()
        {
            ResourceLoader resourceLoader = new ResourceLoader();
            FileSystemResourcePack fileResourcePack = new FileSystemResourcePack(FileSystem);
            EmbeddedResourcesPack embeddedResourcePack = new EmbeddedResourcesPack(Assemblies.EntryAssembly);
            resourceLoader.ResourcePacks.Add(new ResourcePack(fileResourcePack, 0));
            resourceLoader.ResourcePacks.Add(new ResourcePack(embeddedResourcePack, 1));
            return resourceLoader;
        }
    }
}
