using System;
using System.Threading.Tasks;
using AGS.API;
using AGS.Engine.Desktop;
using Android.Content.Res;
using Android.Views;

namespace AGS.Engine.Android
{
    public class AndroidInput : IInput
    {
        private IGameWindowSize _windowSize;
        private IGameState _state;
        private int _virtualWidth, _virtualHeight;
        private IShouldBlockInput _shouldBlockInput;
        private DateTime _lastDrag;
        private AGSGameView _view;

        public AndroidInput(AndroidSimpleGestures gestures, AGS.API.Size virtualResolution, 
                            IGameState state, IShouldBlockInput shouldBlockInput, IGameWindowSize windowSize)
        {
            _shouldBlockInput = shouldBlockInput;
            _windowSize = windowSize;
            _state = state;
            this._virtualWidth = virtualResolution.Width;
            this._virtualHeight = virtualResolution.Height;
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
                await MouseMove.InvokeAsync(sender, new MousePositionEventArgs(MouseX, MouseY));
                await Task.Delay(300);
                if (_lastDrag <= now) IsTouchDrag = false;
            };
            gestures.OnUserSingleTap += async (sender, e) => 
            {
                if (isInputBlocked()) return;
                setMousePosition(e);
                LeftMouseButtonDown = true;
                await MouseDown.InvokeAsync(sender, new MouseButtonEventArgs(MouseButton.Left, MouseX, MouseY));
                await Task.Delay(250);
                await MouseUp.InvokeAsync(sender, new MouseButtonEventArgs(MouseButton.Left, MouseX, MouseY));
                LeftMouseButtonDown = false;
            };
            AndroidGameWindow.Instance.OnNewView += onViewChanged;
            onViewChanged(null, AndroidGameWindow.Instance.View);
        }

        public IObject Cursor { get; set; }

        public IEvent<KeyboardEventArgs> KeyDown { get; private set; }

        public IEvent<KeyboardEventArgs> KeyUp { get; private set; }

        public bool LeftMouseButtonDown { get; private set; }

        public IEvent<MouseButtonEventArgs> MouseDown { get; private set; }

        public IEvent<MousePositionEventArgs> MouseMove { get; private set; }

        public IEvent<MouseButtonEventArgs> MouseUp { get; private set; }

        public PointF MousePosition { get; private set; }

        public float MouseX { get { return MousePosition.X; } }

        public float MouseY { get { return MousePosition.Y; } }

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

        private async void onKeyDown(object sender, Tuple<Keycode, KeyEvent> args)
        { 
            var key = args.Item1.Convert();
            if (key == null) return;
            await KeyDown.InvokeAsync(sender, new KeyboardEventArgs(key.Value));
        }

        private async void onKeyUp(object sender, Tuple<Keycode, KeyEvent> args)
        {
            var key = args.Item1.Convert();
            if (key == null) return;
            await KeyUp.InvokeAsync(sender, new KeyboardEventArgs(key.Value));
        }

        private bool isInputBlocked()
        {
            return _shouldBlockInput.ShouldBlockInput();
        }

        private void setMousePosition(MotionEvent e)
        { 
            float x = convertX(e.GetX());
            float y = convertY(e.GetY());
            MousePosition = new PointF(x, y);
        }

        private float convertX(float x)
        {
            var viewport = getViewport();
            var virtualWidth = _virtualWidth / viewport.ScaleX;
            float density = Resources.System.DisplayMetrics.Density;
            x = (x - GLUtils.ScreenViewport.X) / density;
            float width = _windowSize.GetWidth(null) - ((GLUtils.ScreenViewport.X * 2) / density);
            x = MathUtils.Lerp(0f, 0f, width, virtualWidth, x);
            return x + viewport.X;
        }

        private float convertY(float y)
        {
            var viewport = getViewport();
            var virtualHeight = _virtualHeight / viewport.ScaleY;
            float density = Resources.System.DisplayMetrics.Density;
            y = (y - GLUtils.ScreenViewport.Y) / density;
            float height = _windowSize.GetHeight(null) - ((GLUtils.ScreenViewport.Y * 2) / density);
            y = MathUtils.Lerp(0f, virtualHeight, height, 0f, y);
            return y + viewport.Y;
        }

        private IViewport getViewport()
        {
            return _state.Room.Viewport;
        }
    }
}
