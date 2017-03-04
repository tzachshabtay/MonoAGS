using System;
using System.Diagnostics;
using AGS.API;
using Autofac;
using OpenTK;
using OpenTK.Graphics;

namespace AGS.Engine.IOS
{
    public class IOSGameWindow : IGameWindow
    {
        private IOSGameView _view;
        private bool _started;
        private double _updateRate;
        //GameWindow _window;
        //IGraphicsContext _context;

        class TempWindowInfo : OpenTK.Platform.IWindowInfo
        {
            #region IDisposable Members

            public void Dispose()
            {
            }

            #endregion

            public IntPtr Handle
            {
                get { return IntPtr.Zero; }
            }
        }

        public static IOSGameWindow Instance = new IOSGameWindow();

        private IOSGameWindow()
        {
            Debug.WriteLine("IOS Game Window Constructor");
            Resolver.Override(resolver => resolver.Builder.RegisterInstance(this).As<IGameWindow>());
            Resolver.Override(resolver => resolver.Builder.RegisterType<IOSInput>().SingleInstance().As<IInput>());
            //_window = new GameWindow();
            //_context = OpenTK.Platform.Utilities.CreateGraphicsContext(GraphicsMode.Default, new TempWindowInfo(), 2, 0, GraphicsContextFlags.Default);
        }

        public IOSGameView View
        {
            get { return _view; }
            set
            {
                _view = value;
                var onNewView = OnNewView;
                if (onNewView != null) onNewView(this, value);
            }
        }

        public event EventHandler<IOSGameView> OnNewView;

        public Action StartGame { get; set; }

        public int ClientHeight { get { return View.Size.Height; } }

        public int ClientWidth { get { return View.Size.Width; } }

        public int Height { get { return View.Size.Height; } }

        public int Width { get { return View.Size.Width; } }

        public double TargetUpdateFrequency { get { return 60f; } set { } } //todo
        public VsyncMode Vsync { get { return VsyncMode.Off; } set { } } //todo
        public string Title { get { return ""; } set { } } //todo

        public API.WindowBorder WindowBorder { get { return (API.WindowBorder)View.WindowBorder; } set { View.WindowBorder = (OpenTK.WindowBorder)value; } }
        public API.WindowState WindowState { get { return (API.WindowState)View.WindowState; } set { View.WindowState = (OpenTK.WindowState)value; } }

        public event EventHandler<EventArgs> Load;
        public event EventHandler<FrameEventArgs> RenderFrame;
        public event EventHandler<EventArgs> Resize;
        public event EventHandler<FrameEventArgs> UpdateFrame;

        public void OnLoad(EventArgs args)
        {
            if (!_started)
            {
                _started = true;
                AGSEngineIOS.Init();
                StartGame();
                var onLoad = Load;
                if (onLoad != null)
                {
                    onLoad(this, args);
                }
            }
            else View.Run(_updateRate);
        }

        public void OnResize(EventArgs args)
        {
            var onResize = Resize;
            if (onResize != null) onResize(this, args);
        }

        public void OnRenderFrame(FrameEventArgs args)
        {
            RenderFrame(this, args);
        }

        public void OnUpdateFrame(FrameEventArgs args)
        {
            UpdateFrame(this, args);
        }

        public void Dispose()
        {
            View.Dispose();
        }

        public void Exit()
        {
            //https://developer.apple.com/library/content/qa/qa1561/_index.html
            throw new InvalidOperationException("It's against IOS guidelines to have a Quit button!!");
        }

        public void Run(double updateRate)
        {
            _updateRate = updateRate;
            View.Run(updateRate);
        }

        public void SetSize(Size size) { }

        public void SwapBuffers()
        {
            View.SwapBuffers();
        }
    }
}
