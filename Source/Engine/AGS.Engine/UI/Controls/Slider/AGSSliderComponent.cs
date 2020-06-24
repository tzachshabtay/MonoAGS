using System;
using System.ComponentModel;
using AGS.API;

namespace AGS.Engine
{
    public class AGSSliderComponent : AGSComponent, ISliderComponent
    {
        private bool _isSliding;
        private IObject _graphics, _handleGraphics;
        private ILabel _label;
        private float _minValue, _maxValue, _value, _minHandleOffset, _maxHandleOffset;
        private SliderDirection _direction;

        private readonly IFocusedUI _focus;
        private readonly IInput _input;
        private readonly IGameState _state;
        private readonly IGameEvents _gameEvents;
        private IBoundingBoxComponent _boundingBox;
        private IScaleComponent _scale;
        private IDrawableInfoComponent _drawableInfo;
        private IInObjectTreeComponent _tree;
        private IVisibleComponent _visible;
        private IEnabledComponent _enabled;

        public AGSSliderComponent(IGameState state, IInput input, IGameEvents gameEvents, IFocusedUI focus)
        {
            _focus = focus;
            _state = state;
            _input = input;
            _gameEvents = gameEvents;
            AllowKeyboardControl = true;
            ShouldClampValuesWhenChangingMinMax = true;
            OnValueChanged = new AGSEvent<SliderValueEventArgs>();
            OnValueChanging = new AGSEvent<SliderValueEventArgs>();
            input.KeyDown.Subscribe(onKeyDown);
        }

        public override void Init()
        {
            base.Init();
            bindGraphics(Graphics);
            Entity.Bind<IDrawableInfoComponent>(c => _drawableInfo = c, _ => _drawableInfo = null);
            Entity.Bind<IInObjectTreeComponent>(c => _tree = c, _ => _tree = null);
            Entity.Bind<IVisibleComponent>(c => _visible = c, _ => _visible = null);
            Entity.Bind<IEnabledComponent>(c => _enabled = c, _ => _enabled = null);
            Entity.Bind<IUIEvents>(c => c.LostFocus.Subscribe(onLostFocus), c => c.LostFocus.Unsubscribe(onLostFocus));
            _gameEvents.OnRepeatedlyExecute.Subscribe(onRepeatedlyExecute);
        }

        public IObject Graphics
        {
            get
            {
                return _graphics;
            }
            set
            {
                updateGraphics(_graphics, value, -50f);
                _graphics = value;
                bindGraphics(value);
                refresh();
            }
        }

        public IObject HandleGraphics
        {
            get
            {
                return _handleGraphics;
            }
            set
            {
                updateGraphics(_handleGraphics, value, -100f);
                _handleGraphics = value;
                refresh();
            }
        }

        public ILabel Label
        {
            get { return _label; }
            set
            {
                updateGraphics(_label, value, -100f);
                _label = value;
                setText();
            }
        }

        public float MinValue
        {
            get
            {
                return _minValue;
            }
            set
            {
                if (MathUtils.FloatEquals(_minValue, value)) return;
                _minValue = value;
                if (ShouldClampValuesWhenChangingMinMax && Value < _minValue) Value = _minValue;
                else refresh();
            }
        }

        public float MaxValue
        {
            get
            {
                return _maxValue;
            }
            set
            {
                if (MathUtils.FloatEquals(_maxValue, value)) return;
                _maxValue = value;
                if (ShouldClampValuesWhenChangingMinMax && Value > _maxValue) Value = _maxValue;
                else refresh();
            }
        }

        public bool ShouldClampValuesWhenChangingMinMax { get; set; }

        public float MinHandleOffset
        {
            get
            {
                return _minHandleOffset;
            }
            set
            {
                if (MathUtils.FloatEquals(_minHandleOffset, value)) return;
                _minHandleOffset = value;
                refresh();
            }
        }

        public float MaxHandleOffset
        {
            get
            {
                return _maxHandleOffset;
            }
            set
            {
                if (MathUtils.FloatEquals(_maxHandleOffset, value)) return;
                _maxHandleOffset = value;
                refresh();
            }
        }

