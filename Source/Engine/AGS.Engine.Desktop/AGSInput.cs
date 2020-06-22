using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using AGS.API;
using OpenToolkit.Windowing.Common;
using OpenToolkit.Windowing.Common.Input;
using OpenToolkit.Windowing.Desktop;

namespace AGS.Engine.Desktop
{
    public class AGSInput : IInput
    {
        private GameWindow _game;
        private float _mouseX, _mouseY;
        private bool _isLeftMouseButtonDown, _isRightMouseButtonDown;
        private readonly IShouldBlockInput _shouldBlockInput;
        private readonly IConcurrentHashSet<API.Key> _keysDown;
        private readonly IGameEvents _events;
        private readonly ICoordinates _coordinates;
        private readonly IAGSCursor _cursor;
        private readonly IAGSHitTest _hitTest;

        private MouseCursor _originalOSCursor;
        private readonly ConcurrentQueue<Func<Task>> _actions;
        private int _inUpdate; //For preventing re-entrancy

        public AGSInput(IGameEvents events, IShouldBlockInput shouldBlockInput, IAGSCursor cursor, IAGSHitTest hitTest,
                        IEvent<API.MouseButtonEventArgs> mouseDown, 
                        IEvent<API.MouseButtonEventArgs> mouseUp, IEvent<MousePositionEventArgs> mouseMove,
                        IEvent<KeyboardEventArgs> keyDown, IEvent<KeyboardEventArgs> keyUp, ICoordinates coordinates)
        {
            _cursor = cursor;
            _events = events;
            _hitTest = hitTest;
            _actions = new ConcurrentQueue<Func<Task>>();
            _coordinates = coordinates;
            _shouldBlockInput = shouldBlockInput;
            _keysDown = new AGSConcurrentHashSet<API.Key>();

            MouseDown = mouseDown;
            MouseUp = mouseUp;
            MouseMove = mouseMove;
            KeyDown = keyDown;
            KeyUp = keyUp;

            if (AGSGameWindow.GameWindow != null) init(AGSGameWindow.GameWindow);
            else AGSGameWindow.OnInit = () => init(AGSGameWindow.GameWindow);
        }

        private void init(GameWindow game)
        {
            if (_game != null) return;
            _game = game;
            _originalOSCursor = game.Cursor;

            _cursor.PropertyChanged += (sender, e) =>
            {
                if (_cursor.Cursor != null) _game.Cursor = MouseCursor.Empty;
            };
            game.MouseDown += (e) =>
            {
                _isLeftMouseButtonDown = e.Button == OpenToolkit.Windowing.Common.Input.MouseButton.Left;
                _isRightMouseButtonDown = e.Button == OpenToolkit.Windowing.Common.Input.MouseButton.Right;
                if (isInputBlocked()) return;
                var button = convert(e.Button);
                _actions.Enqueue(() => MouseDown.InvokeAsync(new API.MouseButtonEventArgs(_hitTest.ObjectAtMousePosition, button, MousePosition)));
            };
            game.MouseUp += (e) =>
            {
                if (e.Button == OpenToolkit.Windowing.Common.Input.MouseButton.Left) _isLeftMouseButtonDown = false;
                if (e.Button == OpenToolkit.Windowing.Common.Input.MouseButton.Right) _isRightMouseButtonDown = false;
                if (isInputBlocked()) return;
                var button = convert(e.Button);
                _actions.Enqueue(() => MouseUp.InvokeAsync(new API.MouseButtonEventArgs(_hitTest.ObjectAtMousePosition, button, MousePosition)));
            };
            game.MouseMove += (e) =>
            {
                _mouseX = e.X;
                _mouseY = e.Y;
                _actions.Enqueue(() => MouseMove.InvokeAsync(new MousePositionEventArgs(MousePosition)));
            };
            game.KeyDown += (e) =>
            {
                API.Key key = convert(e.Key);
                _keysDown.Add(key);
                if (isInputBlocked()) return;
                _actions.Enqueue(() => KeyDown.InvokeAsync(new KeyboardEventArgs(key)));
            };
            game.KeyUp += (e) =>
            {
                API.Key key = convert(e.Key);
                _keysDown.Remove(key);
                if (isInputBlocked()) return;
                _actions.Enqueue(() => KeyUp.InvokeAsync(new KeyboardEventArgs(key)));
            };

            _events.OnRepeatedlyExecuteAlways.Subscribe(onRepeatedlyExecute);
        }

        #region IInputEvents implementation

        public IEvent<API.MouseButtonEventArgs> MouseDown { get; private set; }

        public IEvent<API.MouseButtonEventArgs> MouseUp { get; private set; }

        public IEvent<MousePositionEventArgs> MouseMove { get; private set; }

        public IEvent<KeyboardEventArgs> KeyDown { get; private set; }

        public IEvent<KeyboardEventArgs> KeyUp { get; private set; }

        #endregion

        public bool IsKeyDown(API.Key key) => _keysDown.Contains(key);

        public MousePosition MousePosition => new MousePosition(_mouseX, _mouseY, _coordinates);

        public bool LeftMouseButtonDown { get; private set; }
        public bool RightMouseButtonDown { get; private set; }
        public bool IsTouchDrag => false;  //todo: support touch screens on desktops

        public bool ShowHardwareCursor
        {
            get => _game.Cursor != MouseCursor.Empty;
            set => _game.Cursor = value ? _originalOSCursor : MouseCursor.Empty;
        }

        private bool isInputBlocked() => _shouldBlockInput.ShouldBlockInput();

        private API.MouseButton convert(OpenToolkit.Windowing.Common.Input.MouseButton button)
        {
            switch (button)
            {
                case OpenToolkit.Windowing.Common.Input.MouseButton.Left:
                    return API.MouseButton.Left;
                case OpenToolkit.Windowing.Common.Input.MouseButton.Right:
                    return API.MouseButton.Right;
                case OpenToolkit.Windowing.Common.Input.MouseButton.Middle:
                    return API.MouseButton.Middle;
                default:
                    throw new NotSupportedException();
            }
        }

        private void onRepeatedlyExecute()
        {
            _hitTest.Refresh(MousePosition);
            if (!isInputBlocked())
            {
                LeftMouseButtonDown = _isLeftMouseButtonDown;
                RightMouseButtonDown = _isRightMouseButtonDown;
            }

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

        private API.Key convert(OpenToolkit.Windowing.Common.Input.Key key) => (API.Key)(int)key;
    }
}
