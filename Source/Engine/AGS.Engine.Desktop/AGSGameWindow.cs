using System;
using System.ComponentModel;
using AGS.API;
using Autofac;
using OpenTK;
using OpenTK.Graphics;

namespace AGS.Engine.Desktop
{
    public class AGSGameWindow : IGameWindow, IAGSWindowInfo
	{
        private static GameWindow _gameWindow;
        private IGameWindowSize _windowSize;
        private FrameEventArgs _updateFrameArgs, _renderFrameArgs;

        public AGSGameWindow(IGameSettings settings, IGameWindowSize windowSize, AGSInput input)
        {
            _windowSize = windowSize;
            _gameWindow = new GameWindow(settings.WindowSize.Width, settings.WindowSize.Height,
                                         GraphicsMode.Default, settings.Title);
            input.Init(this);
            input.Init(_gameWindow);

            _updateFrameArgs = new FrameEventArgs();
            _renderFrameArgs = new FrameEventArgs();
            _gameWindow.UpdateFrame += onUpdateFrame;
            _gameWindow.RenderFrame += onRenderFrame;
        }

        public static GameWindow GameWindow => _gameWindow;

        public event EventHandler<EventArgs> Load
        {
            add { _gameWindow.Load += value; }
            remove { _gameWindow.Load -= value; }
        }
        public event EventHandler<EventArgs> Resize
        {
            add { _gameWindow.Resize += value; }
            remove { _gameWindow.Resize -= value; }
        }
        public event EventHandler<FrameEventArgs> UpdateFrame;
        public event EventHandler<FrameEventArgs> RenderFrame;
#pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067

        public double TargetUpdateFrequency { get => _gameWindow.TargetUpdateFrequency; set => _gameWindow.TargetUpdateFrequency = value; }
        public string Title { get => _gameWindow.Title; set => _gameWindow.Title = value; }
        public VsyncMode Vsync { get => (VsyncMode)_gameWindow.VSync; set => _gameWindow.VSync = (VSyncMode)value; }
        public bool IsExiting => _gameWindow.IsExiting;
        public API.WindowState WindowState
        {
            get => (API.WindowState)_gameWindow.WindowState;
            set => _gameWindow.WindowState = (OpenTK.WindowState)value;
        }
        public API.WindowBorder WindowBorder
        {
            get => (API.WindowBorder)_gameWindow.WindowBorder;
            set => _gameWindow.WindowBorder = (OpenTK.WindowBorder)value;
        }
        public int Width => _gameWindow.Width;
        public int Height => _gameWindow.Height;
        public int ClientWidth => _windowSize.GetWidth(_gameWindow);
        public int ClientHeight => _windowSize.GetHeight(_gameWindow);
        public void SetSize(Size size) => _windowSize.SetSize(_gameWindow, size);
        public Rectangle GameSubWindow => _windowSize.GetWindow(_gameWindow);
        public float AppWindowHeight => _windowSize.GetHeight(_gameWindow);
        public float AppWindowWidth => _windowSize.GetWidth(_gameWindow);
        public Rectangle ScreenViewport { get; set; }

        public void Run(double updateRate) => _gameWindow.Run(updateRate);
        public void SwapBuffers() => _gameWindow.SwapBuffers();
        public void Exit() => _gameWindow.Exit();
        public void Dispose() => _gameWindow.Dispose();

        private void onUpdateFrame(object sender, OpenTK.FrameEventArgs args)
        {
            _updateFrameArgs.Time = args.Time;
            UpdateFrame?.Invoke(sender, _updateFrameArgs);
        }

        private void onRenderFrame(object sender, OpenTK.FrameEventArgs args)
        {
            _renderFrameArgs.Time = args.Time;
            RenderFrame?.Invoke(sender, _renderFrameArgs);
        }
    }
}