using AGS.API;
using System.Diagnostics;

namespace AGS.Engine
{
    public class AGSTextBoxComponent : AGSComponent, ITextBoxComponent
    {
        private bool _isFocused;
        private ITextComponent _textComponent;
        private IImageComponent _imageComponent;
        private IVisibleComponent _visibleComponent;
        private IUIEvents _uiEvents;        
        private IInObjectTree _tree;
        private IHasRoom _room;
        private readonly IKeyboardState _keyboardState;
        private readonly IGame _game;
        private readonly IFocusedUI _focusedUi;
        private int _caretFlashCounter;

        private int _endOfLine { get { return _textComponent.Text.Length; } }
        private bool _capslock {  get { return _keyboardState.CapslockOn; } }
        private bool _leftShiftOn, _rightShiftOn;
        private bool _shiftOn { get { return _leftShiftOn || _rightShiftOn || (_capslock && _keyboardState.SoftKeyboardVisible); } }

        private ILabel _withCaret;

        public AGSTextBoxComponent(IEvent onFocusChanged, IEvent<TextBoxKeyPressingEventArgs> onPressingKey,
                                   IInput input, IGame game, IKeyboardState keyboardState, IFocusedUI focusedUi)
        {
            CaretFlashDelay = 10;
            _keyboardState = keyboardState;
            _focusedUi = focusedUi;
            OnFocusChanged = onFocusChanged;
            OnPressingKey = onPressingKey;
            _game = game;
            
            input.KeyDown.Subscribe(onKeyDown);
            input.KeyUp.Subscribe(onKeyUp);                        
        }

        public override void Init(IEntity entity)
        {
            base.Init(entity);
            entity.Bind<ITextComponent>(c => _textComponent = c, _ => _textComponent = null);
            entity.Bind<IImageComponent>(c => _imageComponent = c, _ => _imageComponent = null);
            entity.Bind<IUIEvents>(c =>
            {
                _uiEvents = c;
                c.MouseDown.Subscribe(onMouseDown);
                c.LostFocus.Subscribe(onMouseDownOutside);
            }, c =>
            {
                _uiEvents = null;
				c.MouseDown.Unsubscribe(onMouseDown);
				c.LostFocus.Unsubscribe(onMouseDownOutside);
            });
            entity.Bind<IInObjectTree>(c => _tree = c, _ => _tree = null);
            entity.Bind<IHasRoom>(c => _room = c, _ => _room = null);
            entity.Bind<IVisibleComponent>(c => _visibleComponent = c, _ => _visibleComponent = null);

            _game.Events.OnRepeatedlyExecute.Subscribe(onRepeatedlyExecute);

            _caretFlashCounter = (int)CaretFlashDelay;
            _withCaret = _game.Factory.UI.GetLabel(entity.ID + " Caret", "|", 1f, 1f, 0f, 0f, config: new AGSTextConfig(autoFit: AutoFit.LabelShouldFitText));
            _withCaret.Anchor = new PointF(0f, 0f);
        }

        public override void AfterInit()
        {
            base.AfterInit();
            _game.Events.OnBeforeRender.Subscribe(onBeforeRender);
        }

        public bool IsFocused
        {
            get { return _isFocused; }
            set
            {
                if (_isFocused == value) return;
                _isFocused = value;
                if (_isFocused)
                {
                    _keyboardState.OnSoftKeyboardHidden.Subscribe(onSoftKeyboardHidden);
                    _keyboardState.ShowSoftKeyboard();
                    var textComponent = _textComponent;
                    CaretPosition = textComponent == null ? 0 : textComponent.Text.Length;
                    _focusedUi.FocusedTextBox = this;
                }
                else
                {
                    _keyboardState.HideSoftKeyboard();
                    if (_focusedUi.FocusedTextBox == this)
                        _focusedUi.FocusedTextBox = null;
                }
                OnFocusChanged.Invoke();
            }
        }

        public int CaretPosition { get; set; }
        public uint CaretFlashDelay { get; set; }

        public IEvent OnFocusChanged { get; private set; }

        public IEvent<TextBoxKeyPressingEventArgs> OnPressingKey { get; private set; }

        public override void Dispose()
        {
            base.Dispose();
            _game.Events.OnRepeatedlyExecute.Unsubscribe(onRepeatedlyExecute);
            _game.Events.OnBeforeRender.Unsubscribe(onBeforeRender);
        }

        private void onRepeatedlyExecute()
        {
            var visible = _visibleComponent;
            if (visible == null || !visible.Visible) IsFocused = false;
        }

        private void onSoftKeyboardHidden()
        {
            _keyboardState.OnSoftKeyboardHidden.Unsubscribe(onSoftKeyboardHidden);
            IsFocused = false;
        }

        private void onMouseDown(MouseButtonEventArgs args)
        {
            Debug.WriteLine(string.Format("{0} is focused", _textComponent.Text));
            IsFocused = true;
        }

        private void onMouseDownOutside(MouseButtonEventArgs args)
        {
            IsFocused = false;
        }

