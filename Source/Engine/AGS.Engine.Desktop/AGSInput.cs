using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using AGS.API;
using Silk.NET.Windowing.Common;
using Silk.NET.Input.Common;
using Silk.NET.Input;

namespace AGS.Engine.Desktop
{
    public class AGSInput : IInput
    {
        private IWindow _game;
        private float _mouseX, _mouseY;
        private readonly IShouldBlockInput _shouldBlockInput;
        private readonly IConcurrentHashSet<API.Key> _keysDown;
        private readonly IGameEvents _events;
        private readonly ICoordinates _coordinates;
        private readonly IAGSCursor _cursor;
        private readonly IAGSHitTest _hitTest;

        private IInputContext _input;
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

        private void init(IWindow game)
        {
            if (_game != null) return;
            _game = game;
            _input = game.CreateInput();
            foreach (IMouse mouse in _input.Mice)
            {
                _cursor.PropertyChanged += (sender, e) =>
                {
                    if (_cursor.Cursor != null) mouse.Cursor.CursorMode = CursorMode.Hidden;
                };
                mouse.MouseDown += (_, button) =>
                {
                    if (isInputBlocked())
                        return;
                    var apiButton = convert(button);
                    _actions.Enqueue(() => MouseDown.InvokeAsync(new API.MouseButtonEventArgs(_hitTest.ObjectAtMousePosition, apiButton, MousePosition)));
                };
                mouse.MouseUp += (_, button) =>
                {
                    if (isInputBlocked())
                        return;
                    var apiButton = convert(button);
                    _actions.Enqueue(() => MouseUp.InvokeAsync(new API.MouseButtonEventArgs(_hitTest.ObjectAtMousePosition, apiButton, MousePosition)));
                };
                mouse.MouseMove += (_, point) =>
                {
                    _mouseX = point.X;
                    _mouseY = point.Y;
                    _actions.Enqueue(() => MouseMove.InvokeAsync(new MousePositionEventArgs(MousePosition)));
                };
            }
            foreach (IKeyboard keyboard in _input.Keyboards)
            {
                keyboard.KeyDown += (_, key, __) =>
                {
                    API.Key apiKey = convert(key);
                    _keysDown.Add(apiKey);
                    if (isInputBlocked())
                        return;
                    _actions.Enqueue(() => KeyDown.InvokeAsync(new KeyboardEventArgs(apiKey)));
                };
                keyboard.KeyUp += (_, key, __) =>
                {
                    API.Key apiKey = convert(key);
                    _keysDown.Remove(apiKey);
                    if (isInputBlocked())
                        return;
                    _actions.Enqueue(() => KeyUp.InvokeAsync(new KeyboardEventArgs(apiKey)));
                };
            }

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
            get => _input.Mice.Any(m => m.Cursor.CursorMode != CursorMode.Hidden);
            set
            {
                foreach (IMouse mouse in _input.Mice)
                {
                    mouse.Cursor.CursorMode = value ? CursorMode.Normal : CursorMode.Hidden;
                }
            }
        }

        private bool isInputBlocked() => _shouldBlockInput.ShouldBlockInput();

        private API.MouseButton convert(Silk.NET.Input.Common.MouseButton button)
        {
            switch (button)
            {
                case Silk.NET.Input.Common.MouseButton.Left:
                    return API.MouseButton.Left;
                case Silk.NET.Input.Common.MouseButton.Right:
                    return API.MouseButton.Right;
                case Silk.NET.Input.Common.MouseButton.Middle:
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
                bool leftDown = false;
                bool rightDown = false;
                foreach (IMouse mouse in _input.Mice)
                {
                    if (mouse.IsButtonPressed(Silk.NET.Input.Common.MouseButton.Left)) leftDown = true;
                    if (mouse.IsButtonPressed(Silk.NET.Input.Common.MouseButton.Right)) rightDown = true;
                    if (leftDown && rightDown) break;
                }
                LeftMouseButtonDown = leftDown;
                RightMouseButtonDown = rightDown;
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

        private API.Key convert(Silk.NET.Input.Common.Key key) => (API.Key)(int)key;
    }
}
