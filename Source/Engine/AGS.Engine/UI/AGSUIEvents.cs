using System;
using AGS.API;
using System.Threading.Tasks;
using System.Diagnostics;

namespace AGS.Engine
{
	public class AGSUIEvents : AGSComponent, IUIEvents
	{
		private readonly IInput _input;
		private readonly IGameState _state;
		private readonly IGameEvents _gameEvents;
		private bool _leftMouseDown, _rightMouseDown;
		private float _mouseX, _mouseY;
		private Stopwatch _leftMouseClickTimer, _rightMouseClickTimer, _leftMouseDoubleClickTimer, _rightMouseDoubleClickTimer;
        private bool _isFocused;

		private IEnabledComponent _enabled;
		private IVisibleComponent _visible;
        private IEntity _entity;

		public AGSUIEvents(IInput input, IGameState state, IGameEvents gameEvents)
		{
			_input = input;
			_state = state;
			_gameEvents = gameEvents;

			MouseEnter = new AGSEvent<MousePositionEventArgs> ();
			MouseLeave = new AGSEvent<MousePositionEventArgs> ();
			MouseMove = new AGSEvent<MousePositionEventArgs> ();
			MouseClicked = new AGSEvent<MouseButtonEventArgs> ();
            MouseDoubleClicked = new AGSEvent<MouseButtonEventArgs>();
            MouseDown = new AGSEvent<MouseButtonEventArgs> ();
			MouseUp = new AGSEvent<MouseButtonEventArgs> ();
            LostFocus = new AGSEvent<MouseButtonEventArgs>();

			_leftMouseClickTimer = new Stopwatch ();
			_rightMouseClickTimer = new Stopwatch ();
            _leftMouseDoubleClickTimer = new Stopwatch();
            _rightMouseDoubleClickTimer = new Stopwatch();
        }

		public override void Init(IEntity entity)
		{
			base.Init(entity);
            _entity = entity;
			_enabled = entity.GetComponent<IEnabledComponent>();
			_visible = entity.GetComponent<IVisibleComponent>();
			_gameEvents.OnRepeatedlyExecute.SubscribeToAsync(onRepeatedlyExecute);
		}

		public IEvent<MousePositionEventArgs> MouseEnter { get; private set; }

		public IEvent<MousePositionEventArgs> MouseLeave { get; private set; }

		public IEvent<MousePositionEventArgs> MouseMove { get; private set; }

		public IEvent<MouseButtonEventArgs> MouseClicked { get; private set; }

        public IEvent<MouseButtonEventArgs> MouseDoubleClicked { get; private set; }

        public IEvent<MouseButtonEventArgs> MouseDown { get; private set; }

		public IEvent<MouseButtonEventArgs> MouseUp { get; private set; }

        public IEvent<MouseButtonEventArgs> LostFocus { get; private set; }

        public bool IsMouseIn { get; private set; }

		public override void Dispose()
		{
			base.Dispose();
			_gameEvents.OnRepeatedlyExecute.UnsubscribeToAsync(onRepeatedlyExecute);
		}

		private async Task onRepeatedlyExecute(object sender, EventArgs args)
		{
			if (!_enabled.Enabled || !_visible.Visible) return;
            if (_state.Player.Character == null) return;
            IRoom room = _state.Player.Character.Room;
            if (room == null) return;

			PointF position = _input.MousePosition;

            var obj = room.GetObjectAt(position.X, position.Y);
            bool mouseIn = obj == _entity;

			bool leftMouseDown = _input.LeftMouseButtonDown;
			bool rightMouseDown = _input.RightMouseButtonDown;

			bool fireMouseMove = mouseIn && (_mouseX != position.X || _mouseY != position.Y);
			bool fireMouseEnter = mouseIn && !IsMouseIn;
			bool fireMouseLeave = !mouseIn && IsMouseIn;

			_mouseX = position.X;
			_mouseY = position.Y;
			IsMouseIn = mouseIn;

            bool wasLeftMouseDown = _leftMouseDown;
            bool wasRightMouseDown = _rightMouseDown;
            _leftMouseDown = leftMouseDown;
            _rightMouseDown = rightMouseDown;

            await handleMouseButton(_leftMouseClickTimer, _leftMouseDoubleClickTimer, wasLeftMouseDown, leftMouseDown, MouseButton.Left);
			await handleMouseButton(_rightMouseClickTimer, _rightMouseDoubleClickTimer, wasRightMouseDown, rightMouseDown, MouseButton.Right);

			if (fireMouseEnter) await MouseEnter.InvokeAsync(_entity, new MousePositionEventArgs (position.X, position.Y));
			else if (fireMouseLeave) await MouseLeave.InvokeAsync(_entity, new MousePositionEventArgs (position.X, position.Y));
			if (fireMouseMove) await MouseMove.InvokeAsync(_entity, new MousePositionEventArgs(position.X, position.Y));
		}

		private async Task handleMouseButton(Stopwatch sw, Stopwatch doubleClickSw, bool wasDown, bool isDown, MouseButton button)
		{
            bool fireDown = !wasDown && isDown && IsMouseIn;
            bool fireDownOutside = !wasDown && isDown && !IsMouseIn && _isFocused;
            _isFocused = fireDown;
			bool fireUp = wasDown && !isDown;
			if (fireDown)
			{
				sw.Restart();
			}
			bool fireClick = false;
            bool fireDoubleClick = false;
			if (fireUp)
			{
				if (IsMouseIn && sw.ElapsedMilliseconds < 1500 && sw.ElapsedMilliseconds != 0)
				{                    
					fireClick = true;
                    if (doubleClickSw.ElapsedMilliseconds == 0)
                    {
                        doubleClickSw.Restart();
                    }
                    else
                    {
                        if (doubleClickSw.ElapsedMilliseconds < 1500)
                        {
                            fireDoubleClick = true;
                        }
                        doubleClickSw.Stop();
                        doubleClickSw.Reset();
                    }                     
				}
				sw.Stop();
				sw.Reset();
			}

            if (fireDown || fireUp || fireClick || fireDownOutside)
			{
                MouseButtonEventArgs args = new MouseButtonEventArgs (button, _mouseX, _mouseY);
                if (fireDown) await MouseDown.InvokeAsync(_entity, args);
                else if (fireUp) await MouseUp.InvokeAsync(_entity, args);
                else if (fireDownOutside) await LostFocus.InvokeAsync(_entity, args);
				if (fireClick) await MouseClicked.InvokeAsync(_entity, args);
                if (fireDoubleClick) await MouseDoubleClicked.InvokeAsync(_entity, args);
            }
		}
	}
}

