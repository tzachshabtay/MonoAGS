using System.Threading;
using AGS.API;
using OpenTK;

namespace AGS.Engine
{
    public class AGSRuntimeSettings : IRuntimeSettings
    {
        private readonly GameWindow _gameWindow;
        private readonly IMessagePump _messagePump;
        private bool _preserveAspectRatio;

        public AGSRuntimeSettings(IGameSettings settings, GameWindow gameWindow, IMessagePump messagePump)
        {
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

        public VsyncMode Vsync { get { return (VsyncMode)_gameWindow.VSync; } set { _gameWindow.VSync = (VSyncMode)value; } }

        public AGS.API.Size WindowSize 
        { 
            get { return new AGS.API.Size(Hooks.GameWindowSize.GetWidth(_gameWindow), Hooks.GameWindowSize.GetWidth(_gameWindow)); }
            set { _messagePump.Post(_ => Hooks.GameWindowSize.SetSize(_gameWindow, value), null); }                              
        }

        public bool PreserveAspectRatio 
        { 
            get { return _preserveAspectRatio; }
            set 
            { 
                _preserveAspectRatio = value;
                ResetViewport();
            }
        }

        public AGS.API.WindowState WindowState 
        { 
            get { return (AGS.API.WindowState)_gameWindow.WindowState; } 
            set { _messagePump.Post(_ => _gameWindow.WindowState = (OpenTK.WindowState)value, null); } 
        }

        public AGS.API.WindowBorder WindowBorder 
        { 
            get { return (AGS.API.WindowBorder)_gameWindow.WindowBorder; } 
            set { _messagePump.Post(_ => _gameWindow.WindowBorder = (OpenTK.WindowBorder)value, null); } 
        }

        public void ResetViewport()
        {
            GLUtils.RefreshViewport(this, _gameWindow);
        }
    }
}
