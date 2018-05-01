using System;
using System.ComponentModel;
using AGS.API;
using Android.Content.Res;
using Android.Views;
using Autofac;
using OpenTK;

namespace AGS.Engine.Android
{
    public class AndroidGameWindow : IGameWindow, IHostingWindow
    {
        private GestureDetector _gestures;
        private bool _started;
        private double _updateRate;
        private AGSGameView _view;

        public static AndroidGameWindow Instance = new AndroidGameWindow();

        private AndroidGameWindow()
        {
            AndroidSimpleGestures simpleGestures = new AndroidSimpleGestures();
            _gestures = new GestureDetector(simpleGestures);

            Resolver.Override(resolver => resolver.Builder.RegisterInstance(this).As<IGameWindow>().As<IHostingWindow>());
            Resolver.Override(resolver => resolver.Builder.RegisterInstance(simpleGestures));
            Resolver.Override(resolver => resolver.Builder.RegisterType<AndroidInput>().SingleInstance().As<IInput>().As<IAGSInput>());
        }

        public AGSGameView View 
        {
            get => _view;
            set 
            { 
                _view = value;
                OnNewView?.Invoke(this, value);
            } 
        }

        public event EventHandler<AGSGameView> OnNewView;

        public Action StartGame { get; set; }

        public int ClientWidth
        {
            get
            {
                var metrics = Resources.System.DisplayMetrics;
                return convertPixelsToDp(metrics.WidthPixels);
            }
        }
        public int ClientHeight
        {
            get
            {
                var metrics = Resources.System.DisplayMetrics;
                return convertPixelsToDp(metrics.HeightPixels);
            }
        }

        public int Height => View.Height;
        public int Width => View.Width;
        public API.WindowBorder WindowBorder { get => View.WindowBorder; set => View.WindowBorder = value; }
        public API.WindowState WindowState { get => View.WindowState; set => View.WindowState = value; }

        public double TargetUpdateFrequency { get => 60f; set { } } //todo
        public VsyncMode Vsync { get => VsyncMode.Off; set { } } //todo
        public string Title { get => ""; set { } } //todo
        public bool IsExiting => false;  //todo

        public Rectangle HostingWindow => new Rectangle(0, 0, Width, Height);

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
                AGSEngineAndroid.Init();
                StartGame();
                Load?.Invoke(this, args);
            }
            else View.Run(_updateRate);
        }

        public void OnResize(EventArgs args)
        {
            Resize?.Invoke(this, args);
        }

        public void OnRenderFrame(FrameEventArgs args)
        {
            RenderFrame(this, args);
        }

        public void OnUpdateFrame(FrameEventArgs args)
        {
            UpdateFrame?.Invoke(this, args);
        }

        public bool OnTouchEvent(MotionEvent e)
        {
            return _gestures.OnTouchEvent(e);
        }

        public void Dispose()
        {
            View.Dispose();
        }

        public void Exit() 
        {
            Java.Lang.JavaSystem.Exit(0);
        } 

        public void Run(double updateRate)
        {
            _updateRate = updateRate;
            View.Run(updateRate);
        }

        public void SetSize(API.Size size) { }

        public void SwapBuffers()
        {
            View.SwapBuffers();
        }

        private int convertPixelsToDp(float pixelValue)
        {
            var dp = (int)((pixelValue) / Resources.System.DisplayMetrics.Density);
            return dp;
        }
    }
}
