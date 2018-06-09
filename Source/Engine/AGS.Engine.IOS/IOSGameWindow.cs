extern alias IOS;

using System;
using System.Diagnostics;
using AGS.API;
using Autofac;
using OpenTK;
using OpenTK.Graphics;
using IOS::UIKit;
using IOS::CoreGraphics;
using System.Threading.Tasks;
using System.ComponentModel;

namespace AGS.Engine.IOS
{
    public class IOSGameWindow : IGameWindow, IWindowInfo
    {
        private IOSGameView _view;
        private bool _started;
        private double _updateRate;
        private Lazy<Size> _size;

        public static IOSGameWindow Instance = new IOSGameWindow();

        private IOSGameWindow()
        {
            Debug.WriteLine("IOS Game Window Constructor");
            Resolver.Override(resolver => resolver.Builder.RegisterInstance(this).As<IGameWindow>().As<IWindowInfo>());
            Resolver.Override(resolver => resolver.Builder.RegisterType<IOSGestures>().SingleInstance());
            Resolver.Override(resolver => resolver.Builder.RegisterType<IOSInput>().SingleInstance().As<IInput>());
        }

        public IOSGameView View
        {
            get => _view;
            set
            {
                _view = value;
                _size = new Lazy<Size>(() => new Size((int)(View.Size.Width * View.ContentScaleFactor),
                                                      (int)(View.Size.Height * View.ContentScaleFactor)));
                OnNewView?.Invoke(this, value);
            }
        }

        public event EventHandler<IOSGameView> OnNewView;

        public Action StartGame { get; set; }

        public int ClientHeight => _size.Value.Height;

        public int ClientWidth => _size.Value.Width;

        public int Height => _size.Value.Height;

        public int Width => _size.Value.Width;

        public float AppWindowHeight => Height;

        public float AppWindowWidth => Width;

        public Rectangle GameSubWindow 
        {
            get
            {
                //we have to statically evaluate the window size, because Width & Height must be called from the UI thread.
                float density = (float)UIScreen.MainScreen.Scale;
                int windowWidth = Width;
                int windowHeight = Height;

                return new Rectangle(0, 0, (int)((windowWidth - (GLUtils.ScreenViewport.X * 2)) / density),
                                           (int)((windowHeight - (GLUtils.ScreenViewport.Y * 2)) / density));
            }
        }

        public double TargetUpdateFrequency { get => 60f; set { } } //todo
        public VsyncMode Vsync { get => VsyncMode.Off; set { } } //todo
        public string Title { get => ""; set { } } //todo
        public bool IsExiting => false;  //todo

        public API.WindowBorder WindowBorder { get => (API.WindowBorder)View.WindowBorder; set => View.WindowBorder = (OpenTK.WindowBorder)value; }
        public API.WindowState WindowState { get => (API.WindowState)View.WindowState; set => View.WindowState = (OpenTK.WindowState)value; }

        public event EventHandler<EventArgs> Load;
        public event EventHandler<FrameEventArgs> RenderFrame;
        public event EventHandler<EventArgs> Resize;
        public event EventHandler<FrameEventArgs> UpdateFrame;
#pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067

        public void OnLoad(EventArgs args)
        {
            if (!_started)
            {
                _started = true;
                AGSEngineIOS.Init();
                StartGame();
                Load?.Invoke(this, args);
            }
            else View.Run(_updateRate);
        }

        public void OnResize(CGSize size)
        {
            View.ResizeFrameBuffer();
            float width = (float)size.Width;
            float height = (float)size.Height;
            _size = new Lazy<Size>(() => new Size((int)(width * View.ContentScaleFactor),
                                                  (int)(height * View.ContentScaleFactor)));
            Resize?.Invoke(this, new EventArgs());
        }

        public void OnRenderFrame(FrameEventArgs args)
        {
            RenderFrame(this, args);
        }

        public void OnUpdateFrame(FrameEventArgs args)
        {
            UpdateFrame?.Invoke(this, args);
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