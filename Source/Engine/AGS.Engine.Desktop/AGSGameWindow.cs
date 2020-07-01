using System;
using System.ComponentModel;
using AGS.API;
using OpenToolkit.Windowing.Desktop;
using OpenToolkit.Windowing.Common;
using OpenToolkit.Mathematics;

namespace AGS.Engine.Desktop
{
    public class AGSGameWindow : IGameWindow, IWindowInfo
    {
        private static GameWindow _gameWindow;
        private IGameWindowSize _windowSize;
        private FrameEventArgs _updateFrameArgs, _renderFrameArgs;

        public AGSGameWindow(IGameSettings settings, IGameWindowSize windowSize)
        {
            _windowSize = windowSize;
            _gameWindow = new GameWindow(new GameWindowSettings(), new NativeWindowSettings
            {
                Title = settings.Title,
                Size = new Vector2i(settings.WindowSize.Width, settings.WindowSize.Height),
                Flags = ContextFlags.ForwardCompatible,
            });
            OnInit?.Invoke();

            _updateFrameArgs = new FrameEventArgs();
            _renderFrameArgs = new FrameEventArgs();
            _gameWindow.UpdateFrame += onUpdateFrame;
            _gameWindow.RenderFrame += onRenderFrame;
            _gameWindow.Resize += onResize;
        }

        public static GameWindow GameWindow => _gameWindow;

        public static Action OnInit { get; set; }

        public event Action Load
        {
            add { _gameWindow.Load += value; }
            remove { _gameWindow.Load -= value; }
        }
        public event EventHandler<ResizeEventArgs> Resize;
        public event EventHandler<FrameEventArgs> UpdateFrame;
        public event EventHandler<FrameEventArgs> RenderFrame;
#pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067

        public double TargetUpdateFrequency { get => _gameWindow.UpdateFrequency; set => _gameWindow.UpdateFrequency = value; }
        public string Title { get => _gameWindow.Title; set => _gameWindow.Title = value; }
        public VsyncMode Vsync { get => (VsyncMode)_gameWindow.VSync; set => _gameWindow.VSync = (VSyncMode)value; }
        public bool IsExiting => _gameWindow.IsExiting;
        public API.WindowState WindowState
        {
            get => (API.WindowState)_gameWindow.WindowState;
            set => _gameWindow.WindowState = (OpenToolkit.Windowing.Common.WindowState)value;
        }
        public API.WindowBorder WindowBorder
        {
            get => (API.WindowBorder)_gameWindow.WindowBorder;
            set => _gameWindow.WindowBorder = (OpenToolkit.Windowing.Common.WindowBorder)value;
        }
        public int Width => Math.Max(1, _gameWindow.Size.X);
        public int Height => Math.Max(1, _gameWindow.Size.Y);
        public int ClientWidth => Math.Max(1, _windowSize.GetWidth(_gameWindow));
        public int ClientHeight => Math.Max(1, _windowSize.GetHeight(_gameWindow));
        public void SetSize(Size size) => _windowSize.SetSize(_gameWindow, size);
        public Rectangle GameSubWindow => _windowSize.GetWindow(_gameWindow);
        public float AppWindowHeight => Math.Max(1, _windowSize.GetHeight(_gameWindow));
        public float AppWindowWidth => Math.Max(1, _windowSize.GetWidth(_gameWindow));
        public Rectangle ScreenViewport { get; set; } = new Rectangle(0, 0, 1, 1);

        public void Run(double updateRate)
        {
            _gameWindow.UpdateFrequency = updateRate;
            _gameWindow.Run();
        }

        public void SwapBuffers() => _gameWindow.SwapBuffers();
        public void Exit() => _gameWindow.Close();
        public void Dispose() => _gameWindow.Dispose();

        private void onUpdateFrame(OpenToolkit.Windowing.Common.FrameEventArgs args)
        {
            _updateFrameArgs.Time = args.Time;
            UpdateFrame?.Invoke(null, _updateFrameArgs);
        }

        private void onRenderFrame(OpenToolkit.Windowing.Common.FrameEventArgs args)
        {
            _renderFrameArgs.Time = args.Time;
            RenderFrame?.Invoke(null, _renderFrameArgs);
        }

        private void onResize(OpenToolkit.Windowing.Common.ResizeEventArgs args)
        {
            Resize?.Invoke(null, new ResizeEventArgs { Width = args.Width, Height = args.Height });
        }
    }
}