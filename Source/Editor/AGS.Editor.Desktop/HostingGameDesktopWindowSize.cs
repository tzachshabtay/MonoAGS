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

        public int GetHeight(int height) => height;

        public int GetWidth(int width) => width;

        public Rectangle GetWindow(Rectangle window) => Window;

        public bool AllowSetSize => false;
    }
}