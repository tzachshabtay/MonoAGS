using System;
using System.Threading.Tasks;
using AGS.API;

namespace AGS.Engine.IOS
{
    public class IOSInput : IInput
    {
        private IGameWindow _gameWindow;
        private IGameState _state;
        private int _virtualWidth, _virtualHeight;
        private IShouldBlockInput _shouldBlockInput;
        private DateTime _lastDrag;

        public IOSInput(IOSGestures gestures, AGS.API.Size virtualResolution,
                        IGameState state, IShouldBlockInput shouldBlockInput, IGameWindow gameWindow)
        {
            _shouldBlockInput = shouldBlockInput;
            _gameWindow = gameWindow;
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

        private bool isInputBlocked()
        {
            return _shouldBlockInput.ShouldBlockInput();
        }

        private void setMousePosition(MousePositionEventArgs e)
        {
            float x = convertX(e.X);
            float y = convertY(e.Y);
            MousePosition = new PointF(x, y);
        }

        private float convertX(float x)
        {
            return x;
        }

        private float convertY(float y)
        {
            return y;
        }
    }
}
