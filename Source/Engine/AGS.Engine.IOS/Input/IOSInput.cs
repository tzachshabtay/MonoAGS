extern alias IOS;

using System;
using System.Threading.Tasks;
using AGS.API;
using IOS::UIKit;

namespace AGS.Engine.IOS
{
    public class IOSInput : IInput
    {
        private IGameWindow _gameWindow;
        private IGameState _state;
        private IShouldBlockInput _shouldBlockInput;
        private DateTime _lastDrag;

        public IOSInput(IOSGestures gestures, AGS.API.Size virtualResolution,
                        IGameState state, IShouldBlockInput shouldBlockInput, IGameWindow gameWindow)
        {
            MousePosition = new MousePosition(0f, 0f, state.Viewport);
            _shouldBlockInput = shouldBlockInput;
            _gameWindow = gameWindow;
            _state = state;
			API.MousePosition.VirtualResolution = virtualResolution;
            updateWindowSizeFunctions();

			MouseDown = new AGSEvent<AGS.API.MouseButtonEventArgs>();
            MouseUp = new AGSEvent<AGS.API.MouseButtonEventArgs>();
            MouseMove = new AGSEvent<MousePositionEventArgs>();
            KeyDown = new AGSEvent<KeyboardEventArgs>();
            KeyUp = new AGSEvent<KeyboardEventArgs>();

            IOSGameWindow.Instance.View.OnInsertText += onInsertText;
            IOSGameWindow.Instance.View.OnDeleteBackward += onDeleteBackwards;

            gestures.OnUserDrag += async (sender, e) =>
            {
                if (isInputBlocked()) return;
                DateTime now = DateTime.Now;
                _lastDrag = now;
                IsTouchDrag = true;
                setMousePosition(e);
                await MouseMove.InvokeAsync(new MousePositionEventArgs(MousePosition));
                await Task.Delay(300);
                if (_lastDrag <= now) IsTouchDrag = false;
            };
            gestures.OnUserSingleTap += async (sender, e) =>
            {
                if (isInputBlocked()) return;
                setMousePosition(e);
                LeftMouseButtonDown = true;
                await MouseDown.InvokeAsync(new MouseButtonEventArgs(null, MouseButton.Left, MousePosition));
                await Task.Delay(250);
                await MouseUp.InvokeAsync(new MouseButtonEventArgs(null, MouseButton.Left, MousePosition));
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

        public MousePosition MousePosition { get; private set; }

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
            float x = convertX(e.MousePosition.XWindow);
            float y = convertY(e.MousePosition.YWindow);
            updateWindowSizeFunctions();
            MousePosition = new MousePosition(x, y, _state.Viewport);
        }

        private void updateWindowSizeFunctions()
        {
			//we have to statically evaluate the window size, because _gameWindow.Width & Height must be called from the UI thread.
            float density = (float)UIScreen.MainScreen.Scale;
			int windowWidth = _gameWindow.Width;
            int windowHeight = _gameWindow.Height;

            API.MousePosition.GetWindowWidth = () => (int)((windowWidth - (GLUtils.ScreenViewport.X * 2)) / density);
            API.MousePosition.GetWindowHeight = () => (int)((windowHeight - (GLUtils.ScreenViewport.Y * 2)) / density);
        }

        private float convertX(float x)
        {
            float density = (float)UIScreen.MainScreen.Scale;
            x = x - (GLUtils.ScreenViewport.X / density);
            return x;
        }

        private float convertY(float y)
        {
            float density = (float)UIScreen.MainScreen.Scale;
            y = y - (GLUtils.ScreenViewport.Y / density);
            return y;
        }

        private async void onInsertText(object sender, string text)
        {
            foreach (char c in text)
            {
                bool isShift;
                Key key = mapKey(c, out isShift);
                await fireKeyPress(key, isShift);
            }
        }

        private async void onDeleteBackwards(object sender, EventArgs args)
        {
            await fireKeyPress(Key.BackSpace, false);
        }

        private async Task fireKeyPress(Key key, bool isShift)
        {
            var keyDown = KeyDown;
            var keyUp = KeyUp;
            if (keyDown != null)
            {
                if (isShift) await keyDown.InvokeAsync(new KeyboardEventArgs(Key.ShiftLeft));
                await keyDown.InvokeAsync(new KeyboardEventArgs(key));
            }
            await Task.Delay(5);
            if (keyUp != null)
            {
                await keyUp.InvokeAsync(new KeyboardEventArgs(key));
                if (isShift) await keyUp.InvokeAsync(new KeyboardEventArgs(Key.ShiftLeft));
            }
        }

        private Key mapKey(char c, out bool isShift)
        {
            isShift = false;
            if (c >= 'a' && c <= 'z')
            {
                return c - 'a' + Key.A;
            }
            if (c >= 'A' && c <= 'Z')
            {
                isShift = true;
                return c - 'A' + Key.A;
            }
            if (c >= '0' && c <= '9')
            {
                return c - '0' + Key.Number0;
            }
            switch (c)
            {
                case '!': return withShift(Key.Number1, out isShift);
                case '@': return withShift(Key.Number2, out isShift);
                case '#': return withShift(Key.Number3, out isShift);
                case '$': return withShift(Key.Number4, out isShift);
                case '%': return withShift(Key.Number5, out isShift);
                case '^': return withShift(Key.Number6, out isShift);
                case '&': return withShift(Key.Number7, out isShift);
                case '*': return withShift(Key.Number8, out isShift);
                case '(': return withShift(Key.Number9, out isShift);
                case ')': return withShift(Key.Number0, out isShift);
                case '-': return Key.Minus;
                case '_': return withShift(Key.Minus, out isShift);
                case '=': return Key.Plus;
                case '+': return withShift(Key.Plus, out isShift);
                case '`': return Key.Tilde;
                case '~': return withShift(Key.Tilde, out isShift);
                case '[': return Key.BracketLeft;
                case '{': return withShift(Key.BracketLeft, out isShift);
                case ']': return Key.BracketRight;
                case '}': return withShift(Key.BracketRight, out isShift);
                case '\\': return Key.BackSlash;
                case '|': return withShift(Key.BackSlash, out isShift);
                case ';': return Key.Semicolon;
                case ':': return withShift(Key.Semicolon, out isShift);
                case '\'': return Key.Quote;
                case '"': return withShift(Key.Quote, out isShift);
                case ',': return Key.Comma;
                case '<': return withShift(Key.Comma, out isShift);
                case '.': return Key.Period;
                case '>': return withShift(Key.Period, out isShift);
                case '/': return Key.Slash;
                case '?': return withShift(Key.Slash, out isShift);
                case ' ': return Key.Space;
                default: return withShift(Key.Slash, out isShift);
            }
        }

        private Key withShift(Key key, out bool isShift)
        {
            isShift = true;
            return key;
        }
    }
}
