using System;
using AGS.API;
using OpenTK;

namespace AGS.Engine.Desktop
{
    public class AGSInput : IInput
    {
        private GameWindow _game;
        private IGameWindowSize _windowSize;
        private int _virtualWidth, _virtualHeight;
        private IGameState _state;
        private IShouldBlockInput _shouldBlockInput;
        private IConcurrentHashSet<Key> _keysDown;

        private IObject _mouseCursor;
        private MouseCursor _originalOSCursor;

        public AGSInput(GameWindow game, AGS.API.Size virtualResolution, IGameState state, 
                        IShouldBlockInput shouldBlockInput, IGameWindowSize windowSize)
        {
            _windowSize = windowSize;
            this._shouldBlockInput = shouldBlockInput;
            API.MousePosition.VirtualResolution = virtualResolution;
            API.MousePosition.GetWindowWidth = () => _windowSize.GetWidth(_game);
            API.MousePosition.GetWindowHeight = () => _windowSize.GetHeight(_game);
            this._virtualWidth = virtualResolution.Width;
            this._virtualHeight = virtualResolution.Height;
            this._state = state;
            this._keysDown = new AGSConcurrentHashSet<Key>();

            this._game = game;
            this._originalOSCursor = _game.Cursor;

            MouseDown = new AGSEvent<AGS.API.MouseButtonEventArgs>();
            MouseUp = new AGSEvent<AGS.API.MouseButtonEventArgs>();
            MouseMove = new AGSEvent<MousePositionEventArgs>();
            KeyDown = new AGSEvent<KeyboardEventArgs>();
            KeyUp = new AGSEvent<KeyboardEventArgs>();

            game.MouseDown += async (sender, e) =>
            {
                if (isInputBlocked()) return;
                var button = convert(e.Button);
                if (button == AGS.API.MouseButton.Left) LeftMouseButtonDown = true;
                else if (button == AGS.API.MouseButton.Right) RightMouseButtonDown = true;
                await MouseDown.InvokeAsync(new AGS.API.MouseButtonEventArgs(null, button, MousePosition));
            };
            game.MouseUp += async (sender, e) =>
            {
                if (isInputBlocked()) return;
                var button = convert(e.Button);
                if (button == AGS.API.MouseButton.Left) LeftMouseButtonDown = false;
                else if (button == AGS.API.MouseButton.Right) RightMouseButtonDown = false;
                await MouseUp.InvokeAsync(new AGS.API.MouseButtonEventArgs(null, button, MousePosition));
            };
            game.MouseMove += async (sender, e) =>
            {
                if (isInputBlocked()) return;
                await MouseMove.InvokeAsync(new MousePositionEventArgs(MousePosition));
            };
            game.KeyDown += async (sender, e) =>
            {
                Key key = convert(e.Key);
                _keysDown.Add(key);
                if (isInputBlocked()) return;
                await KeyDown.InvokeAsync(new KeyboardEventArgs(key));
            };
            game.KeyUp += async (sender, e) =>
            {
                Key key = convert(e.Key);
                _keysDown.Remove(key);
                if (isInputBlocked()) return;
                await KeyUp.InvokeAsync(new KeyboardEventArgs(key));
            };
        }

        #region IInputEvents implementation

        public IEvent<AGS.API.MouseButtonEventArgs> MouseDown { get; private set; }

		public IEvent<AGS.API.MouseButtonEventArgs> MouseUp { get; private set; }

		public IEvent<MousePositionEventArgs> MouseMove { get; private set; }

		public IEvent<KeyboardEventArgs> KeyDown { get; private set; }

		public IEvent<KeyboardEventArgs> KeyUp { get; private set; }

        #endregion

        public bool IsKeyDown(Key key) => _keysDown.Contains(key);

        //For some reason GameWindow.Mouse is obsolete.
        //From the warning it should be replaced by Input.Mouse which returns screen coordinates
        //and not window coordinates. Changing will require us to gather the screen monitor coordinates
        //and take multiple monitor issues into account, so for now we'll stick with the obsolete GameWindow.Mouse
        //in the hope that future versions will keep it alive.
#pragma warning disable 618
        public MousePosition MousePosition => new MousePosition(_game.Mouse.X, _game.Mouse.Y, _state.Viewport);
#pragma warning restore 618

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

        private AGS.API.Key convert(OpenTK.Input.Key key) => (AGS.API.Key)(int)key;
    }
}

