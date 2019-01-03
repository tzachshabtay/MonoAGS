using System.ComponentModel;
using AGS.API;

namespace AGS.Editor.Desktop
{
    public class HostingGameDesktopWindow : IWindowInfo
    {
        private readonly HostingGameDesktopWindowSize _windowSize;
        private readonly OpenTK.INativeWindow _nativeWindow;

        public HostingGameDesktopWindow(HostingGameDesktopWindowSize windowSize, OpenTK.INativeWindow nativeWindow)
        {
            _windowSize = windowSize;
            _nativeWindow = nativeWindow;
            windowSize.PropertyChanged += onWindowSizePropertyChanged;
        }

        public Rectangle GameSubWindow => _windowSize.Window;
        public float AppWindowHeight => _windowSize.GetHeight(_nativeWindow);
        public float AppWindowWidth => _windowSize.GetWidth(_nativeWindow);

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