using System;
using AGS.API;
using OpenTK;

namespace AGS.Engine.Desktop
{
    public class DesktopGameWindowSize : IGameWindowSize
    {
        public int GetWidth(OpenTK.INativeWindow gameWindow) => Math.Max(1, gameWindow?.ClientSize.Width ?? 1);
        public int GetHeight(OpenTK.INativeWindow gameWindow) => Math.Max(1, gameWindow?.ClientSize.Height ?? 1);
        public void SetSize(OpenTK.INativeWindow gameWindow, AGS.API.Size size)
        {
            gameWindow.Size = new System.Drawing.Size(size.Width, size.Height);
        }

        public Rectangle GetWindow(INativeWindow gameWindow) => new Rectangle(0, 0, GetWidth(gameWindow), GetHeight(gameWindow));
    }
}
