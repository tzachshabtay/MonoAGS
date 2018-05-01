using System;
using System.Threading.Tasks;
using AGS.API;
using AGS.Engine.Desktop;
using Android.Content.Res;
using Android.Views;

namespace AGS.Engine.Android
{
    public class AndroidInput : IAGSInput
    {
        private IGameWindowSize _windowSize;
        private IGameState _state;
        private IShouldBlockInput _shouldBlockInput;
        private DateTime _lastDrag;
        private AGSGameView _view;
        private Size _virtualResolution;

        public AndroidInput(AndroidSimpleGestures gestures, IGameState state, IShouldBlockInput shouldBlockInput, IGameWindowSize windowSize)
        {
            MousePosition = new MousePosition(0f, 0f, state.Viewport, new Size(0, 0), windowSize.GetWindow(null));
            _shouldBlockInput = shouldBlockInput;
            _windowSize = windowSize;
            _state = state;
			MouseDown = new AGSEvent<AGS.API.MouseButtonEventArgs>();
            MouseUp = new AGSEvent<AGS.API.MouseButtonEventArgs>();
            MouseMove = new AGSEvent<MousePositionEventArgs>();
            KeyDown = new AGSEvent<KeyboardEventArgs>();
            KeyUp = new AGSEvent<KeyboardEventArgs>();

            gestures.OnUserDrag += async (sender, e) => 
            {
                if (isInputBlocked()) return;
                DateTime now = DateTime.Now;
                _lastDrag = now;
                IsTouchDrag = true;
                setMousePosition(e);
                await MouseMove.InvokeAsync(new MousePositionEventArgs(MousePosition));
                await Task.Delay(300);
                if (_lastDrag <= now) IsTouchDrag = false;
            };
            gestures.OnUserSingleTap += async (sender, e) => 
            {
                if (isInputBlocked()) return;
                setMousePosition(e);
                LeftMouseButtonDown = true;
                await MouseDown.InvokeAsync(new MouseButtonEventArgs(null, MouseButton.Left, MousePosition));
                await Task.Delay(250);
                await MouseUp.InvokeAsync(new MouseButtonEventArgs(null, MouseButton.Left, MousePosition));
                LeftMouseButtonDown = false;
            };
            AndroidGameWindow.Instance.OnNewView += onViewChanged;
            onViewChanged(null, AndroidGameWindow.Instance.View);
        }

        public void Init(AGS.API.Size virtualResolution)
        {
            _virtualResolution = virtualResolution;
        }

        public IObject Cursor { get; set; }

        public IEvent<KeyboardEventArgs> KeyDown { get; private set; }

        public IEvent<KeyboardEventArgs> KeyUp { get; private set; }

        public bool LeftMouseButtonDown { get; private set; }

        public IEvent<MouseButtonEventArgs> MouseDown { get; private set; }

        public IEvent<MousePositionEventArgs> MouseMove { get; private set; }

        public IEvent<MouseButtonEventArgs> MouseUp { get; private set; }

        public MousePosition MousePosition { get; private set; }

        public bool RightMouseButtonDown { get; private set; }

        public bool IsTouchDrag { get; private set; }

        public bool IsKeyDown(Key key)
        {
            return false;
        }

        private void onViewChanged(object sender, AGSGameView view)
        {
            if (_view != null)
            {
                _view.KeyDown -= onKeyDown;
                _view.KeyUp -= onKeyUp;
            }
            if (view != null)
            {
                view.KeyDown += onKeyDown;
                view.KeyUp += onKeyUp;
            }
            _view = view;
        }

        private async void onKeyDown(object sender, (Keycode code, KeyEvent) args)
        { 
            var key = args.code.Convert();
            if (key == null) return;
            await KeyDown.InvokeAsync(new KeyboardEventArgs(key.Value));
        }

        private async void onKeyUp(object sender, (Keycode code, KeyEvent) args)
        {
            var key = args.code.Convert();
            if (key == null) return;
            await KeyUp.InvokeAsync(new KeyboardEventArgs(key.Value));
        }

        private bool isInputBlocked()
        {
            return _shouldBlockInput.ShouldBlockInput();
        }

        private void setMousePosition(MotionEvent e)
        { 
            float x = convertX(e.GetX());
            float y = convertY(e.GetY());
            MousePosition = new MousePosition(x, y, _state.Viewport, _virtualResolution, _windowSize.GetWindow(null));
        }

        private float convertX(float x)
        {
            float density = Resources.System.DisplayMetrics.Density;
            x = (x - GLUtils.ScreenViewport.X) / density;
            return x;
        }

        private float convertY(float y)
        {
            float density = Resources.System.DisplayMetrics.Density;
            y = (y - GLUtils.ScreenViewport.Y) / density;
            return y;
        }
    }
}