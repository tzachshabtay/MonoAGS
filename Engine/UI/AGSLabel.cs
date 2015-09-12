using System;
using API;
using System.Threading.Tasks;
using System.Drawing;

namespace Engine
{
	public class AGSLabel : ILabel
	{
		private IObject _obj;
		private ILabelRenderer _labelRenderer;

		public AGSLabel(IObject obj, IUIEvents events, IImage image, ILabelRenderer labelRenderer)
		{
			this._obj = obj;
			Anchor = new AGSPoint ();
			Events = events;
			RenderLayer = AGSLayers.UI;
			Image = image;
			IgnoreViewport = true;
			_labelRenderer = labelRenderer;
			CustomRenderer = _labelRenderer;
		}

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

		#endregion

		#region IObject implementation

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

		public IRoom Room { get { return _obj.Room; } set { _obj.Room = value; } }

		public IAnimation Animation { get { return _obj.Animation; } }

		public IInteractions Interactions { get { return _obj.Interactions; } }

		public ISquare BoundingBox { get { return _obj.BoundingBox; } set { _obj.BoundingBox = value; } }

		public IRenderLayer RenderLayer { get { return _obj.RenderLayer; } set { _obj.RenderLayer = value; } }

		public ITreeNode<IObject> TreeNode { get { return _obj.TreeNode; } }

		public bool Visible { get { return _obj.Visible; } set { _obj.Visible = value; } }

		public bool Enabled { get { return _obj.Enabled; } set { _obj.Enabled = value; } }

		public string Hotspot { get { return _obj.Hotspot; } set { _obj.Hotspot = value; } }

		public bool IgnoreViewport { get { return _obj.IgnoreViewport; } set { _obj.IgnoreViewport = value; } }

		public bool DebugDrawAnchor { get { return _obj.DebugDrawAnchor; } set { _obj.DebugDrawAnchor = value; } }

		public IBorderStyle Border { get { return _obj.Border; } set { _obj.Border = value; } }

		#endregion

		#region ISprite implementation

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

		public ILocation Location { get { return _obj.Location; } set { _obj.Location = value; } }

		public float X { get { return _obj.X; } set { _obj.X = value; } }

		public float Y { get { return _obj.Y; } set { _obj.Y = value; } }

		public float Z { get { return _obj.Z; } set { _obj.Z = value; } }

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
	}
}

