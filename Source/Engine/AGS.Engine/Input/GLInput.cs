using System;
using AGS.API;
using OpenTK;
using OpenTK.Input;


namespace AGS.Engine
{
	public class GLInput : IInput
	{
		private GameWindow _game;
		private int _virtualWidth, _virtualHeight;
		private IGameState _state;
		private IAGSRoomTransitions _roomTransitions;

		private IAnimationContainer _mouseCursor;
		private MouseCursor _originalOSCursor;

		public GLInput (GameWindow game, AGS.API.Size virtualResolution, IGameState state, IAGSRoomTransitions roomTransitions)
		{
			this._roomTransitions = roomTransitions;
			this._virtualWidth = virtualResolution.Width;
			this._virtualHeight = virtualResolution.Height;
			this._state = state;
				
			this._game = game;
			this._originalOSCursor = _game.Cursor;

			MouseDown = new AGSEvent<AGS.API.MouseButtonEventArgs> ();
			MouseUp = new AGSEvent<AGS.API.MouseButtonEventArgs> ();
			MouseMove = new AGSEvent<MousePositionEventArgs> ();
			KeyDown = new AGSEvent<KeyboardEventArgs> ();
			KeyUp = new AGSEvent<KeyboardEventArgs> ();

			game.MouseDown += async (sender, e) => 
			{ 
				if (isInputBlocked()) return;
				var button = convert(e.Button);
				if (button == AGS.API.MouseButton.Left) LeftMouseButtonDown = true;
				else if (button == AGS.API.MouseButton.Right) RightMouseButtonDown = true;
				await MouseDown.InvokeAsync(sender, new AGS.API.MouseButtonEventArgs(button, convertX(e.X), convertY(e.Y)));
			};
			game.MouseUp += async (sender, e) => 
			{
				if (isInputBlocked()) return;
				var button = convert(e.Button);
				if (button == AGS.API.MouseButton.Left) LeftMouseButtonDown = false;
				else if (button == AGS.API.MouseButton.Right) RightMouseButtonDown = false;
				await MouseUp.InvokeAsync(sender, new AGS.API.MouseButtonEventArgs(button, convertX(e.X), convertY(e.Y)));
			};
			game.MouseMove += async (sender, e) =>
			{
				if (isInputBlocked()) return;
				await MouseMove.InvokeAsync(sender, new MousePositionEventArgs (convertX(e.X), convertY(e.Y)));
			};
			game.KeyDown += async (sender, e) =>
			{
				if (isInputBlocked()) return;
				await KeyDown.InvokeAsync(sender, new KeyboardEventArgs (convert(e.Key)));
			};
			game.KeyUp += async (sender, e) =>
			{
				if (isInputBlocked()) return;
				await KeyUp.InvokeAsync(sender, new KeyboardEventArgs (convert(e.Key)));
			};
		}

		#region IInputEvents implementation

		public IEvent<AGS.API.MouseButtonEventArgs> MouseDown { get; private set; }

		public IEvent<AGS.API.MouseButtonEventArgs> MouseUp { get; private set; }

		public IEvent<MousePositionEventArgs> MouseMove { get; private set; }

		public IEvent<KeyboardEventArgs> KeyDown { get; private set; }

		public IEvent<KeyboardEventArgs> KeyUp { get; private set; }

		#endregion

		public PointF MousePosition
		{
			get 
			{
				return new PointF(MouseX, MouseY);
			}
		}

		public bool LeftMouseButtonDown { get; private set; }
		public bool RightMouseButtonDown { get; private set; }

		//For some reason GameWindow.Mouse is obsolete.
		//From the warning it should be replaced by Input.Mouse which returns screen coordinates
		//and not window coordinates. Changing will require us to gather the screen monitor coordinates
		//and take multiple monitor issues into account, so for now we'll stick with the obsolete GameWindow.Mouse
		//in the hope that future versions will keep it alive.
		#pragma warning disable 618
		public float MouseX { get { return convertX(_game.Mouse.X); } }
		public float MouseY { get { return convertY(_game.Mouse.Y); } }
		#pragma warning restore 618

		public IAnimationContainer Cursor
		{ 
			get { return _mouseCursor; } 
		  	set 
		  	{ 
				_mouseCursor = value;
				_game.Cursor = _mouseCursor == null ? _originalOSCursor : MouseCursor.Empty;
		  	}
		}

		private bool isInputBlocked()
		{
			if (_roomTransitions.State != RoomTransitionState.NotInTransition) return true;
			return false;
		}

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

		private AGS.API.Key convert(OpenTK.Input.Key key)
		{
			return (AGS.API.Key)(int)key;
		}

		private float convertX(float x)
		{
			var viewport = getViewport();
			var virtualWidth = _virtualWidth / viewport.ScaleX;
			x = MathUtils.Lerp (0f, 0f, Hooks.GameWindowSize.GetWidth(_game), virtualWidth, x);
			return x + viewport.X;
		}

		private float convertY(float y)
		{
			var viewport = getViewport();
			var virtualHeight = _virtualHeight / viewport.ScaleY;
			y = MathUtils.Lerp (0f, virtualHeight, Hooks.GameWindowSize.GetHeight(_game), 0f, y);
			return y + viewport.Y;
		}

		private IViewport getViewport()
		{
			return _state.Player.Character.Room.Viewport;
		}
	}
}

