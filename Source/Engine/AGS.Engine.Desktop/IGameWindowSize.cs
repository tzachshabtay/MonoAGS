using AGS.API;
using OpenToolkit.Windowing.Common;

namespace AGS.Engine.Desktop
{
    public interface IGameWindowSize
    {
        Rectangle GetWindow(INativeWindow gameWindow);
        int GetWidth(INativeWindow gameWindow);
        int GetHeight(INativeWindow gameWindow);
        void SetSize(INativeWindow gameWindow, Size size);
    }
}