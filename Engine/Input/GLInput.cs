using System;
using AGS.API;
using OpenTK;
using OpenTK.Input;
using System.Drawing;

namespace AGS.Engine
{
	public class GLInput : IInput
	{
		private GameWindow _game;
		private int _virtualWidth, _virtualHeight;
		private IGameState _state;

		public GLInput (GameWindow game, Size virtualResolution, IGameState state)
		{
			this._virtualWidth = virtualResolution.Width;
			this._virtualHeight = virtualResolution.Height;
			this._state = state;
				
			this._game = game;

			MouseDown = new AGSEvent<AGS.API.MouseButtonEventArgs> ();
			MouseUp = new AGSEvent<AGS.API.MouseButtonEventArgs> ();
			MouseMove = new AGSEvent<MousePositionEventArgs> ();
			KeyDown = new AGSEvent<KeyboardEventArgs> ();
			KeyUp = new AGSEvent<KeyboardEventArgs> ();

			game.MouseDown += async (sender, e) => await MouseDown.InvokeAsync(sender, new AGS.API.MouseButtonEventArgs(convert(e.Button), convertX(e.X), convertY(e.Y)));
			game.MouseUp += async (sender, e) => await MouseUp.InvokeAsync(sender, new AGS.API.MouseButtonEventArgs(convert(e.Button), convertX(e.X), convertY(e.Y)));
			game.MouseMove += async (sender, e) => await MouseMove.InvokeAsync(sender, new MousePositionEventArgs(convertX(e.X), convertY(e.Y)));
			game.KeyDown += async (sender, e) => await KeyDown.InvokeAsync(sender, new KeyboardEventArgs(convert(e.Key)));
			game.KeyUp += async (sender, e) => await KeyUp.InvokeAsync(sender, new KeyboardEventArgs(convert(e.Key)));
		}

		#region IInputEvents implementation

		public IEvent<AGS.API.MouseButtonEventArgs> MouseDown { get; private set; }

		public IEvent<AGS.API.MouseButtonEventArgs> MouseUp { get; private set; }

		public IEvent<MousePositionEventArgs> MouseMove { get; private set; }

		public IEvent<KeyboardEventArgs> KeyDown { get; private set; }

		public IEvent<KeyboardEventArgs> KeyUp { get; private set; }

		#endregion

		public IPoint MousePosition
		{
			get 
			{
				return new AGSPoint(MouseX, MouseY);
			}
		}

		//For some reason GameWindow.Mouse is obsolete.
		//From the warning it should be replaced by Input.Mouse which returns screen coordinates
		//and not window coordinates. Changing will require us to gather the screen monitor coordinates
		//and take multiple monitor issues into account, so for now we'll stick with the obsolete GameWindow.Mouse
		//in the hope that future versions will keep it alive.
		#pragma warning disable 618
		public float MouseX { get { return convertX(_game.Mouse.X); } }
		public float MouseY { get { return convertY(_game.Mouse.Y); } }
		#pragma warning restore 618

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
			x = MathUtils.Lerp (0f, 0f, _game.ClientSize.Width, _virtualWidth, x);
			return x + getViewport().X;
		}

		private float convertY(float y)
		{
			y = MathUtils.Lerp (0f, _virtualHeight, _game.ClientSize.Height, 0f, y);
			return y + getViewport().Y;
		}

		private IViewport getViewport()
		{
			return _state.Player.Character.Room.Viewport;
		}
	}
}