        public float Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (!setValue(value)) return;
                onValueChanged();
            }
        }

        public SliderDirection Direction
        {
            get
            {
                return _direction;
            }
            set
            {
                _direction = value;
                refresh();
            }
        }

        public bool AllowKeyboardControl { get; set; }

        public IBlockingEvent<SliderValueEventArgs> OnValueChanged { get; private set; }

        public IBlockingEvent<SliderValueEventArgs> OnValueChanging { get; private set; }

        public void Increase(float step) => Value = Math.Max(Value, MinValue) + step;

        public void Decrease(float step) => Value = Math.Min(Value, MaxValue) - step;

        public bool IsHorizontal() =>  _direction == SliderDirection.LeftToRight || _direction == SliderDirection.RightToLeft;

        public override void Dispose()
        {
            _gameEvents?.OnRepeatedlyExecute.Unsubscribe(onRepeatedlyExecute);
            _input?.KeyDown.Unsubscribe(onKeyDown);
            _graphics?.Dispose();
            _handleGraphics?.Dispose();
            _label?.Dispose();
        }

        private void bindGraphics(IObject graphics)
        {
            if (graphics == null) return;
            graphics.Bind<IBoundingBoxComponent>(c => { _boundingBox = c; c.OnBoundingBoxesChanged.Subscribe(refresh); },
                                                 c => { _boundingBox = null; c.OnBoundingBoxesChanged.Unsubscribe(refresh); });
            graphics.Bind<IScaleComponent>(c => { _scale = c; c.PropertyChanged += onScaleChanged; },
                                           c => { _scale = null; c.PropertyChanged -= onScaleChanged; });
        }

        private void onScaleChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName != nameof(IScaleComponent.Width) &&
                args.PropertyName != nameof(IScaleComponent.Height)) return;
            refresh();
        }

        private void onLostFocus(MouseButtonEventArgs args)
        {
            if (args.ClickedEntity != null && (args.ClickedEntity == Graphics || args.ClickedEntity == HandleGraphics
                                               || args.ClickedEntity == Label)) return;
            if (_focus.HasKeyboardFocus == Entity) _focus.HasKeyboardFocus = null;
        }

        private void onKeyDown(KeyboardEventArgs args)
        {
            if (_focus.HasKeyboardFocus != Entity || !AllowKeyboardControl) return;
            float smallStep = (MaxValue - MinValue) / 100f;
            float bigStep = (MaxValue - MinValue) / 5f;
            switch (args.Key)
            {
                case Key.Up:
                    if (IsHorizontal()) return;
                    if (Direction == SliderDirection.BottomToTop) Increase(smallStep);
                    else Decrease(smallStep);
                    break;
                case Key.Down:
					if (IsHorizontal()) return;
                    if (Direction == SliderDirection.BottomToTop) Decrease(smallStep);
                    else Increase(smallStep);
					break;
                case Key.Right:
					if (!IsHorizontal()) return;
                    if (Direction == SliderDirection.LeftToRight) Increase(smallStep);
                    else Decrease(smallStep);
					break;
                case Key.Left:
					if (!IsHorizontal()) return;
                    if (Direction == SliderDirection.LeftToRight) Decrease(smallStep);
                    else Increase(smallStep);
					break;
                case Key.PageUp:
                    if (Direction == SliderDirection.BottomToTop || Direction == SliderDirection.LeftToRight) Increase(bigStep);
                    else Decrease(bigStep);
                    break;
                case Key.PageDown:
                    if (Direction == SliderDirection.BottomToTop || Direction == SliderDirection.LeftToRight) Decrease(bigStep);
                    else Increase(bigStep);
                    break;
                case Key.Home:
                    if (Direction == SliderDirection.BottomToTop || Direction == SliderDirection.LeftToRight) Value = MinValue;
                    else Value = MaxValue;
                    break;
                case Key.End:
                    if (Direction == SliderDirection.BottomToTop || Direction == SliderDirection.LeftToRight) Value = MaxValue;
                    else Value = MinValue;
                    break;
            }
        }

		private void updateGraphics(IObject oldGraphics, IObject newGraphics, float z)
		{
			if (oldGraphics != null)
			{
				_state.UI.Remove(oldGraphics);
				oldGraphics.TreeNode.SetParent(null);
			}
			if (newGraphics == null) return;
            var drawableInfo = _drawableInfo;
            if (drawableInfo != null) newGraphics.RenderLayer = drawableInfo.RenderLayer;
			newGraphics.Z = z;
            var tree = _tree;
            if (tree != null) newGraphics.TreeNode.SetParent(tree.TreeNode);
			_state.UI.Add(newGraphics);
		}

		private void onRepeatedlyExecute()
		{
            var boundingBox = _boundingBox;
            if (boundingBox == null) return;
            var hitTestBox = boundingBox.WorldBoundingBox;
            var visible = _visible;
            var enabled = _enabled;
            if (visible == null || !visible.Visible || enabled == null || !enabled.Enabled || 
                enabled.ClickThrough ||
                (!_input.LeftMouseButtonDown && !_input.IsTouchDrag) || Graphics == null || 
                Graphics.GetBoundingBoxes(_state.Viewport) == null || HandleGraphics == null)
			{
				if (_isSliding)
				{
					_isSliding = false;
					onValueChanged();
				}
				return;
			}
            if (!_isSliding && !Graphics.CollidesWith(
                _input.MousePosition.XMainViewport, _input.MousePosition.YMainViewport, _state.Viewport))
            {
                return;
            }
            _focus.HasKeyboardFocus = Entity;
			_isSliding = true;
            if (IsHorizontal()) setValue(getSliderValue(MathUtils.Clamp(_input.MousePosition.XMainViewport - hitTestBox.MinX, 
                                                                        0f, hitTestBox.Width), ref hitTestBox));
            else setValue(getSliderValue(MathUtils.Clamp(_input.MousePosition.YMainViewport - hitTestBox.MinY
                                                         , 0f, hitTestBox.Height), ref hitTestBox));
		}

		private void refresh()
		{
            var boundingBox = _boundingBox;
            var scale = _scale;
            if (boundingBox == null || scale == null) return;
            var boundingBoxes = boundingBox.GetBoundingBoxes(_state.Viewport);
            if (boundingBoxes == null || HandleGraphics == null) return;

            if (MathUtils.FloatEquals(MinValue, MaxValue)) return;

            var handlePos = getHandlePos(Value, scale);
            if (IsHorizontal()) HandleGraphics.X = MathUtils.Clamp(handlePos, 0f, boundingBoxes.ViewportBox.Width);
            else HandleGraphics.Y = MathUtils.Clamp(handlePos, 0f, boundingBoxes.ViewportBox.Height);
			setText();
		}

        private float getSliderValue(float handlePos, ref AGSBoundingBox hitTestBox)
		{
            float min = isReverse() ? MaxValue : MinValue;
            float max = isReverse() ? MinValue : MaxValue;
            return MathUtils.Lerp(0f, min, IsHorizontal() ? 
                                  hitTestBox.Width : hitTestBox.Height, 
                                  max, handlePos);
		}

		private float getHandlePos(float value, IScale scale)
		{
            float min = isReverse() ? MaxValue : MinValue;
            float max = isReverse() ? MinValue : MaxValue;
            return MathUtils.Lerp(min, _minHandleOffset, max, IsHorizontal() ? 
                                  scale.Width - _maxHandleOffset : scale.Height - _maxHandleOffset, 
                                  value);
		}

		private void setText()
		{
			if (_label == null) return;
			_label.Text = ((int)Value).ToString();
		}

		private bool setValue(float value)
		{
            if (MathUtils.FloatEquals(_value, value)) return false;
            if (value < MinValue) _value = MinValue;
            else if (value > MaxValue) _value = MaxValue;
            else _value = value;
            OnValueChanging.Invoke(new SliderValueEventArgs(_value));
			refresh();
            return true;
		}

		private void onValueChanged()
		{
            var args = new SliderValueEventArgs(_value);
            OnValueChanging.Invoke(args);
			OnValueChanged.Invoke(args);
		}

        private bool isReverse() => _direction == SliderDirection.RightToLeft || _direction == SliderDirection.TopToBottom;
	}
}