        private void onBeforeRender()
        {
            if (_textComponent == null) return;
            if (_room.Room != null && _room.Room != _game.State.Room) return;
            if (_withCaret.TreeNode.Parent == null) _withCaret.TreeNode.SetParent(_tree.TreeNode);
            bool isVisible = IsFocused;
            if (isVisible)
            {
                _caretFlashCounter--;
                if (_caretFlashCounter < 0)
                {
                    isVisible = false;
                    if (_caretFlashCounter < -CaretFlashDelay)
                    {
                        _caretFlashCounter = (int)CaretFlashDelay;
                    }                    
                }
            }
            _withCaret.Visible = isVisible;
            _withCaret.Text = _textComponent.Text;
            _withCaret.TextConfig = _textComponent.TextConfig;
            var renderer = _withCaret.CustomRenderer as ILabelRenderer;
            if (renderer != null)
            {
                renderer.CaretPosition = CaretPosition;
                renderer.BaseSize = _textComponent.LabelRenderSize;
                renderer.RenderCaret = true;
            }
            _textComponent.TextVisible = !isVisible;
            var imageComponent = _imageComponent;
            renderer = imageComponent == null ? null : imageComponent.CustomRenderer as ILabelRenderer;
            if (renderer != null)
            {
                renderer.CaretPosition = CaretPosition;
            }
        }

        private void onKeyUp(KeyboardEventArgs args)
        {
            if (args.Key == Key.ShiftLeft) { _leftShiftOn = false; return; }
            if (args.Key == Key.ShiftRight) { _rightShiftOn = false; return; }
        }

        private void onKeyDown(KeyboardEventArgs args)
        {
            if (args.Key == Key.ShiftLeft) { _leftShiftOn = true; return; }
            if (args.Key == Key.ShiftRight) { _rightShiftOn = true; return; }

            if (!IsFocused || _textComponent == null) return;
            TextBoxKeyPressingEventArgs pressingArgs = new TextBoxKeyPressingEventArgs(args.Key);
            OnPressingKey.Invoke(pressingArgs);
            if (pressingArgs.ShouldCancel) return;

            processKey(args.Key);
        }

        private void processKey(Key key)
        {
            switch (key)
            {
                case Key.Home: CaretPosition = 0; break;
                case Key.End: CaretPosition = _endOfLine; break;
                case Key.Left: CaretPosition = CaretPosition > 0 ? CaretPosition - 1 : 0; break;
                case Key.Right: CaretPosition = CaretPosition < _endOfLine ? CaretPosition + 1 : _endOfLine; break;
                case Key.BackSpace: processBackspace(); break;
                case Key.Delete: processDelete(); break;

                case Key.Tilde: addCharacter(_shiftOn ? '~' : '`'); break;
                case Key.Minus: addCharacter(_shiftOn ? '_' : '-'); break;
                case Key.Plus: addCharacter(_shiftOn ? '+' : '='); break;
                case Key.BracketLeft: addCharacter(_shiftOn ? '{' : '['); break;
                case Key.BracketRight: addCharacter(_shiftOn ? '}' : ']'); break;
                case Key.BackSlash: case Key.NonUSBackSlash: addCharacter(_shiftOn ? '|' : '\\'); break;
                case Key.Semicolon: addCharacter(_shiftOn ? ':' : ';'); break;
                case Key.Quote: addCharacter(_shiftOn ? '"' : '\''); break;
                case Key.Comma: addCharacter(_shiftOn ? '<' : ','); break;
                case Key.Period: addCharacter(_shiftOn ? '>' : '.'); break;
                case Key.Slash: addCharacter(_shiftOn ? '?' : '/'); break;
                case Key.KeypadAdd: addCharacter('+'); break;
                case Key.KeypadPeriod: addCharacter('.'); break;
                case Key.KeypadDivide: addCharacter('/'); break;
                case Key.KeypadMinus: addCharacter('-'); break;
                case Key.KeypadMultiply: addCharacter('*'); break;
                case Key.Space: addCharacter(' '); break;
                default:
                    if (key >= Key.A && key <= Key.Z)
                    {
                        processLetter(key);
                    }
                    if (key >= Key.Number0 && key <= Key.Number9)
                    {
                        processDigit((char)((int)'0' + (key - Key.Number0)));
                    }
                    if (key >= Key.Keypad0 && key <= Key.Keypad9)
                    {
                        addCharacter((char)((int)'0' + (key - Key.Keypad0)));
                    }
                    break;
            }
        }

        private void processDigit(char c)
        {
            if (!_shiftOn)
            {
                addCharacter(c);
                return;
            }
            switch (c)
            {
                case '1': addCharacter('!'); break;
                case '2': addCharacter('@'); break;
                case '3': addCharacter('#'); break;
                case '4': addCharacter('$'); break;
                case '5': addCharacter('%');  break;
                case '6': addCharacter('^'); break;
                case '7': addCharacter('&'); break;
                case '8': addCharacter('*'); break;
                case '9': addCharacter('('); break;
                case '0': addCharacter(')'); break;
            }
        }

        private void processLetter(Key key)
        {
            char c = (char)((int)'a' + (key - Key.A));
            if (_capslock || _shiftOn) c = char.ToUpperInvariant(c);
            addCharacter(c);
        }

        private void addCharacter(char c)
        {
            string text = _textComponent.Text;
            _textComponent.Text = string.Format("{0}{1}{2}", text.Substring(0, CaretPosition), c, text.Substring(CaretPosition));
            CaretPosition++;
        }

        private void processBackspace()
        {
            if (CaretPosition == 0) return;            
            string text = _textComponent.Text;
            _textComponent.Text = string.Format("{0}{1}", text.Substring(0, CaretPosition - 1), text.Substring(CaretPosition));
            CaretPosition--;
        }

        private void processDelete()
        {
            if (CaretPosition == _endOfLine) return;
            string text = _textComponent.Text;
            _textComponent.Text = string.Format("{0}{1}", text.Substring(0, CaretPosition), text.Substring(CaretPosition + 1));            
        }
    }
}
