namespace AGS.Engine.Desktop
{
    public interface IGameWindowSize
    {
        int GetWidth(OpenTK.INativeWindow gameWindow);
        int GetHeight(OpenTK.INativeWindow gameWindow);
        void SetSize(OpenTK.INativeWindow gameWindow, AGS.API.Size size);
    }
}
