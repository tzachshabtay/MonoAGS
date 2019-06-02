using System.ComponentModel;
using AGS.API;

namespace AGS.Editor.Desktop
{
    public class HostingGameDesktopWindow : IWindowInfo
    {
        private readonly HostingGameDesktopWindowSize _windowSize;
        private readonly IWindowInfo _nativeWindow;

        public HostingGameDesktopWindow(HostingGameDesktopWindowSize windowSize, IWindowInfo nativeWindow)
        {
            _windowSize = windowSize;
            _nativeWindow = nativeWindow;
            windowSize.PropertyChanged += onWindowSizePropertyChanged;
        }

        public Rectangle GameSubWindow => _windowSize.Window;
        public float AppWindowHeight => _nativeWindow.AppWindowHeight;
        public float AppWindowWidth => _nativeWindow.AppWindowWidth;

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