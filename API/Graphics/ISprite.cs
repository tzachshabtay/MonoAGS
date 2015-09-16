using System;
using System.Drawing;

namespace AGS.API
{
	public interface ISprite
	{
		ILocation Location { get; set; }
		float X { get; set; }
		float Y { get; set; }
		float Z { get; set; }

		IArea PixelPerfectHitTestArea { get; } //This is the area not projected on the screen (i.e not rotated and will start from 0,0)
		void PixelPerfect(bool pixelPerfect);

		float Height { get; }
		float Width { get; }
		float ScaleX { get; }
		float ScaleY { get; }
		float Angle { get; set; }
		byte Opacity { get; set; }
		Color Tint { get; set; }
		IPoint Anchor { get; set; }

		IImage Image { get; set; }
		IImageRenderer CustomRenderer { get; set; }

		void ResetScale();
		void ScaleBy(float scaleX, float scaleY);
		void ScaleTo(float width, float height);
		void FlipHorizontally();
		void FlipVertically();
		ISprite Clone();
	}
}

