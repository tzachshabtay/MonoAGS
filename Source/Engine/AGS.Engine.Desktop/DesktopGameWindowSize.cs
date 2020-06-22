using System;
using AGS.API;
using OpenToolkit.Mathematics;
using OpenToolkit.Windowing.Common;

namespace AGS.Engine.Desktop
{
    public class DesktopGameWindowSize : IGameWindowSize
    {
        public int GetWidth(INativeWindow gameWindow) => Math.Max(1, gameWindow?.ClientSize.X ?? 1);
        public int GetHeight(INativeWindow gameWindow) => Math.Max(1, gameWindow?.ClientSize.Y ?? 1);
        public void SetSize(INativeWindow gameWindow, AGS.API.Size size)
        {
            gameWindow.Size = new Vector2i(size.Width, size.Height);
        }

        public Rectangle GetWindow(INativeWindow gameWindow) => new Rectangle(0, 0, GetWidth(gameWindow), GetHeight(gameWindow));
    }
}
