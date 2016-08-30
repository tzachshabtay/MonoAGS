using System;
using AGS.API;

namespace AGS.Engine
{
    //todo: create interface for matrix render target with just the required properties
    public class LabelMatrixRenderTarget : IHasModelMatrix
	{
		public LabelMatrixRenderTarget()
		{
		}

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

		public void FlipHorizontally()
		{
			throw new NotImplementedException();
		}

		public void FlipVertically()
		{
			throw new NotImplementedException();
		}

		public void ResetBaseSize(float initialWidth, float initialHeight)
        {
            throw new NotImplementedException();
        }

        public void ResetScale(float initialWidth, float initialHeight)
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

		public Color Tint
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

		public PointF Anchor { get; set; }

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

        public IEvent<AGSEventArgs> OnImageChanged
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}

