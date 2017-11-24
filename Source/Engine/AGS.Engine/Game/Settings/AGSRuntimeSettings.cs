using AGS.API;

namespace AGS.Engine
{
    public class AGSRuntimeSettings : IRuntimeSettings
    {
        private readonly IGameWindow _gameWindow;
        private readonly IRenderMessagePump _messagePump;
        private readonly IGLUtils _glUtils;
        private bool _preserveAspectRatio;

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
        }

        public string Title { get { return _gameWindow.Title; } set { _gameWindow.Title = value; } }
        
        public AGS.API.Size VirtualResolution { get; private set; }

        public VsyncMode Vsync { get { return _gameWindow.Vsync; } set { _gameWindow.Vsync = value; } }

        public AGS.API.Size WindowSize 
        { 
            get { return new AGS.API.Size(_gameWindow.ClientWidth, _gameWindow.ClientHeight); }
            set { _messagePump.Post(_ => _gameWindow.SetSize(value), null); }                              
        }

        public bool PreserveAspectRatio 
        { 
            get { return _preserveAspectRatio; }
            set 
            { 
                _preserveAspectRatio = value;
            }
        }

        public AGS.API.WindowState WindowState 
        { 
            get { return (AGS.API.WindowState)_gameWindow.WindowState; } 
            set { _messagePump.Post(_ => _gameWindow.WindowState = value, null); } 
        }

        public AGS.API.WindowBorder WindowBorder 
        { 
            get { return (AGS.API.WindowBorder)_gameWindow.WindowBorder; } 
            set { _messagePump.Post(_ => _gameWindow.WindowBorder = value, null); } 
        }
    }
}
