using System;
using API;

namespace Engine
{
	public class AGSTextObject : IObject
	{
		public AGSTextObject()
		{
		}

		#region IObject implementation

		public void StartAnimation(IAnimation animation)
		{
			throw new NotImplementedException();
		}

		public AnimationCompletedEventArgs Animate(IAnimation animation)
		{
			throw new NotImplementedException();
		}

		public System.Threading.Tasks.Task<AnimationCompletedEventArgs> AnimateAsync(IAnimation animation)
		{
			throw new NotImplementedException();
		}

		public IRoom Room
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public IAnimation Animation
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public IInteractions Interactions
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public ISquare BoundingBox
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public IRenderLayer RenderLayer
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public bool Visible
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public bool Enabled
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public string Hotspot
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public bool IgnoreViewport
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public bool DebugDrawAnchor
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public IBorderStyle Border
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		#endregion

		#region ISprite implementation

		public void ResetScale()
		{
			throw new NotImplementedException();
		}

		public void ScaleBy(float scaleX, float scaleY)
		{
			throw new NotImplementedException();
		}

		public void ScaleTo(float width, float height)
		{
			throw new NotImplementedException();
		}

		public ILocation Location
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public float X
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public float Y
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public float Z
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public float Height
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public float Width
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public float ScaleX
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public float ScaleY
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public float Angle
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public byte Opacity
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public System.Drawing.Color Tint
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public IPoint Anchor
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public IImage Image
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public IImageRenderer CustomRenderer
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		#endregion

		#region IInTree implementation

		public ITreeNode<IObject> TreeNode
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		#endregion
	}
}

