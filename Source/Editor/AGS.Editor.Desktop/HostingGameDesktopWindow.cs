using System;
using System.ComponentModel;
using AGS.API;
using AGS.Engine;
using AGS.Engine.Desktop;

namespace AGS.Editor.Desktop
{
    public class HostingGameDesktopWindow : IAGSWindowInfo
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

        public Rectangle ScreenViewport { get; set; }

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