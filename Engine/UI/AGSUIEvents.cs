using System;
using AGS.API;
using System.Threading.Tasks;
using System.Diagnostics;

namespace AGS.Engine
{
	public class AGSUIEvents : IUIEvents
	{
		private readonly IInput _input;
		private readonly IGameState _state;
		private readonly IObject _obj;
		private readonly IGameEvents _gameEvents;
		private bool _leftMouseDown, _rightMouseDown;
		private float _mouseX, _mouseY;
		private Stopwatch _leftMouseClickTimer, _rightMouseClickTimer;

		public AGSUIEvents(IObject obj, IInput input, IGameState state, IGameEvents gameEvents)
		{
			_obj = obj;
			_input = input;
			_state = state;
			_gameEvents = gameEvents;

			MouseEnter = new AGSEvent<MousePositionEventArgs> ();
			MouseLeave = new AGSEvent<MousePositionEventArgs> ();
			MouseMove = new AGSEvent<MousePositionEventArgs> ();
			MouseClicked = new AGSEvent<MouseButtonEventArgs> ();
			MouseDown = new AGSEvent<MouseButtonEventArgs> ();
			MouseUp = new AGSEvent<MouseButtonEventArgs> ();

			_leftMouseClickTimer = new Stopwatch ();
			_rightMouseClickTimer = new Stopwatch ();
			gameEvents.OnRepeatedlyExecute.SubscribeToAsync(onRepeatedlyExecute);
		}

		public IEvent<MousePositionEventArgs> MouseEnter { get; private set; }

		public IEvent<MousePositionEventArgs> MouseLeave { get; private set; }

		public IEvent<MousePositionEventArgs> MouseMove { get; private set; }

		public IEvent<MouseButtonEventArgs> MouseClicked { get; private set; }

		public IEvent<MouseButtonEventArgs> MouseDown { get; private set; }

		public IEvent<MouseButtonEventArgs> MouseUp { get; private set; }

		public bool IsMouseIn { get; private set; }

		public void Dispose()
		{
			_gameEvents.OnRepeatedlyExecute.UnsubscribeToAsync(onRepeatedlyExecute);
		}

		private async Task onRepeatedlyExecute(object sender, EventArgs args)
		{
			if (!_obj.Enabled || !_obj.Visible) return;
			IPoint position = _input.MousePosition;
			IViewport viewport = _state.Player.Character.Room.Viewport;

			//todo: Support mouseX/Y When IgnoreScalingArea = false (i.e 4 options: IgnoreScaling+IgnoreViewport,IgnoreScaling,IgnoreViewport,None)
			float mouseX = _obj.IgnoreViewport ? (position.X - viewport.X) * viewport.ScaleX + viewport.X : position.X;
			float mouseY = _obj.IgnoreViewport ? (position.Y - viewport.Y) * viewport.ScaleY + viewport.Y : position.Y;
			bool mouseIn = _obj.CollidesWith(mouseX, mouseY);

			bool leftMouseDown = _input.LeftMouseButtonDown;
			bool rightMouseDown = _input.RightMouseButtonDown;

			bool fireMouseMove = mouseIn && (_mouseX != position.X || _mouseY != position.Y);
			bool fireMouseEnter = mouseIn && !IsMouseIn;
			bool fireMouseLeave = !mouseIn && IsMouseIn;

			_mouseX = position.X;
			_mouseY = position.Y;
			IsMouseIn = mouseIn;

			await handleMouseButton(_leftMouseClickTimer, _leftMouseDown, leftMouseDown, MouseButton.Left);
			await handleMouseButton(_rightMouseClickTimer, _rightMouseDown, rightMouseDown, MouseButton.Right);

			_leftMouseDown = leftMouseDown;
			_rightMouseDown = rightMouseDown;

			if (fireMouseEnter) await MouseEnter.InvokeAsync(this, new MousePositionEventArgs (position.X, position.Y));
			else if (fireMouseLeave) await MouseLeave.InvokeAsync(this, new MousePositionEventArgs (position.X, position.Y));
			if (fireMouseMove) await MouseMove.InvokeAsync(this, new MousePositionEventArgs(position.X, position.Y));
		}

		private async Task handleMouseButton(Stopwatch sw, bool wasDown, bool isDown, MouseButton button)
		{
			bool fireDown = !wasDown && isDown && IsMouseIn;
			bool fireUp = wasDown && !isDown;
			if (fireDown)
			{
				sw.Restart();
			}
			bool fireClick = false;
			if (fireUp)
			{
				if (IsMouseIn && sw.ElapsedMilliseconds < 1500 && sw.ElapsedMilliseconds != 0)
				{
					fireClick = true;
				}
				sw.Stop();
				sw.Reset();
			}

			if (fireDown || fireUp || fireClick)
			{
				MouseButtonEventArgs args = new MouseButtonEventArgs (button, _mouseX, _mouseY);
				if (fireDown) await MouseDown.InvokeAsync(this, args);
				else if (fireUp) await MouseUp.InvokeAsync(this, args);
				if (fireClick) await MouseClicked.InvokeAsync(this, args);
			}
		}
	}
}

