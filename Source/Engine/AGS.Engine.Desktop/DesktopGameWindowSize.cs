namespace AGS.Engine.Desktop
{
    public class DesktopGameWindowSize : IGameWindowSize
    {
        public int GetWidth(OpenTK.INativeWindow gameWindow) => gameWindow.ClientSize.Width;
        public int GetHeight(OpenTK.INativeWindow gameWindow) => gameWindow.ClientSize.Height;
        public void SetSize(OpenTK.INativeWindow gameWindow, AGS.API.Size size)
        {
            gameWindow.Size = new System.Drawing.Size(size.Width, size.Height);
        }
    }
}
