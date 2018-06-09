using AGS.API;
using System.Diagnostics;
using System.ComponentModel;
using System;

namespace AGS.Engine
{
    public class AGSTextBoxComponent : AGSComponent, ITextBoxComponent
    {
        private bool _isFocused;
        private ITextComponent _textComponent;
        private IImageComponent _imageComponent;
        private IBorderComponent _borderComponent;
        private IVisibleComponent _visibleComponent;
        private IDrawableInfoComponent _drawableComponent;
        private IUIEvents _uiEvents;        
        private IInObjectTreeComponent _tree;
        private IHasRoomComponent _room;
        private IEntity _entity;
        private readonly IKeyboardState _keyboardState;
        private readonly IGame _game;
        private readonly IFocusedUI _focusedUi;
        private readonly IInput _input;
        private int _caretFlashCounter;

        private int _endOfLine => _textComponent.Text.Length;
        private bool _capslock => _keyboardState.CapslockOn;
        private bool _leftShiftOn, _rightShiftOn;
        private bool _shiftOn => _leftShiftOn || _rightShiftOn || (_capslock && _keyboardState.SoftKeyboardVisible);

        private ILabel _withCaret, _watermark;

        public AGSTextBoxComponent(IBlockingEvent<TextBoxKeyPressingEventArgs> onPressingKey,
                                   IInput input, IGame game, IKeyboardState keyboardState, IFocusedUI focusedUi)
        {
            CaretFlashDelay = 10;
            _input = input;
            _keyboardState = keyboardState;
            _focusedUi = focusedUi;
            OnPressingKey = onPressingKey;
            _game = game;
            
            input.KeyDown.Subscribe(onKeyDown);
            input.KeyUp.Subscribe(onKeyUp);
        }

        public override void Init(IEntity entity)
        {
            base.Init(entity);
            _entity = entity;
            entity.Bind<ITextComponent>(c =>
            {
                _textComponent = c;
                c.PropertyChanged += onTextPropertyChanged;
            }, c =>
            {
                _textComponent = null;
                c.PropertyChanged -= onTextPropertyChanged;
            });
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
            entity.Bind<IInObjectTreeComponent>(c => _tree = c, _ => _tree = null);
            entity.Bind<IHasRoomComponent>(c => _room = c, _ => _room = null);
            entity.Bind<IVisibleComponent>(c => _visibleComponent = c, _ => _visibleComponent = null);

            _caretFlashCounter = (int)CaretFlashDelay;
            _withCaret = _game.Factory.UI.GetLabel(entity.ID + " Caret", "|", 1f, 1f, 0f, 0f, config: new AGSTextConfig(autoFit: AutoFit.LabelShouldFitText));
            _withCaret.Pivot = new PointF(0f, 0f);
            _withCaret.TextBackgroundVisible = false;

            entity.Bind<IImageComponent>(c => _imageComponent = c, _ => _imageComponent = null);

            entity.Bind<IBorderComponent>(c => _borderComponent = c, _ => _borderComponent = null);

            entity.Bind<IDrawableInfoComponent>(c =>
            {
                _drawableComponent = c;
                c.PropertyChanged += onDrawableChanged;
                onRenderLayerChanged();
            }, c => 
            { 
                c.PropertyChanged -= onDrawableChanged; 
                _drawableComponent = null; 
            });

            _game.Events.OnRepeatedlyExecute.Subscribe(onRepeatedlyExecute);
        }

        private void onTextPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(ITextComponent.Text)) return;
            updateWatermark();
        }

        public override void AfterInit()
        {
            base.AfterInit();
            _game.Events.OnBeforeRender.Subscribe(onBeforeRender);
        }

        public bool IsFocused
        {
            get => _isFocused;
            set
            {
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
                    if (_focusedUi.HasKeyboardFocus == _entity)
                        _focusedUi.HasKeyboardFocus = null;
                }
                updateWatermark();
            }
        }

        public int CaretPosition { get; set; }
        public uint CaretFlashDelay { get; set; }

        public IBlockingEvent<TextBoxKeyPressingEventArgs> OnPressingKey { get; }

        public ILabel Watermark { get { return _watermark; } set { _watermark = value; updateWatermark(); }}

        public override void Dispose()
        {
            base.Dispose();
            _game?.Events.OnRepeatedlyExecute.Unsubscribe(onRepeatedlyExecute);
            _game?.Events.OnBeforeRender.Unsubscribe(onBeforeRender);
            _input?.KeyDown.Unsubscribe(onKeyDown);
            _input?.KeyUp.Unsubscribe(onKeyUp);
            _entity = null;
        }

        private void onDrawableChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName != nameof(IDrawableInfoComponent.RenderLayer)) return;
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
            if (_withCaret.TreeNode.Parent == null) _withCaret.TreeNode.SetParent(_tree.TreeNode);
        }

        private void onSoftKeyboardHidden()
        {
            _keyboardState.OnSoftKeyboardHidden.Unsubscribe(onSoftKeyboardHidden);
            IsFocused = false;
        }

        private void updateWatermark()
        {
            var watermark = _watermark;
            if (watermark == null) return;
            watermark.Visible = !IsFocused && string.IsNullOrEmpty(_textComponent?.Text);
        }

        private void onMouseDown(MouseButtonEventArgs args)
        {
            IsFocused = true;
        }

        private void onMouseDownOutside(MouseButtonEventArgs args)
        {
            IsFocused = false;
        }

        private void onBeforeRender()
        {
            if (_textComponent == null) return;
            if (_room?.Room != null && _room?.Room != _game?.State?.Room) return;
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

                _withCaret.Tint = _imageComponent.Tint;
                _withCaret.Border = isVisible ? _borderComponent?.Border : null;
                _withCaret.Text = _textComponent.Text;
                _withCaret.TextConfig = _textComponent.TextConfig;
            }
            _withCaret.TextVisible = isVisible;
            _textComponent.TextVisible = !isVisible;

            var caretTextComponent = _withCaret.GetComponent<ITextComponent>();
            if (caretTextComponent != null)
            {
                caretTextComponent.CaretPosition = CaretPosition;
                caretTextComponent.LabelRenderSize = _textComponent.LabelRenderSize;
                caretTextComponent.RenderCaret = true;
            }
            _textComponent.TextVisible = !isVisible;
            var imageComponent = _imageComponent;
            var textComponent = _textComponent;
            if (textComponent != null)
            {
                textComponent.CaretPosition = CaretPosition;
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
            text = $"{text.Substring(0, CaretPosition - 1)}{text.Substring(CaretPosition)}";
            return new TextboxState(text, CaretPosition - 1);
        }

        private TextboxState processDelete()
        {
            if (CaretPosition == _endOfLine) return getCurrentState();
            string text = _textComponent.Text;
            text = $"{text.Substring(0, CaretPosition)}{text.Substring(CaretPosition + 1)}";
            return new TextboxState(text, CaretPosition);
        }

        private TextboxState getCurrentState() => new TextboxState(_textComponent.Text, CaretPosition);
    }
}
