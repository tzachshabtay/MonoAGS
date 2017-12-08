using AGS.API;

namespace AGS.Engine
{
    [RequiredComponent(typeof(ITextBoxComponent))]
    [RequiredComponent(typeof(ITextComponent))]
    public class AGSNumberEditorComponent : AGSComponent, INumberEditorComponent
    {
        private float _value;
        private float? _minValue, _maxValue, _suggestedMinValue, _suggestedMaxValue;
        private ISlider _slider;
        private IButton _upButton, _downButton;
        private bool _editWholeNumbers;

        private ITextBoxComponent _textBox;
        private ITextComponent _text;

        public AGSNumberEditorComponent(IBlockingEvent onValueChanged)
        {
            OnValueChanged = onValueChanged;
            Step = 1f;
        }

        public float Value
        {
            get { return _value; }
            set
            {
                if (MathUtils.FloatEquals(_value, value) || !validateValue(value)) return;
                _value = value;
                refreshValue();
            }
        }
        public bool EditWholeNumbersOnly { get { return _editWholeNumbers; } set { _editWholeNumbers = value; setText(); } }
        public float Step { get; set; }
        public float? MinValue { get { return _minValue; } set { if (_minValue == value) return; _minValue = value; refreshSliderLimits(); } }
        public float? MaxValue { get { return _maxValue; } set { if (_maxValue == value) return; _maxValue = value; refreshSliderLimits(); } }
        public float? SuggestedMinValue { get { return _suggestedMinValue; } set { if (_suggestedMinValue == value) return; _suggestedMinValue = value; refreshSliderLimits(); } }
        public float? SuggestedMaxValue { get { return _suggestedMaxValue; } set { if (_suggestedMaxValue == value) return; _suggestedMaxValue = value; refreshSliderLimits(); } }

        public IButton UpButton
        {
            get { return _upButton; }
            set
            {
                var currentButton = _upButton;
                if (currentButton == value) return;
                if (currentButton != null)
                {
                    currentButton.MouseClicked.Unsubscribe(onUpButtonClicked);
                }
                _upButton = value;
                value.MouseClicked.Subscribe(onUpButtonClicked);
            }
        }
        public IButton DownButton
        {
            get { return _downButton; }
            set
            {
                var currentButton = _downButton;
                if (currentButton == value) return;
                if (currentButton != null)
                {
                    currentButton.MouseClicked.Unsubscribe(onDownButtonClicked);
                }
                _downButton = value;
                value.MouseClicked.Subscribe(onDownButtonClicked);
            }
        }
        public ISlider Slider
        {
            get { return _slider; }
            set
            {
                var currentSlider = _slider;
                if (currentSlider == value) return;
                if (currentSlider != null)
                {
                    currentSlider.OnValueChanging.Unsubscribe(onSliderValueChanged);
                }
                _slider = value;
                refreshSliderLimits();
                value.OnValueChanging.Subscribe(onSliderValueChanged);
            }
        }

        public IBlockingEvent OnValueChanged { get; private set; }

        public override void Init(IEntity entity)
        {
            base.Init(entity);
            entity.Bind<ITextBoxComponent>(c => { _textBox = c; refreshValue(); c.OnPressingKey.Subscribe(onPressingKey); },
                                           c => { c.OnPressingKey.Unsubscribe(onPressingKey); });
            entity.Bind<ITextComponent>(c => { _text = c; c.Text = valueToString(); }, _ => { _text = null; });
        }

        private string valueToString()
        {
            string valStr = Value.ToString("F");
            if (EditWholeNumbersOnly) valStr = valStr.Substring(0, valStr.Length - 3);
            return valStr;
        }

        private void refreshValue()
        {
            setText();
            refreshSlider();
            OnValueChanged.Invoke();
        }

        private void setText()
        {
            var textBox = _text;
            if (textBox != null) textBox.Text = valueToString();
        }

        private void refreshSlider()
        {
            var slider = Slider;
            if (slider == null) return;
            slider.OnValueChanging.Unsubscribe(onSliderValueChanged);
            slider.Value = Value > slider.MaxValue ? slider.MaxValue :
                           Value < slider.MinValue ? slider.MinValue : Value;
            slider.OnValueChanging.Subscribe(onSliderValueChanged);
        }

        private void refreshSliderLimits()
        {
            float minValue = MinValue ?? -1000f;
            float maxValue = MaxValue ?? 1000f;
            if (Value < minValue) Value = minValue;
            else if (Value > maxValue) Value = maxValue;
            var slider = _slider;
            if (slider == null) return;
            float suggestedMinValue = SuggestedMinValue.HasValue ? SuggestedMinValue.Value : minValue;
            float suggestedMaxValue = SuggestedMaxValue.HasValue ? SuggestedMaxValue.Value : maxValue;
            slider.MinValue = suggestedMinValue;
            slider.MaxValue = suggestedMaxValue;
        }

        private void onPressingKey(TextBoxKeyPressingEventArgs args)
        {
            float val;
            if (string.IsNullOrEmpty(args.IntendedState.Text))
            {
                val = 0f;
                if (!validateValue(val)) val = getMinValue();
            }
            else if (EditWholeNumbersOnly)
            {
                int intVal;
                if (!int.TryParse(args.IntendedState.Text, out intVal)) 
                {
                    args.ShouldCancel = true;
                    return;
                }
                val = intVal;
            }
            else
            {
                if (!float.TryParse(args.IntendedState.Text, out val))
                {
                    args.ShouldCancel = true;
                    return;
                }
            }
            if (!validateValue(val))
            {
                args.ShouldCancel = true;
                return;
            }
            switch (args.PressedKey)
            {
                case Key.Up:
                    stepUp();
                    args.IntendedState.Text = valueToString();
                    break;
                case Key.Down:
                    stepDown();
                    args.IntendedState.Text = valueToString();
                    break;
                case Key.PageUp:
                    if (!MaxValue.HasValue) break;
                    float maxValue = getMaxValue();
                    Value = maxValue;
                    args.IntendedState.Text = valueToString();
                    break;
                case Key.PageDown:
                    if (!MinValue.HasValue) break;
                    float minValue = getMinValue();
                    Value = minValue;
                    args.IntendedState.Text = valueToString();
                    break;
                default:
                    Value = val;
                    args.IntendedState.Text = valueToString();
                    break;
            }
        }

        private bool validateValue(float val)
        {
            float minValue = getMinValue();
            float maxValue = getMaxValue();
            return (val >= minValue && val <= maxValue);
        }

        private void onSliderValueChanged(SliderValueEventArgs args)
        {
            Value = args.Value;
        }

        private void onUpButtonClicked(MouseButtonEventArgs args)
        {
            stepUp();
        }

        private void onDownButtonClicked(MouseButtonEventArgs args)
        {
            stepDown();
        }

        private void stepUp()
        {
            float valUp = Value + Step;
            if (!validateValue(valUp)) Value = getMaxValue();
            else Value = valUp;
        }

        private void stepDown()
        {
            float valDown = Value - Step;
            if (!validateValue(valDown)) Value = getMinValue();
            else Value = valDown;
        }

        private float getMinValue()
        {
            return MinValue ?? (EditWholeNumbersOnly ? int.MinValue : float.MinValue);
        }

        private float getMaxValue()
        {
            return MaxValue ?? (EditWholeNumbersOnly ? int.MaxValue : float.MaxValue);
        }
    }
}
