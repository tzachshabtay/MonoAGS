using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using AGS.API;
using OpenTK;
using OpenTK.Input;

namespace AGS.Engine.Desktop
{
    public class AGSInput : IAGSInput
    {
        private GameWindow _game;
        private readonly IGameWindowSize _windowSize;
        private int _virtualWidth, _virtualHeight;
        private float _mouseX, _mouseY;
        private readonly IGameState _state;
        private readonly IShouldBlockInput _shouldBlockInput;
        private readonly IConcurrentHashSet<API.Key> _keysDown;
        private readonly IGameEvents _events;

        private IObject _mouseCursor;
        private MouseCursor _originalOSCursor;
        private readonly ConcurrentQueue<Func<Task>> _actions;
        private int _inUpdate; //For preventing re-entrancy

        public AGSInput(IGameState state, IGameEvents events,
                        IShouldBlockInput shouldBlockInput, IGameWindowSize windowSize)
        {
            _events = events;
            _actions = new ConcurrentQueue<Func<Task>>();
            _windowSize = windowSize;
            this._shouldBlockInput = shouldBlockInput;
            API.MousePosition.GetWindowWidth = () => _windowSize.GetWidth(_game);
            API.MousePosition.GetWindowHeight = () => _windowSize.GetHeight(_game);
            this._state = state;
            this._keysDown = new AGSConcurrentHashSet<API.Key>();

            MouseDown = new AGSEvent<AGS.API.MouseButtonEventArgs>();
            MouseUp = new AGSEvent<AGS.API.MouseButtonEventArgs>();
            MouseMove = new AGSEvent<MousePositionEventArgs>();
            KeyDown = new AGSEvent<KeyboardEventArgs>();
            KeyUp = new AGSEvent<KeyboardEventArgs>();

            if (AGSGameWindow.GameWindow != null) Init(AGSGameWindow.GameWindow);
        }

        public void Init(API.Size virtualResolution)
        {
            API.MousePosition.VirtualResolution = virtualResolution;
            this._virtualWidth = virtualResolution.Width;
            this._virtualHeight = virtualResolution.Height;
        }

        public void Init(GameWindow game)
        {
            if (_game != null) return;
            _game = game;
            this._originalOSCursor = _game.Cursor;

            game.MouseDown += (sender, e) =>
            {
                if (isInputBlocked()) return;
                var button = convert(e.Button);
                _actions.Enqueue(() => MouseDown.InvokeAsync(new AGS.API.MouseButtonEventArgs(null, button, MousePosition)));
            };
            game.MouseUp += (sender, e) =>
            {
                if (isInputBlocked()) return;
                var button = convert(e.Button);
                _actions.Enqueue(() => MouseUp.InvokeAsync(new AGS.API.MouseButtonEventArgs(null, button, MousePosition)));
            };
            game.MouseMove += (sender, e) =>
            {
                if (isInputBlocked()) return;
                _mouseX = e.Mouse.X;
                _mouseY = e.Mouse.Y;
                _actions.Enqueue(() => MouseMove.InvokeAsync(new MousePositionEventArgs(MousePosition)));
            };
            game.KeyDown += (sender, e) =>
            {
                API.Key key = convert(e.Key);
                _keysDown.Add(key);
                if (isInputBlocked()) return;
                _actions.Enqueue(() => KeyDown.InvokeAsync(new KeyboardEventArgs(key)));
            };
            game.KeyUp += (sender, e) =>
            {
                API.Key key = convert(e.Key);
                _keysDown.Remove(key);
                if (isInputBlocked()) return;
                _actions.Enqueue(() => KeyUp.InvokeAsync(new KeyboardEventArgs(key)));
            };

            _events.OnRepeatedlyExecute.Subscribe(onRepeatedlyExecute);
        }

        #region IInputEvents implementation

        public IEvent<AGS.API.MouseButtonEventArgs> MouseDown { get; private set; }

		public IEvent<AGS.API.MouseButtonEventArgs> MouseUp { get; private set; }

		public IEvent<MousePositionEventArgs> MouseMove { get; private set; }

		public IEvent<KeyboardEventArgs> KeyDown { get; private set; }

		public IEvent<KeyboardEventArgs> KeyUp { get; private set; }

        #endregion

        public bool IsKeyDown(AGS.API.Key key) => _keysDown.Contains(key);

        public MousePosition MousePosition => new MousePosition(_mouseX, _mouseY, _state.Viewport);

        public bool LeftMouseButtonDown { get; private set; }
        public bool RightMouseButtonDown { get; private set; }
        public bool IsTouchDrag => false;  //todo: support touch screens on desktops

        public IObject Cursor
		{
            get => _mouseCursor;
            set 
		  	{ 
				_mouseCursor = value;
				_game.Cursor = _mouseCursor == null ? _originalOSCursor : MouseCursor.Empty;
		  	}
		}

        private bool isInputBlocked() => _shouldBlockInput.ShouldBlockInput();

        private AGS.API.MouseButton convert(OpenTK.Input.MouseButton button)
		{
			switch (button) {
			case OpenTK.Input.MouseButton.Left:
				return AGS.API.MouseButton.Left;
			case OpenTK.Input.MouseButton.Right:
				return AGS.API.MouseButton.Right;
			case OpenTK.Input.MouseButton.Middle:
				return AGS.API.MouseButton.Middle;
			default:
				throw new NotSupportedException ();
			}
		}

        private void onRepeatedlyExecute()
        {
            var cursorState = Mouse.GetCursorState();
            LeftMouseButtonDown = cursorState.LeftButton == ButtonState.Pressed;
            RightMouseButtonDown = cursorState.RightButton == ButtonState.Pressed;

            if (Interlocked.CompareExchange(ref _inUpdate, 1, 0) != 0) return;
            try
            {
                while (_actions.TryDequeue(out var action))
                {
                    action();
                }
            }
            finally
            {
                _inUpdate = 0;   
            }
        }

        private AGS.API.Key convert(OpenTK.Input.Key key) => (AGS.API.Key)(int)key;
    }
}

