using System;
using AGS.API;
using System.Threading.Tasks;
using System.Drawing;

namespace AGS.Engine
{
	public class AGSSlider : ISlider
	{
		private IPanel _obj;
		private IObject _graphics, _handleGraphics;
		private ILabel _label;
		private float _minValue, _maxValue, _value;
		private bool _isHorizontal;
		private IInput _input;
		private IGameState _state;

		public AGSSlider(IPanel panel, IInput input, IGameEvents gameEvents, IGameState state)
		{
			_input = input;
			_obj = panel;
			_state = state;
			OnValueChanged = new AGSEvent<SliderValueEventArgs> ();

			gameEvents.OnRepeatedlyExecute.Subscribe(onRepeatedlyExecute);
		}

		#region IUIControl implementation

		public void ApplySkin(ISlider skin)
		{
			throw new NotImplementedException();
		}

		public IUIEvents Events { get { return _obj.Events; } }

		public bool IsMouseIn { get { return _obj.IsMouseIn; } }

		#endregion

		#region IObject implementation

		public ICustomProperties Properties { get { return _obj.Properties; } }

		public void ChangeRoom(IRoom room, float? x = default(float?), float? y = default(float?))
		{
			_obj.ChangeRoom(room, x, y);
		}

		public bool CollidesWith(float x, float y)
		{
			return _obj.CollidesWith(x, y);
		}

		public IRoom Room { get { return _obj.Room; } }

		public IInteractions Interactions { get { return _obj.Interactions; } }

		public ISquare BoundingBox { get { return _obj.BoundingBox; } set { _obj.BoundingBox = value; } }

		public IRenderLayer RenderLayer { get { return _obj.RenderLayer; } set { _obj.RenderLayer = value; } }

		public IPoint WalkPoint { get { return _obj.WalkPoint; } set { _obj.WalkPoint = value; } }

		public IPoint CenterPoint { get { return _obj.CenterPoint; } }

		public bool Enabled { get { return _obj.Enabled; } set { _obj.Enabled = value; } }

		public string Hotspot { get { return _obj.Hotspot; } set { _obj.Hotspot = value; } }

		public bool IgnoreViewport { get { return _obj.IgnoreViewport; } set { _obj.IgnoreViewport = value; } }

		public bool IgnoreScalingArea { get { return _obj.IgnoreScalingArea; } set { _obj.IgnoreScalingArea = value; } }

		#endregion

		#region IAnimationContainer implementation

		public void StartAnimation(IAnimation animation)
		{
			_obj.StartAnimation(animation);
		}

		public AnimationCompletedEventArgs Animate(IAnimation animation)
		{
			return _obj.Animate(animation);
		}

		public async Task<AnimationCompletedEventArgs> AnimateAsync(IAnimation animation)
		{
			return await _obj.AnimateAsync(animation);
		}

		public IAnimation Animation { get { return _obj.Animation; } }

		public bool Visible { get { return _obj.Visible; } set { _obj.Visible = value; } }

		public bool DebugDrawAnchor { get { return _obj.DebugDrawAnchor; } set { _obj.DebugDrawAnchor = value; } }

		public IBorderStyle Border { get { return _obj.Border; } set { _obj.Border = value; } }

		#endregion

		#region ISprite implementation

		public void PixelPerfect(bool pixelPerfect)
		{
			_obj.PixelPerfect(pixelPerfect);
		}

		public void ResetScale()
		{
			_obj.ResetScale();
		}

		public void ScaleBy(float scaleX, float scaleY)
		{
			_obj.ScaleBy(scaleX, scaleY);
		}

		public void ScaleTo(float width, float height)
		{
			_obj.ScaleTo(width, height);
		}

		public void FlipHorizontally()
		{
			_obj.FlipHorizontally();
		}

		public void FlipVertically()
		{
			_obj.FlipVertically();
		}

		public ISprite Clone()
		{
			return _obj.Clone();
		}

		public ILocation Location { get { return _obj.Location; } set { _obj.Location = value; } }

		public float X { get { return _obj.X; } set { _obj.X = value; } }

		public float Y { get { return _obj.Y; } set { _obj.Y = value; } }

		public float Z { get { return _obj.Z; } set { _obj.Z = value; } }

		public IArea PixelPerfectHitTestArea { get { return _obj.PixelPerfectHitTestArea; } }

		public float Height { get { return _obj.Height; } }

		public float Width { get { return _obj.Width; } }

		public float ScaleX { get { return _obj.ScaleX; } }

		public float ScaleY { get { return _obj.ScaleY; } }

		public float Angle { get { return _obj.Angle; } set { _obj.Angle = value; } }

		public byte Opacity { get { return _obj.Opacity; } set { _obj.Opacity = value; } }

		public Color Tint { get { return _obj.Tint; } set { _obj.Tint = value; } }

		public IPoint Anchor { get { return _obj.Anchor; } set { _obj.Anchor = value; } }

		public IImage Image { get { return _obj.Image; } set { _obj.Image = value; } }

		public IImageRenderer CustomRenderer { get { return _obj.CustomRenderer; } set { _obj.CustomRenderer = value; } }

		#endregion

		#region ISlider implementation

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
				if (_value == value) return;
				_value = value;
				refresh();
				OnValueChanged.Invoke(this, new SliderValueEventArgs (value));
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

		#endregion

		#region IInTree implementation

		public ITreeNode<IObject> TreeNode { get { return _obj.TreeNode; } }

		#endregion

		private void updateGraphics(IObject oldGraphics, IObject newGraphics, float z)
		{
			if (oldGraphics != null)
			{
				_state.UI.Remove(oldGraphics);
				oldGraphics.TreeNode.SetParent(null);
			}
			if (newGraphics == null) return;
			newGraphics.RenderLayer = RenderLayer;
			newGraphics.Z = z;
			newGraphics.TreeNode.SetParent(TreeNode);
			_state.UI.Add(newGraphics);
		}
			
		private void onRepeatedlyExecute(object sender, AGSEventArgs args)
		{
			if (BoundingBox == null || !_input.LeftMouseButtonDown || 
				!Graphics.BoundingBox.Contains(new AGSPoint(_input.MouseX, _input.MouseY)) ||  Graphics == null || HandleGraphics == null) 
				return;
			if (IsHorizontal) Value = getSliderValue(MathUtils.Clamp(_input.MouseX - BoundingBox.MinX, 0f, Graphics.Width));
			else Value = getSliderValue(MathUtils.Clamp(_input.MouseY - BoundingBox.MinY
				, 0f, Graphics.Height));
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
	}
}

