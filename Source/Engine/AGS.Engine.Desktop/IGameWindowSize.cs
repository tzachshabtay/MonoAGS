using AGS.API;

namespace AGS.Engine.Desktop
{
    public interface IGameWindowSize
    {
        Rectangle GetWindow(Rectangle gameWindow);
        int GetWidth(int width);
        int GetHeight(int height);
        bool AllowSetSize { get; }
    }
}