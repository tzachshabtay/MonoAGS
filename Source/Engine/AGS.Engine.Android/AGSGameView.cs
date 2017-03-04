using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Platform.Android;
using Android.Views;
using Android.Util;
using Android.Content;
using Android.Runtime;
using System.Diagnostics;
using Android.Views.InputMethods;
using Android.Graphics;
using Res=Android.Content.Res;

namespace AGS.Engine.Android
{
    public class AGSGameView : AndroidGameView
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
            AndroidGameWindow.Instance.View = this;
            this.Focusable = true;
            this.FocusableInTouchMode = true;
            this.ViewTreeObserver.GlobalLayout += (sender, e) => 
            {
                Rect rect = new Rect();
                GetWindowVisibleDisplayFrame(rect);
                var heightDiff = this.Height - (rect.Bottom - rect.Top);
                if (heightDiff > 100) SoftKeyboardVisible = true;
                else SoftKeyboardVisible = false;
            };
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
            base.UpdateFrame += onUpdateFrame;
            base.RenderFrame += onRenderFrame;
            base.OnLoad(e);

            AndroidGameWindow.Instance.OnLoad(e);
        }

        protected override void OnResize(EventArgs e)
        {
            MakeCurrent();
            base.OnResize(e);

            AndroidGameWindow.Instance.OnResize(e);
        }

        /*protected override void OnResize(EventArgs e)
        {
            //viewportWidth = Width;
            //viewportHeight = Height;
        }*/

        public override bool OnTouchEvent(MotionEvent e)
        {
            if (SoftKeyboardVisible) return false;
            bool gestureHandled = AndroidGameWindow.Instance.OnTouchEvent(e);
            bool touchHandled = base.OnTouchEvent(e);
            return touchHandled || gestureHandled;
        }

        public event EventHandler<Tuple<Keycode, KeyEvent>> KeyDown;
        public override bool OnKeyDown(Keycode keyCode, KeyEvent e)
        {
            CapslockOn = e.IsShiftPressed;
            var mappedKey = mapKey(keyCode);
            var keyDown = KeyDown;
            if (keyDown != null) keyDown(this, new Tuple<Keycode, KeyEvent>(mappedKey, e));
            return base.OnKeyDown(keyCode, e);
        }

        public event EventHandler<Tuple<Keycode, KeyEvent>> KeyUp;
        public override bool OnKeyUp(Keycode keyCode, KeyEvent e)
        {
            var keyUp = KeyUp;
            var mappedKey = mapKey(keyCode);
            if (keyUp != null) keyUp(this, new Tuple<Keycode, KeyEvent>(mappedKey, e));
            return base.OnKeyUp(keyCode, e);
        }

        protected override void OnConfigurationChanged(Res.Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);
            SoftKeyboardVisible = newConfig.HardKeyboardHidden == Res.HardKeyboardHidden.No;
        }

        public bool CapslockOn { get; private set; }
        public bool SoftKeyboardVisible { get; private set; }

        public void ShowKeyboard()
        {
            RequestFocus();
            SoftKeyboardVisible = true;
            InputMethodManager mgr = (InputMethodManager)this.Context.GetSystemService(Context.InputMethodService);
            mgr.ShowSoftInput(this, ShowFlags.Forced);
        }

        public void HideKeyboard()
        {
            CapslockOn = false;
            SoftKeyboardVisible = false;
            InputMethodManager mgr = (InputMethodManager)this.Context.GetSystemService(Context.InputMethodService);
            mgr.HideSoftInputFromWindow(WindowToken, HideSoftInputFlags.ImplicitOnly);
        }

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

        private Keycode mapKey(Keycode key)
        { 
            //Yikes! A few of the keys don't map nicely to their matching numbers + shift (tested on a Nexus5X), so we need to simulate this for consistency
            switch (key)
            {
                case Keycode.Star:
                    CapslockOn = true;
                    return Keycode.Num8;
                case Keycode.Pound:
                    CapslockOn = true;
                    return Keycode.Num3;
                case Keycode.At:
                    CapslockOn = true;
                    return Keycode.Num2;
                case Keycode.Plus:
                    CapslockOn = true;
                    return Keycode.Plus;
            }
            return key;
        }

        private void onUpdateFrame(object sender, OpenTK.FrameEventArgs args)
        {
            _updateFrameArgs.Time = args.Time;
            AndroidGameWindow.Instance.OnUpdateFrame(_updateFrameArgs);
        }

        private void onRenderFrame(object sender, OpenTK.FrameEventArgs args)
        {
            _renderFrameArgs.Time = args.Time;
            AndroidGameWindow.Instance.OnRenderFrame(_renderFrameArgs);
        }
    }
}