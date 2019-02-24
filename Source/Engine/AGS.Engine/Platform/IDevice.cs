using AGS.API;

namespace AGS.Engine
{
    public interface IDevice
    {
        IBrushLoader BrushLoader { get; }
        IFontLoader FontLoader { get; }
        IBitmapLoader BitmapLoader { get; }
        IEngineConfigFile ConfigFile { get; }
        IFileSystem FileSystem { get; }
        IKeyboardState KeyboardState { get; }
        IGraphicsBackend GraphicsBackend { get; }
        IAssemblies Assemblies { get; }
        float DisplayDensity { get; }
    }
}
