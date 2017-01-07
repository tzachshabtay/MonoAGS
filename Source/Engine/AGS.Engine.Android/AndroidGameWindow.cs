using System;
using AGS.API;
using Android.Content.Res;
using Android.Views;
using Autofac;
using OpenTK;

namespace AGS.Engine.Android
{
    public class AndroidGameWindow : IGameWindow
    {
        private GestureDetector _gestures;
        private bool _started;
        private double _updateRate;

        public static AndroidGameWindow Instance = new AndroidGameWindow();

        private AndroidGameWindow()
        {
            AndroidSimpleGestures simpleGestures = new AndroidSimpleGestures();
            _gestures = new GestureDetector(simpleGestures);

            Resolver.Override(resolver => resolver.Builder.RegisterInstance(this).As<IGameWindow>());
            Resolver.Override(resolver => resolver.Builder.RegisterInstance(simpleGestures));
            Resolver.Override(resolver => resolver.Builder.RegisterType<AndroidInput>().SingleInstance().As<IInput>());
        }

        public AGSGameView View { get; set; }

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

        public int Height { get { return View.Height; } }
        public int Width { get { return View.Width; } }
        public API.WindowBorder WindowBorder { get { return View.WindowBorder; } set { View.WindowBorder = value; } }
        public API.WindowState WindowState { get { return View.WindowState; } set { View.WindowState = value; } }

        public double TargetUpdateFrequency { get { return 60f; } set { } } //todo
        public VsyncMode Vsync { get { return VsyncMode.Off; } set { } } //todo
        public string Title { get { return ""; } set { } } //todo

        public event EventHandler<EventArgs> Load;
        public event EventHandler<FrameEventArgs> RenderFrame;
        public event EventHandler<EventArgs> Resize;
        public event EventHandler<FrameEventArgs> UpdateFrame;

        public void OnLoad(EventArgs args)
        {
            if (!_started)
            {
                _started = true;
                AGSEngineAndroid.Init();
                StartGame();
                var onLoad = Load;
                if (onLoad != null) onLoad(this, args);
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

        public bool OnTouchEvent(MotionEvent e)
        {
            return _gestures.OnTouchEvent(e);
        }

        public void Dispose()
        {
            View.Dispose();
        }

        public void Exit() { } //todo

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
