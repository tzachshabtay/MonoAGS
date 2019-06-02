using System.ComponentModel;
using AGS.API;

namespace AGS.Engine
{
    public class AGSRuntimeSettings : IRuntimeSettings
    {
        private readonly IGameWindow _gameWindow;
        private readonly IRenderMessagePump _messagePump;
        private string _title;
        private VsyncMode _vsync;
        private Size _windowSize;

#pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067

        public AGSRuntimeSettings(IGameSettings settings, IGameWindow gameWindow, IRenderMessagePump messagePump)
        {
            _gameWindow = gameWindow;
            _messagePump = messagePump;
            LoadFrom(settings);
        }

        public void LoadFrom(IGameSettings settings)
        {
            Title = settings.Title;
            VirtualResolution = settings.VirtualResolution;
            Vsync = settings.Vsync;
            PreserveAspectRatio = settings.PreserveAspectRatio;
            WindowState = settings.WindowState;
            WindowBorder = settings.WindowBorder;
            Backend = settings.Backend;
            _windowSize = new AGS.API.Size(_gameWindow.ClientWidth, _gameWindow.ClientHeight);
            var messageBoxSettings = Defaults?.MessageBox;
            var fontSettings = Defaults?.Fonts;
            var dialogSettings = Defaults?.Dialog;
            Defaults = settings.Defaults;
            if (Defaults.MessageBox == null)
            {
                Defaults.MessageBox = messageBoxSettings; 
            }
            if (Defaults.Fonts == null)
            {
                Defaults.Fonts = fontSettings;
            }
            if (Defaults.Dialog == null)
            {
                Defaults.Dialog = dialogSettings;
            }
        }

        public string Title 
        { 
            get => _title; 
            set 
            { 
                _gameWindow.Title = value; 
                _title = value; 
            } 
        }
        
        public Size VirtualResolution { get; private set; }

        public VsyncMode Vsync 
        { 
            get => _vsync; 
            set
            {
                _vsync = value;
                _messagePump.Post(_ => _gameWindow.Vsync = value, null);
            }
        }

        public Size WindowSize 
        {
            get => _windowSize;
            set
            {
                _windowSize = value;
                _messagePump.Post(_ => _gameWindow.SetSize(value), null);
            }
        }

        public bool PreserveAspectRatio { get; set; }

        public WindowState WindowState 
        {
            get => _gameWindow.WindowState;
            set => _messagePump.Post(_ => _gameWindow.WindowState = value, null);
        }

        public WindowBorder WindowBorder 
        {
            get => _gameWindow.WindowBorder;
            set => _messagePump.Post(_ => _gameWindow.WindowBorder = value, null);
        }

        public GraphicsBackend Backend { get; private set; }

        public IDefaultsSettings Defaults { get; private set; }
    }
}
