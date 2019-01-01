using System;
using System.ComponentModel;
using AGS.API;
using AGS.Engine.Desktop;
using OpenTK;

namespace AGS.Editor.Desktop
{
    public class HostingGameDesktopWindowSize : IGameWindowSize, INotifyPropertyChanged
    {
        public Rectangle Window { get; set; } = new Rectangle(0, 0, 1, 1);

#pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067

        public int GetHeight(INativeWindow gameWindow) => Math.Max(1, gameWindow?.ClientSize.Height ?? 1);

        public int GetWidth(INativeWindow gameWindow) => Math.Max(1, gameWindow?.ClientSize.Width ?? 1);

        public Rectangle GetWindow(INativeWindow gameWindow) => Window;

        public void SetSize(INativeWindow gameWindow, Size size)
        {
        }
    }
}