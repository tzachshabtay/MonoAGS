using System;
using AGS.API;
using System.Threading.Tasks;
using System.Drawing;
using Autofac;

namespace AGS.Engine
{
	public class AGSLabel : ILabel
	{
		private readonly IPanel _obj;
		private readonly ILabelRenderer _labelRenderer;
		private IHasRoom _roomBehavior;
		private readonly VisibleProperty _visible;
		private readonly EnabledProperty _enabled;

		public AGSLabel(IPanel obj, IImage image, ILabelRenderer labelRenderer, SizeF baseSize, Resolver resolver)
		{
			this._obj = obj;
			_visible = new VisibleProperty (this);
			_enabled = new EnabledProperty (this);
			Enabled = false;
			_labelRenderer = labelRenderer;
			_labelRenderer.BaseSize = baseSize;
			CustomRenderer = _labelRenderer;
			TreeNode = new AGSTreeNode<IObject> (this);

			TypedParameter panelParam = new TypedParameter (typeof(IObject), this);
			_roomBehavior = resolver.Container.Resolve<IHasRoom>(panelParam);
			Events = resolver.Container.Resolve<IUIEvents>(panelParam);
		}

		public string ID { get { return _obj.ID; } }

		#region IUIControl implementation

		public void ApplySkin(ILabel skin)
		{
			throw new NotImplementedException();
		}

		public IUIEvents Events { get; private set; }

		#endregion

		#region ILabel implementation

		public ITextConfig TextConfig 
		{
			get { return _labelRenderer.Config; }
			set { _labelRenderer.Config = value; }
		}

		public string Text 
		{ 
			get { return _labelRenderer.Text; }
			set { _labelRenderer.Text = value; }
		}


		public float TextHeight { get { return _labelRenderer.TextHeight; } }

		public float TextWidth { get { return _labelRenderer.TextWidth; } }

		#endregion

		#region IObject implementation

		public ICustomProperties Properties { get { return _obj.Properties; } }

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

		public void ChangeRoom(IRoom room, float? x = null, float? y = null)
		{
			_roomBehavior.ChangeRoom(room, x, y);
		}

		public bool CollidesWith(float x, float y)
		{
			return _obj.CollidesWith(x, y);
		}
			
		public IRoom Room { get { return _roomBehavior.Room; } }

		public IRoom PreviousRoom { get { return _roomBehavior.PreviousRoom; } }

		public IAnimation Animation { get { return _obj.Animation; } }

		public IInteractions Interactions { get { return _obj.Interactions; } }

		public ISquare BoundingBox { get { return _obj.BoundingBox; } set { _obj.BoundingBox = value; } }

		public void PixelPerfect(bool pixelPerfect)
		{
			_obj.PixelPerfect(pixelPerfect);
		}

		public IArea PixelPerfectHitTestArea
		{
			get
			{
				return _obj.PixelPerfectHitTestArea;
			}
		}

		public IRenderLayer RenderLayer { get { return _obj.RenderLayer; } set { _obj.RenderLayer = value; } }

		public ITreeNode<IObject> TreeNode { get; private set; }

		public bool Visible { get { return _visible.Value; } set { _visible.Value = value; } }

		public bool Enabled { get { return _enabled.Value; } set { _enabled.Value = value; } }

		public bool UnderlyingVisible { get { return _visible.UnderlyingValue; } }

		public bool UnderlyingEnabled { get { return _enabled.UnderlyingValue; } }

		public string Hotspot { get { return _obj.Hotspot; } set { _obj.Hotspot = value; } }

		public bool IgnoreViewport { get { return _obj.IgnoreViewport; } set { _obj.IgnoreViewport = value; } }
		public bool IgnoreScalingArea { get { return _obj.IgnoreScalingArea; } set { _obj.IgnoreScalingArea = value; } }

		public IPoint WalkPoint { get { return _obj.WalkPoint; } set { _obj.WalkPoint = value; } }
		public IPoint CenterPoint { get { return _obj.CenterPoint; } }

		public bool DebugDrawAnchor { get { return _obj.DebugDrawAnchor; } set { _obj.DebugDrawAnchor = value; } }

		public IBorderStyle Border { get { return _obj.Border; } set { _obj.Border = value; } }

		#endregion

		#region ISprite implementation

		public void ResetScale()
		{
			_obj.ResetScale();
		}

		public void ResetScale(float initialWidth, float initialHeight)
		{
			_obj.ResetScale(initialWidth, initialHeight);
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

		public float Height { get { return _labelRenderer.Height; } }

		public float Width { get { return _labelRenderer.Width; } }

		public float ScaleX { get { return _obj.ScaleX; } }

		public float ScaleY { get { return _obj.ScaleY; } }

		public float Angle { get { return _obj.Angle; } set { _obj.Angle = value; } }

		public byte Opacity { get { return _obj.Opacity; } set { _obj.Opacity = value; } }

		public Color Tint { get { return _obj.Tint; } set { _obj.Tint = value; } }

		public IPoint Anchor { get { return _obj.Anchor; } set { _obj.Anchor = value; } }

		public IImage Image { get { return _obj.Image; } set { _obj.Image = value; } }

		public IImageRenderer CustomRenderer { get { return _obj.CustomRenderer; } set { _obj.CustomRenderer = value; } }

		#endregion

		public override string ToString()
		{
			return string.Format("Label: {0}", ID ?? (string.IsNullOrWhiteSpace(Text) ? _obj.ToString() : Text));
		}

		public void Dispose()
		{
			_obj.Dispose();
		}
	}
}

