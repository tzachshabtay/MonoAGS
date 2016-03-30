using System;
using AGS.API;

namespace AGS.Engine
{
	public class AGSSliderComponent : AGSComponent, ISliderComponent
	{
		private bool _isSliding;
		private IObject _graphics, _handleGraphics;
		private ILabel _label;
		private float _minValue, _maxValue, _value;
		private bool _isHorizontal;

		private readonly IInput _input;
		private readonly IGameState _state;
		private readonly IGameEvents _gameEvents;
		private ICollider _collider;
		private IDrawableInfo _drawableInfo;
		private IInTree<IObject> _tree;
		private IVisibleComponent _visible;
		private IEnabledComponent _enabled;

		public AGSSliderComponent(IGameState state, IInput input, IGameEvents gameEvents)
		{
			_state = state;
			_input = input;
			_gameEvents = gameEvents;
			OnValueChanged = new AGSEvent<SliderValueEventArgs> ();
		}

		public override void Init(IEntity entity)
		{
			base.Init(entity);
			_collider = entity.GetComponent<ICollider>();
			_drawableInfo = entity.GetComponent<IDrawableInfo>();
			_tree = entity.GetComponent<IInTree<IObject>>();
			_visible = entity.GetComponent<IVisibleComponent>();
			_enabled = entity.GetComponent<IEnabledComponent>();
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
				_minValue = value;
				refresh();
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
				_maxValue = value;
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
				setValue(value);
				onValueChanged();
			}
		}

		public bool IsHorizontal
		{
			get 
			{ 
				return _isHorizontal; 
			}
			set 
			{
				_isHorizontal = value;
				refresh();
			}
		}

		public IEvent<SliderValueEventArgs> OnValueChanged { get; private set; }

		public override void Dispose()
		{
			_gameEvents.OnRepeatedlyExecute.Unsubscribe(onRepeatedlyExecute);
			if (_graphics != null) _graphics.Dispose();
			if (_handleGraphics != null) _handleGraphics.Dispose();
			if (_label != null) _label.Dispose();
		}

		private void updateGraphics(IObject oldGraphics, IObject newGraphics, float z)
		{
			if (oldGraphics != null)
			{
				_state.UI.Remove(oldGraphics);
				oldGraphics.TreeNode.SetParent(null);
			}
			if (newGraphics == null) return;
			newGraphics.RenderLayer = _drawableInfo.RenderLayer;
			newGraphics.Z = z;
			newGraphics.TreeNode.SetParent(_tree.TreeNode);
			_state.UI.Add(newGraphics);
		}

		private void onRepeatedlyExecute(object sender, AGSEventArgs args)
		{
			if (!_visible.Visible || !_enabled.Enabled || _collider.BoundingBox == null || !_input.LeftMouseButtonDown || Graphics == null || Graphics.BoundingBox == null ||
				!Graphics.BoundingBox.Contains(new AGSPoint (_input.MouseX, _input.MouseY)) || HandleGraphics == null)
			{
				if (_isSliding)
				{
					_isSliding = false;
					onValueChanged();
				}
				return;
			}
			_isSliding = true;
			if (IsHorizontal) setValue(getSliderValue(MathUtils.Clamp(_input.MouseX - _collider.BoundingBox.MinX, 0f, Graphics.Width)));
			else setValue(getSliderValue(MathUtils.Clamp(_input.MouseY - _collider.BoundingBox.MinY
				, 0f, Graphics.Height)));
		}

		private void refresh()
		{
			if (Graphics == null || HandleGraphics == null) return;

			if (IsHorizontal) HandleGraphics.X = MathUtils.Clamp(getHandlePos(Value), 0f, Graphics.Width);
			else HandleGraphics.Y = MathUtils.Clamp(getHandlePos(Value), 0f, Graphics.Height);
			setText();
		}

		private float getSliderValue(float handlePos)
		{
			return MathUtils.Lerp(0f, MinValue, IsHorizontal ? Graphics.Width : Graphics.Height, MaxValue, handlePos);
		}

		private float getHandlePos(float value)
		{
			return MathUtils.Lerp(MinValue, 0f, MaxValue, IsHorizontal ? Graphics.Width : Graphics.Height, value);
		}

		private void setText()
		{
			if (_label == null) return;
			_label.Text = ((int)Value).ToString();
		}

		private void setValue(float value)
		{
			if (_value == value) return;
			_value = value;
			refresh();
		}

		private void onValueChanged()
		{
			OnValueChanged.Invoke(this, new SliderValueEventArgs (_value));
		}
	}
}

