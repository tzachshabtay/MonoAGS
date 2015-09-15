using System;
using AGS.API;
using System.Drawing;

namespace AGS.Engine
{
	public class AGSSprite : ISprite
	{
		private IImage _image;

		public AGSSprite ()
		{
			ScaleX = 1;
			ScaleY = 1;
			Anchor = new AGSPoint ();

			Tint = Color.White;
			Location = new AGSLocation ();
		}

		#region ISprite implementation

		public void ResetScale()
		{
			ScaleX = 1;
			ScaleY = 1;
			Width = Image.Width;
			Height = Image.Height;
		}

		public void ScaleBy (float scaleX, float scaleY)
		{
			ScaleX = scaleX;
			ScaleY = scaleY;
			Width = Image.Width * ScaleX;
			Height = Image.Height * ScaleY;
		}

		public void ScaleTo (float width, float height)
		{
			Width = width;
			Height = height;
			ScaleX = Width / Image.Width;
			ScaleY = Height / Image.Height;
		}

		public void FlipHorizontally()
		{
			ScaleBy(-ScaleX, ScaleY);
			Anchor = new AGSPoint (-Anchor.X, Anchor.Y);
		}

		public void FlipVertically()
		{
			ScaleBy(ScaleX, -ScaleY);
			Anchor = new AGSPoint (Anchor.X, -Anchor.Y);
		}

		public ISprite Clone()
		{
			return (ISprite)MemberwiseClone();
		}

		public ILocation Location { get; set; }

		public float X { get { return Location.X; } set { Location = new AGSLocation(value, Y, Z); } }

		public float Y { get { return Location.Y; } set { Location = new AGSLocation(X, value, Z == Y ? value : Z); } }

		public float Z { get { return Location.Z; } set { Location = new AGSLocation(X, Y, value); } }

		public float Height { get; private set; }

		public float Width { get; private set; }

		public float ScaleX { get; private set; }

		public float ScaleY { get; private set; }

		public float Angle { get; set; }

		public byte Opacity 
		{ 
			get { return Tint.A; }
			set { Color.FromArgb(value, Tint); }
		}

		public Color Tint { get; set; }

		public IPoint Anchor { get; set; }

		public IImage Image 
		{ 
			get { return _image; }
			set 
			{
				_image = value;
				ScaleBy (ScaleX, ScaleY);
			}
		}

		public IImageRenderer CustomRenderer { get; set; }

		#endregion

		public override string ToString()
		{
			return _image == null ? base.ToString() : _image.ToString();
		}
	}
}

