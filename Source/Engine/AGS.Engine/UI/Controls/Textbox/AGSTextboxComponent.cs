using AGS.API;
using System.Diagnostics;
using System.ComponentModel;

namespace AGS.Engine
{
    public class AGSTextBoxComponent : AGSComponent, ITextBoxComponent
    {
        private bool _isFocused;
        private ITextComponent _textComponent;
        private IImageComponent _imageComponent;
        private IVisibleComponent _visibleComponent;
        private IDrawableInfo _drawableComponent;
        private IUIEvents _uiEvents;        
        private IInObjectTree _tree;
        private IHasRoom _room;
        private IEntity _entity;
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
            _entity = entity;
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

            entity.Bind<IDrawableInfo>(c =>
            {
                _drawableComponent = c;
                c.PropertyChanged += onDrawableChanged;
                onRenderLayerChanged();
            }, c => 
            { 
                c.PropertyChanged -= onDrawableChanged; 
                _drawableComponent = null; 
            });
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
                    _focusedUi.HasKeyboardFocus = _entity;
                }
                else
                {
                    _keyboardState.HideSoftKeyboard();
                    if (_focusedUi.HasKeyboardFocus == this)
                        _focusedUi.HasKeyboardFocus = null;
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

        private void onDrawableChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName != nameof(IDrawableInfo.RenderLayer)) return;
            onRenderLayerChanged();
        }

        private void onRenderLayerChanged()
        {
            var drawable = _drawableComponent;
            if (drawable == null) return;
            var layer = drawable.RenderLayer;
            if (layer == null) return;
            _withCaret.RenderLayer = layer;
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
            _withCaret.Tint = _imageComponent.Tint;
            _withCaret.Visible = isVisible;
            _textComponent.TextVisible = _textComponent.TextBackgroundVisible = !isVisible;
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
            var intendedState = processKey(args.Key);
            TextBoxKeyPressingEventArgs pressingArgs = new TextBoxKeyPressingEventArgs(args.Key, intendedState);
            OnPressingKey.Invoke(pressingArgs);
            if (pressingArgs.ShouldCancel) return;

            _textComponent.Text = intendedState.Text;
            int pos = intendedState.CaretPosition;
            if (pos > intendedState.Text.Length) pos = intendedState.Text.Length;
            CaretPosition = pos;
        }

        private TextboxState processKey(Key key)
        {
            switch (key)
            {
                case Key.Home: return new TextboxState(_textComponent.Text, 0);
                case Key.End: return new TextboxState(_textComponent.Text, _endOfLine);
                case Key.Left: return new TextboxState(_textComponent.Text, CaretPosition > 0 ? CaretPosition - 1 : 0);
                case Key.Right: return new TextboxState(_textComponent.Text, CaretPosition < _endOfLine ? CaretPosition + 1 : _endOfLine);
                case Key.BackSpace: return processBackspace();
                case Key.Delete: return processDelete();

                case Key.Tilde: return addCharacter(_shiftOn ? '~' : '`');
                case Key.Minus: return addCharacter(_shiftOn ? '_' : '-');
                case Key.Plus: return addCharacter(_shiftOn ? '+' : '=');
                case Key.BracketLeft: return addCharacter(_shiftOn ? '{' : '[');
                case Key.BracketRight: return addCharacter(_shiftOn ? '}' : ']');
                case Key.BackSlash: case Key.NonUSBackSlash: return addCharacter(_shiftOn ? '|' : '\\');
                case Key.Semicolon: return addCharacter(_shiftOn ? ':' : ';');
                case Key.Quote: return addCharacter(_shiftOn ? '"' : '\'');
                case Key.Comma: return addCharacter(_shiftOn ? '<' : ',');
                case Key.Period: return addCharacter(_shiftOn ? '>' : '.');
                case Key.Slash: return addCharacter(_shiftOn ? '?' : '/');
                case Key.KeypadAdd: return addCharacter('+');
                case Key.KeypadPeriod: return addCharacter('.');
                case Key.KeypadDivide: return addCharacter('/');
                case Key.KeypadMinus: return addCharacter('-');
                case Key.KeypadMultiply: return addCharacter('*');
                case Key.Space: return addCharacter(' ');
                default:
                    if (key >= Key.A && key <= Key.Z)
                    {
                        return processLetter(key);
                    }
                    if (key >= Key.Number0 && key <= Key.Number9)
                    {
                        return processDigit((char)((int)'0' + (key - Key.Number0)));
                    }
                    if (key >= Key.Keypad0 && key <= Key.Keypad9)
                    {
                        return addCharacter((char)((int)'0' + (key - Key.Keypad0)));
                    }
                    return getCurrentState();
            }
        }

        private TextboxState processDigit(char c)
        {
            if (!_shiftOn)
            {
                return addCharacter(c);
            }
            switch (c)
            {
                case '1': return addCharacter('!');
                case '2': return addCharacter('@');
                case '3': return addCharacter('#');
                case '4': return addCharacter('$');
                case '5': return addCharacter('%');
                case '6': return addCharacter('^');
                case '7': return addCharacter('&');
                case '8': return addCharacter('*');
                case '9': return addCharacter('(');
                case '0': return addCharacter(')');
            }
            return getCurrentState();
        }

        private TextboxState processLetter(Key key)
        {
            char c = (char)((int)'a' + (key - Key.A));
            if (_capslock || _shiftOn) c = char.ToUpperInvariant(c);
            return addCharacter(c);
        }

        private TextboxState addCharacter(char c)
        {
            string text = _textComponent.Text;
            text = string.Format("{0}{1}{2}", text.Substring(0, CaretPosition), c, text.Substring(CaretPosition));
            return new TextboxState(text, CaretPosition + 1);
        }

        private TextboxState processBackspace()
        {
            if (CaretPosition == 0) return getCurrentState();            
            string text = _textComponent.Text;
            text = string.Format("{0}{1}", text.Substring(0, CaretPosition - 1), text.Substring(CaretPosition));
            return new TextboxState(text, CaretPosition - 1);
        }

        private TextboxState processDelete()
        {
            if (CaretPosition == _endOfLine) return getCurrentState();
            string text = _textComponent.Text;
            text = string.Format("{0}{1}", text.Substring(0, CaretPosition), text.Substring(CaretPosition + 1));
            return new TextboxState(text, CaretPosition);
        }

        private TextboxState getCurrentState()
        {
            return new TextboxState(_textComponent.Text, CaretPosition);
        }
    }
}
