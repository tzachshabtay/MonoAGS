using System;
using AGS.API;

namespace AGS.Engine
{
	//todo: create interface for matrix render target with just the required properties
	public class LabelMatrixRenderTarget : ISprite
	{
		public LabelMatrixRenderTarget()
		{
		}

		#region ISprite implementation

		public void PixelPerfect(bool pixelPerfect)
		{
			throw new NotImplementedException();
		}

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

		public void FlipHorizontally()
		{
			throw new NotImplementedException();
		}

		public void FlipVertically()
		{
			throw new NotImplementedException();
		}

		public ISprite Clone()
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

		public float X { get; set; }

		public float Y { get; set; }

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

		public IArea PixelPerfectHitTestArea
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public float Height { get; set; }

		public float Width { get; set; }

		public float ScaleX { get; set; }

		public float ScaleY { get; set; }

		public float Angle { get; set; }

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

		public IPoint Anchor { get; set; }

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
	}
}

