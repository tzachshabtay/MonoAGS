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

        private ITextComponent _text;

        public AGSNumberEditorComponent(IBlockingEvent<NumberValueChangedArgs> onValueChanged)
        {
            OnValueChanged = onValueChanged;
            Step = 1f;
        }

        public float Value
        {
            get => _value;
            set
            {
                setValue(value, false);
            }
        }

        public void SetUserInitiatedValue(float value) => setValue(value, true);

        public bool EditWholeNumbersOnly { get => _editWholeNumbers; set { _editWholeNumbers = value; setText(); } }
        public float Step { get; set; }
        public float? MinValue { get => _minValue; set { _minValue = value; refreshSliderLimits(); } }
        public float? MaxValue { get => _maxValue; set { _maxValue = value; refreshSliderLimits(); } }
        public float? SuggestedMinValue { get => _suggestedMinValue; set { _suggestedMinValue = value; refreshSliderLimits(); } }
        public float? SuggestedMaxValue { get => _suggestedMaxValue; set { _suggestedMaxValue = value; refreshSliderLimits(); } }

        public IButton UpButton
        {
            get => _upButton;
            set
            {
                _upButton?.MouseClicked.Unsubscribe(onUpButtonClicked);
                _upButton = value;
                value?.MouseClicked.Subscribe(onUpButtonClicked);
            }
        }
        public IButton DownButton
        {
            get => _downButton;
            set
            {
                _downButton?.MouseClicked.Unsubscribe(onDownButtonClicked);
                _downButton = value;
                value?.MouseClicked.Subscribe(onDownButtonClicked);
            }
        }
        public ISlider Slider
        {
            get => _slider;
            set
            {
                _slider?.OnValueChanging.Unsubscribe(onSliderValueChanged);
                _slider = value;
                refreshSliderLimits();
                value?.OnValueChanging.Subscribe(onSliderValueChanged);
            }
        }

        public IBlockingEvent<NumberValueChangedArgs> OnValueChanged { get; }

        public override void Init()
        {
            base.Init();
            Entity.Bind<ITextBoxComponent>(
                c => { refreshValue(false); c.OnPressingKey.Subscribe(onPressingKey); },
                c => { c.OnPressingKey.Unsubscribe(onPressingKey); });
            Entity.Bind<ITextComponent>(c => { _text = c; c.Text = valueToString(); }, _ => { _text = null; });
        }

        public override void Dispose()
        {
            base.Dispose();
            _slider?.OnValueChanging?.Unsubscribe(onSliderValueChanged);
            _upButton?.MouseClicked?.Unsubscribe(onUpButtonClicked);
            _downButton?.MouseClicked?.Unsubscribe(onDownButtonClicked);
        }

        private string valueToString()
        {
            string valStr = Value.ToString("F");
            if (EditWholeNumbersOnly) valStr = valStr.Substring(0, valStr.Length - 3);
            return valStr;
        }

        private void setValue(float value, bool userInitiated)
        {
            if (MathUtils.FloatEquals(_value, value) || !validateValue(value)) return;
            _value = value;
            refreshValue(userInitiated);
        }

        private void refreshValue(bool userInitiated)
        {
            setText();
            refreshSlider();
            OnValueChanged.Invoke(new NumberValueChangedArgs(userInitiated));
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
            slider.ShouldClampValuesWhenChangingMinMax = false;
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
                    setValue(maxValue, true);
                    args.IntendedState.Text = valueToString();
                    break;
                case Key.PageDown:
                    if (!MinValue.HasValue) break;
                    float minValue = getMinValue();
                    setValue(minValue, true);
                    args.IntendedState.Text = valueToString();
                    break;
                default:
                    setValue(val, true);
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
            setValue(args.Value, true);
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
            if (!validateValue(valUp)) setValue(getMaxValue(), true);
            else setValue(valUp, true);
        }

        private void stepDown()
        {
            float valDown = Value - Step;
            if (!validateValue(valDown)) setValue(getMinValue(), true);
            else setValue(valDown, true);
        }

        private float getMinValue() => MinValue ?? (EditWholeNumbersOnly ? int.MinValue : float.MinValue);

        private float getMaxValue() => MaxValue ?? (EditWholeNumbersOnly ? int.MaxValue : float.MaxValue);
    }
}
