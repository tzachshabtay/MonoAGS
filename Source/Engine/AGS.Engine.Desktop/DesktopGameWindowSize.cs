using System;
using AGS.API;
using Silk.NET.Windowing.Common;

namespace AGS.Engine.Desktop
{
    public class DesktopGameWindowSize : IGameWindowSize
    {
        public int GetWidth(IWindow gameWindow) => Math.Max(1, gameWindow?.Size.Width ?? 1);
        public int GetHeight(IWindow gameWindow) => Math.Max(1, gameWindow?.Size.Height ?? 1);
        public void SetSize(IWindow gameWindow, AGS.API.Size size)
        {
            gameWindow.Size = new System.Drawing.Size(size.Width, size.Height);
        }

        public Rectangle GetWindow(IWindow gameWindow) => new Rectangle(0, 0, GetWidth(gameWindow), GetHeight(gameWindow));
    }
}
