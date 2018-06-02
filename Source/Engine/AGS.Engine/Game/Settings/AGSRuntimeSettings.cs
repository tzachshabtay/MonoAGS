using AGS.API;

namespace AGS.Engine
{
    public class AGSRuntimeSettings : IRuntimeSettings
    {
        private readonly IGameWindow _gameWindow;
        private readonly IRenderMessagePump _messagePump;
        private readonly IGLUtils _glUtils;
        private bool _preserveAspectRatio;
        private string _title;

        public AGSRuntimeSettings(IGameSettings settings, IGameWindow gameWindow, IRenderMessagePump messagePump, IGLUtils glUtils)
        {
            _glUtils = glUtils;
            _gameWindow = gameWindow;
            _messagePump = messagePump;
            Title = settings.Title;
            VirtualResolution = settings.VirtualResolution;
            Vsync = settings.Vsync;
            PreserveAspectRatio = settings.PreserveAspectRatio;
            WindowState = settings.WindowState;
            WindowBorder = settings.WindowBorder;
            Defaults = settings.Defaults;
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
        
        public AGS.API.Size VirtualResolution { get; }

        public VsyncMode Vsync { get => _gameWindow.Vsync; set => _gameWindow.Vsync = value; }

        public AGS.API.Size WindowSize 
        {
            get => new AGS.API.Size(_gameWindow.ClientWidth, _gameWindow.ClientHeight);
            set => _messagePump.Post(_ => _gameWindow.SetSize(value), null);
        }

        public bool PreserveAspectRatio 
        {
            get => _preserveAspectRatio;
            set => _preserveAspectRatio = value;
        }

        public AGS.API.WindowState WindowState 
        {
            get => (AGS.API.WindowState)_gameWindow.WindowState;
            set => _messagePump.Post(_ => _gameWindow.WindowState = value, null);
        }

        public AGS.API.WindowBorder WindowBorder 
        {
            get => (AGS.API.WindowBorder)_gameWindow.WindowBorder;
            set => _messagePump.Post(_ => _gameWindow.WindowBorder = value, null);
        }

        public IDefaultsSettings Defaults { get; }
    }
}
