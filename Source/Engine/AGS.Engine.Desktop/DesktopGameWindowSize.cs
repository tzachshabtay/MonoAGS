using AGS.API;
using OpenTK;

namespace AGS.Engine.Desktop
{
    public class DesktopGameWindowSize : IGameWindowSize
    {
        public int GetWidth(OpenTK.INativeWindow gameWindow) => gameWindow?.ClientSize.Width ?? 0;
        public int GetHeight(OpenTK.INativeWindow gameWindow) => gameWindow?.ClientSize.Height ?? 0;
        public void SetSize(OpenTK.INativeWindow gameWindow, AGS.API.Size size)
        {
            gameWindow.Size = new System.Drawing.Size(size.Width, size.Height);
        }

        public Rectangle GetWindow(INativeWindow gameWindow) => new Rectangle(0, 0, GetWidth(gameWindow), GetHeight(gameWindow));
    }
}