namespace AGS.Engine.Desktop
{
    public interface IGameWindowSize
    {
        int GetWidth(OpenTK.GameWindow gameWindow);
        int GetHeight(OpenTK.GameWindow gameWindow);
        void SetSize(OpenTK.GameWindow gameWindow, AGS.API.Size size);
    }
}
