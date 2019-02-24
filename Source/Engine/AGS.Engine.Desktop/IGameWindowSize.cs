using AGS.API;

namespace AGS.Engine.Desktop
{
    public interface IGameWindowSize
    {
        Rectangle GetWindow(OpenTK.INativeWindow gameWindow);
        int GetWidth(OpenTK.INativeWindow gameWindow);
        int GetHeight(OpenTK.INativeWindow gameWindow);
        void SetSize(OpenTK.INativeWindow gameWindow, Size size);
    }
}