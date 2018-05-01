using System;
using System.ComponentModel;
using AGS.API;
using AGS.Engine;

namespace AGS.Editor.Desktop
{
    public class HostingGameDesktopWindow : IHostingWindow
    {
        private readonly HostingGameDesktopWindowSize _windowSize;

        public HostingGameDesktopWindow(HostingGameDesktopWindowSize windowSize)
        {
            _windowSize = windowSize;
            windowSize.PropertyChanged += onWindowSizePropertyChanged;
        }

        public Rectangle HostingWindow => _windowSize.Window;

        public event PropertyChangedEventHandler PropertyChanged;

        private void onWindowSizePropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(HostingGameDesktopWindowSize.Window))
            {
                PropertyChanged?.Invoke(this, args);
            }
        }
    }
}