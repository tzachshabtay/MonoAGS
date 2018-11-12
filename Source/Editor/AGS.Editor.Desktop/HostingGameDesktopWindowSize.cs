using System.ComponentModel;
using AGS.API;
using AGS.Engine.Desktop;
using OpenTK;

namespace AGS.Editor.Desktop
{
    public class HostingGameDesktopWindowSize : IGameWindowSize, INotifyPropertyChanged
    {
        public Rectangle Window { get; set; }

#pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067

        public int GetHeight(INativeWindow gameWindow) => gameWindow?.ClientSize.Height ?? 0;

        public int GetWidth(INativeWindow gameWindow) =>gameWindow?.ClientSize.Width ?? 0;

        public Rectangle GetWindow(INativeWindow gameWindow) => Window;

        public void SetSize(INativeWindow gameWindow, Size size)
        {
        }
    }
}