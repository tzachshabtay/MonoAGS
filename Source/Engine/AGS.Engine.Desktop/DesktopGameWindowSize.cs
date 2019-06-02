using System;
using AGS.API;
using OpenTK;

namespace AGS.Engine.Desktop
{
    public class DesktopGameWindowSize : IGameWindowSize
    {
        public int GetWidth(int width) => width;
        public int GetHeight(int height) => height;
        public Rectangle GetWindow(Rectangle window) => window;
        public bool AllowSetSize => true;
    }
}
