using System;
using API;
using OpenTK;
using OpenTK.Input;

namespace Engine
{
	public class GLInputEvents : IInputEvents
	{
		private GameWindow game;
		private int viewportX, viewportY;

		public GLInputEvents (GameWindow game)
		{
			this.viewportX = game.Bounds.Width;
			this.viewportY = game.Bounds.Height;
				;
			this.game = game;

			MouseDown = new AGSEvent<API.MouseButtonEventArgs> ();
			MouseUp = new AGSEvent<API.MouseButtonEventArgs> ();
			MouseMove = new AGSEvent<MousePositionEventArgs> ();
			KeyDown = new AGSEvent<KeyboardEventArgs> ();
			KeyUp = new AGSEvent<KeyboardEventArgs> ();

			game.MouseDown += async (sender, e) => await MouseDown.InvokeAsync(sender, new API.MouseButtonEventArgs(convert(e.Button), convertX(e.X), convertY(e.Y)));
			game.MouseUp += async (sender, e) => await MouseUp.InvokeAsync(sender, new API.MouseButtonEventArgs(convert(e.Button), convertX(e.X), convertY(e.Y)));
			game.MouseMove += async (sender, e) => await MouseMove.InvokeAsync(sender, new MousePositionEventArgs(convertX(e.X), convertY(e.Y)));
			game.KeyDown += async (sender, e) => await KeyDown.InvokeAsync(sender, new KeyboardEventArgs(convert(e.Key)));
			game.KeyUp += async (sender, e) => await KeyUp.InvokeAsync(sender, new KeyboardEventArgs(convert(e.Key)));
		}

		#region IInputEvents implementation

		public IEvent<API.MouseButtonEventArgs> MouseDown { get; private set; }

		public IEvent<API.MouseButtonEventArgs> MouseUp { get; private set; }

		public IEvent<MousePositionEventArgs> MouseMove { get; private set; }

		public IEvent<KeyboardEventArgs> KeyDown { get; private set; }

		public IEvent<KeyboardEventArgs> KeyUp { get; private set; }

		#endregion

		private API.MouseButton convert(OpenTK.Input.MouseButton button)
		{
			switch (button) {
			case OpenTK.Input.MouseButton.Left:
				return API.MouseButton.Left;
			case OpenTK.Input.MouseButton.Right:
				return API.MouseButton.Right;
			case OpenTK.Input.MouseButton.Middle:
				return API.MouseButton.Middle;
			default:
				throw new NotSupportedException ();
			}
		}

		private API.Key convert(OpenTK.Input.Key key)
		{
			return (API.Key)(int)key;
		}

		private float convertX(float x)
		{
			x = MathUtils.Lerp (0f, 0f, game.ClientSize.Width, viewportX, x);
			return x;
		}

		private float convertY(float y)
		{
			y = MathUtils.Lerp (0f, viewportY, game.ClientSize.Height, 0f, y);
			return y;
		}
	}
}

