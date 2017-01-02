using System;
using System.Runtime.InteropServices;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.ES11;
using OpenTK.Platform;
using OpenTK.Platform.Android;

using Android.Content.Res;
using Android.Views;
using Android.Util;
using Android.Content;
using AGS.Engine.Desktop;
using AGS.API;
using Autofac;
using Android.Runtime;
using System.Diagnostics;

namespace AGS.Engine.Android
{
    public class AGSGameView : AndroidGameView, IGameWindow
    {
        private FrameEventArgs _updateFrameArgs, _renderFrameArgs;

        public AGSGameView(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            init();
        }

        public AGSGameView(IntPtr handle, JniHandleOwnership transfer) : base (handle, transfer)
        {
            init();
        }

        private void init()
        {
            _updateFrameArgs = new FrameEventArgs();
            _renderFrameArgs = new FrameEventArgs();

            Resolver.Override(resolver => resolver.Builder.RegisterInstance(this).As<IGameWindow>());
            Resolver.Override(resolver => resolver.Builder.RegisterInstance(this).As<INativeWindow>());
            Resolver.Override(resolver => resolver.Builder.RegisterType<AndroidInput>().SingleInstance().As<IInput>());
        }

        // This method is called everytime the context needs
        // to be recreated. Use it to set any egl-specific settings
        // prior to context creation
        protected override void CreateFrameBuffer()
        {
            ContextRenderingApi = GLVersion.ES2;

            // the default GraphicsMode that is set consists of (16, 16, 0, 0, 2, false)
            try
            {
                Debug.WriteLine("Loading with default settings");

                // if you don't call this, the context won't be created
                base.CreateFrameBuffer();
                return;
            }
            catch (Exception ex)
            {
                Log.Verbose("AGS", "{0}", ex);
            }

            // this is a graphics setting that sets everything to the lowest mode possible so
            // the device returns a reliable graphics setting.
            try
            {
                Log.Verbose("AGS", "Loading with custom Android settings (low mode)");
                GraphicsMode = new GraphicsMode(0, 0, 0, 0, 0, 0, false);

                // if you don't call this, the context won't be created
                base.CreateFrameBuffer();
                return;
            }
            catch (Exception ex)
            {
                Log.Verbose("AGS", "{0}", ex);
            }
            throw new Exception("Can't load egl, aborting");
        }

        // This gets called when the drawing surface is ready
        protected override void OnLoad(EventArgs e)
        {
            //AGSEngineAndroid.Init();

            // UpdateFrame and RenderFrame are called
            // by the render loop. This takes effect
            // when we use 'Run ()', like below
            base.UpdateFrame += onUpdateFrame;
            base.RenderFrame += onRenderFrame;

            // this call is optional, and meant to raise delegates
            // in case any are registered
            base.OnLoad(e);
        }

        protected override void OnResize(EventArgs e)
        {
            MakeCurrent();
            base.OnResize(e);
        }

        /*protected override void OnResize(EventArgs e)
        {
            //viewportWidth = Width;
            //viewportHeight = Height;
        }*/

        public new event EventHandler<FrameEventArgs> UpdateFrame;
        public new event EventHandler<FrameEventArgs> RenderFrame;

        public double TargetUpdateFrequency { get { return 60f; } set { } } //todo
        public VsyncMode Vsync { get { return VsyncMode.Off; } set { } } //todo
        public new string Title { get { return ""; } set { } }
        public new API.WindowState WindowState
        {
            get { return (API.WindowState)base.WindowState; }
            set { base.WindowState = (OpenTK.WindowState)value; }
        }
        public new API.WindowBorder WindowBorder
        {
            get { return (API.WindowBorder)base.WindowBorder; }
            set { base.WindowBorder = (OpenTK.WindowBorder)value; }
        }
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
        public void SetSize(API.Size size) { }

        public void Exit() {  } //todo

        private void onUpdateFrame(object sender, OpenTK.FrameEventArgs args)
        {
            _updateFrameArgs.Time = args.Time;
            var updateFrame = UpdateFrame;
            if (updateFrame != null) updateFrame(sender, _updateFrameArgs);
        }

        private void onRenderFrame(object sender, OpenTK.FrameEventArgs args)
        {
            _renderFrameArgs.Time = args.Time;
            var renderFrame = RenderFrame;
            if (renderFrame != null) renderFrame(sender, _renderFrameArgs);
        }

        private int convertPixelsToDp(float pixelValue)
        {
            var dp = (int)((pixelValue) / Resources.System.DisplayMetrics.Density);
            return dp;
        }
    }
}