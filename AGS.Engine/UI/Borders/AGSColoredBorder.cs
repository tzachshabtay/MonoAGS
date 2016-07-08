using System;
using AGS.API;


namespace AGS.Engine
{
	public class AGSColoredBorder : IBorderStyle
	{
		private GLVertex[] _roundCorner;
		public static int ROUND_CORNER_SAMPLE_SIZE = 15;

		public AGSColoredBorder(float lineWidth, FourCorners<Color> color, FourCorners<bool> hasRoundCorner)
		{
			LineWidth = lineWidth;
			Color = color;
			HasRoundCorner = hasRoundCorner;
			_roundCorner = new GLVertex[ROUND_CORNER_SAMPLE_SIZE + 1];
		}

		public float LineWidth { get; set; }
		public FourCorners<Color> Color { get; set; }
		public FourCorners<bool> HasRoundCorner { get; set; }
		public bool DrawBorderBehind { get; set; }

        public float WidthLeft { get { return LineWidth; } }
        public float WidthRight { get { return LineWidth; } }
        public float WidthTop { get { return LineWidth; } }
        public float WidthBottom { get { return LineWidth; } }

		#region IBorderStyle implementation

		public void RenderBorderBack(ISquare square)
		{
			if (DrawBorderBehind) drawBorders(square);
		}

		public void RenderBorderFront(ISquare square)
		{
			if (!DrawBorderBehind) drawBorders(square);
		}

		#endregion

		private void drawBorders(ISquare square)
		{
			FourCorners<IGLColor> colors = Color.Convert(c => c.ToGLColor());

			float farLeft = square.TopLeft.X - LineWidth;
			float farRight = square.TopRight.X + LineWidth;
			float farTop = square.TopLeft.Y + LineWidth;
			float farBottom = square.BottomLeft.Y - LineWidth;

			AGSSquare border = new AGSSquare (new PointF (farLeft, farBottom), new PointF (farRight, farBottom),
				new PointF (farLeft, farTop), new PointF (farRight, farTop));

			float topQuadBottomY = square.TopLeft.Y;
			float topQuadLeftX = HasRoundCorner.TopLeft ? square.TopLeft.X : farLeft;
			float topQuadRightX = HasRoundCorner.TopRight ? square.TopRight.X : farRight;
			AGSSquare topQuad = new AGSSquare (new PointF (topQuadLeftX, topQuadBottomY), new PointF (topQuadRightX, topQuadBottomY),
				new PointF (topQuadLeftX, farTop), new PointF (topQuadRightX, farTop));

			float bottomQuadTopY = square.BottomLeft.Y;
			float bottomQuadLeftX = HasRoundCorner.BottomLeft ? square.BottomLeft.X : farLeft;
			float bottomQuadRightX = HasRoundCorner.BottomRight ? square.BottomRight.X : farRight;
			AGSSquare bottomQuad = new AGSSquare (new PointF (bottomQuadLeftX, farBottom), new PointF (bottomQuadRightX, farBottom),
				new PointF (bottomQuadLeftX, bottomQuadTopY), new PointF (bottomQuadRightX, bottomQuadTopY));

			float horizQuadTop = square.TopLeft.Y;
			float horizQuadBottom = square.BottomLeft.Y;

			float leftQuadRightX = square.BottomLeft.X;
			AGSSquare leftQuad = new AGSSquare (new PointF (farLeft, horizQuadBottom), new PointF (leftQuadRightX, horizQuadBottom),
				new PointF (farLeft, horizQuadTop), new PointF (leftQuadRightX, horizQuadTop));

			float rightQuadLeftX = square.BottomRight.X;
			AGSSquare rightQuad = new AGSSquare (new PointF (rightQuadLeftX, horizQuadBottom), new PointF (farRight, horizQuadBottom),
				new PointF (rightQuadLeftX, horizQuadTop), new PointF (farRight, horizQuadTop));

			if (HasRoundCorner.TopLeft) drawRoundCorner(square.TopLeft, LineWidth, 270f, border, colors);
			if (HasRoundCorner.TopRight) drawRoundCorner(square.TopRight, LineWidth, 0f, border, colors);
			if (HasRoundCorner.BottomLeft) drawRoundCorner(square.BottomLeft, LineWidth, 180f, border, colors);
			if (HasRoundCorner.BottomRight) drawRoundCorner(square.BottomRight, LineWidth, 90f, border, colors);
			drawQuad(topQuad, border, colors);
			drawQuad(bottomQuad, border, colors);
			drawQuad(leftQuad, border, colors);
			drawQuad(rightQuad, border, colors);
		}

		private void drawQuad(ISquare quad, ISquare border, FourCorners<IGLColor> colors)
		{
			GLColor bottomLeftColor = getColor(colors, border, quad.BottomLeft); 
			GLColor bottomRightColor = getColor(colors, border, quad.BottomRight);
			GLColor topRightColor = getColor(colors, border, quad.TopRight);
			GLColor topLeftColor = getColor(colors, border, quad.TopLeft);

			GLUtils.DrawQuad(0, quad.BottomLeft.ToVector3(), quad.BottomRight.ToVector3(),
				quad.TopLeft.ToVector3(), quad.TopRight.ToVector3(), bottomLeftColor, bottomRightColor, topLeftColor, topRightColor);
		}

		private void drawRoundCorner(PointF center, float radius, float angle, ISquare border, FourCorners<IGLColor> colors)
		{
			OpenTK.Vector2 tex = new OpenTK.Vector2 ();
			GLVertex centerVertex = new GLVertex (center.ToVector2(), tex, getColor(colors, border, center));
			_roundCorner[0] = centerVertex;
			float step = (90f / (_roundCorner.Length - 2));
			for (int i = 1; i < _roundCorner.Length; i++)
			{
				float anglerad = (float)Math.PI * angle / 180.0f;
				float x = (float)Math.Sin(anglerad) * radius; 
				float y = (float)Math.Cos(anglerad) * radius;
				angle += step;
				PointF point = new PointF (x + center.X, y + center.Y);
				_roundCorner[i] = new GLVertex (point.ToVector2(), tex, getColor(colors, border, point));
			}

			GLUtils.DrawTriangleFan(0, _roundCorner);
		}

		private GLColor getColor(FourCorners<IGLColor> colors, ISquare border, PointF point)
		{
			return getColor(colors, MathUtils.Lerp(border.BottomLeft.X, 0f, border.BottomRight.X, 1f, point.X),
				MathUtils.Lerp(border.BottomRight.Y, 0f, border.TopRight.Y, 1f, point.Y));
		}

		private GLColor getColor(FourCorners<IGLColor> color, float fractionX, float fractionY)
		{
			GLColor colorBottomX = getColor(color.BottomLeft, color.BottomRight, fractionX);
			GLColor colorTopX = getColor(color.TopLeft, color.TopRight, fractionX);
			return getColor(colorBottomX, colorTopX, fractionY);
		}
			
		private GLColor getColor(IGLColor color1, IGLColor color2, float fraction)
		{
			float r = color1.R + ((color2.R - color1.R) * fraction);
			float g = color1.G + ((color2.G - color1.G) * fraction);
			float b = color1.B + ((color2.B - color1.B) * fraction);
			float a = color1.A + ((color2.A - color1.A) * fraction);
			return new GLColor (r, g, b, a);
		}
	}
}

