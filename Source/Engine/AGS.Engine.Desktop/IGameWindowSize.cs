using AGS.API;
using Silk.NET.Windowing.Common;

namespace AGS.Engine.Desktop
{
    public interface IGameWindowSize
    {
        Rectangle GetWindow(IWindow gameWindow);
        int GetWidth(IWindow gameWindow);
        int GetHeight(IWindow gameWindow);
        void SetSize(IWindow gameWindow, Size size);
    }
}