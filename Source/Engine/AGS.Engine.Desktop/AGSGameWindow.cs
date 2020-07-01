using System;
using System.ComponentModel;
using System.Diagnostics;
using AGS.API;
using Silk.NET.Windowing;
using Silk.NET.Windowing.Common;

namespace AGS.Engine.Desktop
{
    public class AGSGameWindow : IGameWindow, IWindowInfo
    {
        private static IWindow _gameWindow;
        private IGameWindowSize _windowSize;
        private FrameEventArgs _updateFrameArgs, _renderFrameArgs;

        public AGSGameWindow(IGameSettings settings, IGameWindowSize windowSize)
        {
            _windowSize = windowSize;
            _gameWindow = Window.Create(new WindowOptions
            {
                Title = settings.Title,
                UseSingleThreadedWindow = false,
                IsEventDriven = true,
                Size = new System.Drawing.Size(settings.WindowSize.Width, settings.WindowSize.Height)
            });
            OnInit?.Invoke();

            _updateFrameArgs = new FrameEventArgs();
            _renderFrameArgs = new FrameEventArgs();
            _gameWindow.Update += onUpdateFrame;
            _gameWindow.Render += onRenderFrame;
        }

        public static IWindow GameWindow => _gameWindow;

        public static Action OnInit { get; set; }

        public event Action Load
        {
            add { _gameWindow.Load += value; }
            remove { _gameWindow.Load -= value; }
        }
        public event Action<System.Drawing.Size> Resize
        {
            add { _gameWindow.Resize += value; }
            remove { _gameWindow.Resize -= value; }
        }
        public event EventHandler<FrameEventArgs> UpdateFrame;
        public event EventHandler<FrameEventArgs> RenderFrame;
#pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067

        public double TargetUpdateFrequency { get => _gameWindow.UpdatesPerSecond; set => _gameWindow.UpdatesPerSecond = value; }
        public string Title { get => _gameWindow.Title; set => _gameWindow.Title = value; }
        public VsyncMode Vsync { get => (VsyncMode)_gameWindow.VSync; set => _gameWindow.VSync = (VSyncMode)value; }
        public bool IsExiting => _gameWindow.IsClosing;
        public API.WindowState WindowState
        {
            get => (API.WindowState)_gameWindow.WindowState;
            set => _gameWindow.WindowState = (Silk.NET.Windowing.Common.WindowState)value;
        }
        public API.WindowBorder WindowBorder
        {
            get => (API.WindowBorder)_gameWindow.WindowBorder;
            set => _gameWindow.WindowBorder = (Silk.NET.Windowing.Common.WindowBorder)value;
        }
        public int Width => Math.Max(1, _gameWindow.Size.Width);
        public int Height => Math.Max(1, _gameWindow.Size.Height);
        public int ClientWidth => Math.Max(1, _windowSize.GetWidth(_gameWindow));
        public int ClientHeight => Math.Max(1, _windowSize.GetHeight(_gameWindow));
        public void SetSize(Size size) => _windowSize.SetSize(_gameWindow, size);
        public Rectangle GameSubWindow => _windowSize.GetWindow(_gameWindow);
        public float AppWindowHeight => Math.Max(1, _windowSize.GetHeight(_gameWindow));
        public float AppWindowWidth => Math.Max(1, _windowSize.GetWidth(_gameWindow));
        public Rectangle ScreenViewport { get; set; } = new Rectangle(0, 0, 1, 1);

        public void Run(double updateRate)
        {
            Debug.WriteLine("Running Window");
            _gameWindow.UpdatesPerSecond = updateRate;
            _gameWindow.Run();
        }
        public void SwapBuffers() => _gameWindow.SwapBuffers();
        public void Exit() => _gameWindow.Close();
        public void Dispose() => _gameWindow.Dispose();

        private void onUpdateFrame(double time)
        {
            _updateFrameArgs.Time = time;
            UpdateFrame?.Invoke(null, _updateFrameArgs);
        }

        private void onRenderFrame(double time)
        {
            _renderFrameArgs.Time = time;
            RenderFrame?.Invoke(null, _renderFrameArgs);
        }
    }
}