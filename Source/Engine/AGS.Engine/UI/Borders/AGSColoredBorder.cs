using System;
using AGS.API;

namespace AGS.Engine
{
    [PropertyFolder]
	public class AGSColoredBorder : IBorderStyle
	{
		private GLVertex[] _roundCorner;
		public static int ROUND_CORNER_SAMPLE_SIZE = 15;
        private readonly IGLUtils _glUtils;
        private readonly IGameSettings _settings;
        private IFrameBuffer _frameBuffer;
        private readonly GLVertex[] _quad = new GLVertex[4];

        private FourCorners<Color> _colors;
        private FourCorners<IGLColor> _glColors;

        private AGSBoundingBox _lastSquare;
        private Size _lastWindowSize;

        public AGSColoredBorder(IGLUtils glUtils, IGameSettings settings, float lineWidth, FourCorners<Color> color, FourCorners<bool> hasRoundCorner)
		{
            _glUtils = glUtils;
            _settings = settings;
			LineWidth = lineWidth;
			Color = color;
			HasRoundCorner = hasRoundCorner;
			_roundCorner = new GLVertex[ROUND_CORNER_SAMPLE_SIZE + 1];
		}

		public float LineWidth { get; set; }
		public FourCorners<Color> Color 
        {
            get => _colors;
            set
            {
                _colors = value;
                _glColors = value.Convert(c => c.ToGLColor());
            }
        }
		public FourCorners<bool> HasRoundCorner { get; set; }
		public bool DrawBorderBehind { get; set; }

        public float WidthLeft => LineWidth;
        public float WidthRight => LineWidth;
        public float WidthTop => LineWidth;
        public float WidthBottom => LineWidth;

        #region IBorderStyle implementation

        public void RenderBorderBack(AGSBoundingBox square)
		{
			if (DrawBorderBehind) drawBorders(square);
		}

		public void RenderBorderFront(AGSBoundingBox square)
		{
			if (!DrawBorderBehind) drawBorders(square);
		}

		#endregion

        private void drawBorders(AGSBoundingBox square)
		{
            var screenSquare = new AGSBoundingBox(0f, _glUtils.CurrentResolution.Width, 0f, _glUtils.CurrentResolution.Height);

            if (_settings.WindowSize.Equals(_lastWindowSize) && _lastSquare.Equals(square)
                && _glUtils.DrawQuad(_frameBuffer, screenSquare, _quad))
                return;

            _frameBuffer?.Dispose();
            _lastSquare = square;
            _lastWindowSize = _settings.WindowSize;

            FourCorners<IGLColor> colors = _glColors;

            float farBottomLeftX = square.BottomLeft.X - LineWidth;
            float farBottomLeftY = _glUtils.CurrentResolution.Height - square.BottomLeft.Y - LineWidth;
            float farBottomRightX = square.BottomRight.X + LineWidth;
            float farBottomRightY = _glUtils.CurrentResolution.Height - square.BottomRight.Y - LineWidth;
            float farTopLeftX = square.TopLeft.X - LineWidth;
            float farTopLeftY = _glUtils.CurrentResolution.Height - square.TopLeft.Y + LineWidth;
            float farTopRightX = square.TopRight.X + LineWidth;
            float farTopRightY = _glUtils.CurrentResolution.Height - square.TopRight.Y + LineWidth;

            float topLeftX = square.TopLeft.X;
            float topRightX = square.TopRight.X;
            float topLeftY = _glUtils.CurrentResolution.Height - square.TopLeft.Y;
            float topRightY = _glUtils.CurrentResolution.Height - square.TopRight.Y;

            float bottomLeftX = square.BottomLeft.X;
            float bottomRightX = square.BottomRight.X;
            float bottomLeftY = _glUtils.CurrentResolution.Height - square.BottomLeft.Y;
            float bottomRightY = _glUtils.CurrentResolution.Height - square.BottomRight.Y;

            AGSBoundingBox colorBox = new AGSBoundingBox(new Vector2(farBottomLeftX, farBottomLeftY), new Vector2(farBottomRightX, farBottomRightY),
                new Vector2(farTopLeftX, farTopLeftY), new Vector2(farTopRightX, farTopRightY));

            float topQuadLeftX = HasRoundCorner.TopLeft ? topLeftX : farTopLeftX;
            float topQuadRightX = HasRoundCorner.TopRight ? topRightX : farTopRightX;
            AGSBoundingBox topQuad = new AGSBoundingBox(
                new Vector2(topQuadLeftX, topLeftY), new Vector2(topQuadRightX, topRightY),
                new Vector2(topQuadLeftX, farTopLeftY), new Vector2(topQuadRightX, farTopRightY));

            float bottomQuadLeftX = HasRoundCorner.BottomLeft ? bottomLeftX : farBottomLeftX;
            float bottomQuadRightX = HasRoundCorner.BottomRight ? bottomRightX : farBottomRightX;
            AGSBoundingBox bottomQuad = new AGSBoundingBox(
                new Vector2(bottomQuadLeftX, farBottomLeftY), new Vector2(bottomQuadRightX, farBottomRightY),
                new Vector2(bottomQuadLeftX, bottomLeftY), new Vector2(bottomQuadRightX, bottomRightY));

            float leftQuadBottomY = bottomLeftY;
            float leftQuadTopY = topLeftY;
            AGSBoundingBox leftQuad = new AGSBoundingBox(
                new Vector2(farBottomLeftX, leftQuadBottomY), new Vector2(bottomLeftX, leftQuadBottomY),
                new Vector2(farTopLeftX, leftQuadTopY), new Vector2(topLeftX, leftQuadTopY));

            float rightQuadBottomY = bottomRightY;
            float rightQuadTopY = topRightY;
            AGSBoundingBox rightQuad = new AGSBoundingBox(
                new Vector2(bottomRightX, rightQuadBottomY), new Vector2(farBottomRightX, rightQuadBottomY),
                new Vector2(topRightX, rightQuadTopY), new Vector2(farTopRightX, rightQuadTopY));

            _frameBuffer = _glUtils.BeginFrameBuffer(screenSquare, _settings);
            if (_frameBuffer == null)
                return;
            if (HasRoundCorner.TopLeft) drawRoundCorner(new Vector3(topLeftX, topLeftY, 0f), LineWidth, 270f, colorBox, colors);
            if (HasRoundCorner.TopRight) drawRoundCorner(new Vector3(topRightX, topRightY, 0f), LineWidth, 0f, colorBox, colors);
            if (HasRoundCorner.BottomLeft) drawRoundCorner(new Vector3(bottomLeftX, bottomLeftY, 0f), LineWidth, 180f, colorBox, colors);
            if (HasRoundCorner.BottomRight) drawRoundCorner(new Vector3(bottomRightX, bottomRightY, 0f), LineWidth, 90f, colorBox, colors);

            drawQuad(topQuad, colorBox, colors);
            drawQuad(bottomQuad, colorBox, colors);
            drawQuad(leftQuad, colorBox, colors);
            drawQuad(rightQuad, colorBox, colors);
            _frameBuffer.End();

            _glUtils.DrawQuad(_frameBuffer, screenSquare, _quad);
        }

		private void drawQuad(AGSBoundingBox quad, AGSBoundingBox border, FourCorners<IGLColor> colors)
		{
			IGLColor bottomLeftColor = getColor(colors, border, quad.BottomLeft); 
			IGLColor bottomRightColor = getColor(colors, border, quad.BottomRight);
			IGLColor topRightColor = getColor(colors, border, quad.TopRight);
			IGLColor topLeftColor = getColor(colors, border, quad.TopLeft);

            _glUtils.DrawQuad(0, quad.BottomLeft, quad.BottomRight,
				quad.TopLeft, quad.TopRight, bottomLeftColor, bottomRightColor, topLeftColor, topRightColor);
		}

        private void drawRoundCorner(Vector3 center, float radius, float angle, AGSBoundingBox border, FourCorners<IGLColor> colors)
		{
			Vector2 tex = new Vector2 ();
            GLVertex centerVertex = new GLVertex (center.Xy, tex, getColor(colors, border, center));
			_roundCorner[0] = centerVertex;
			float step = (90f / (_roundCorner.Length - 2));
			for (int i = 1; i < _roundCorner.Length; i++)
			{
				float anglerad = (float)Math.PI * angle / 180.0f;
				float x = (float)Math.Sin(anglerad) * radius; 
				float y = (float)Math.Cos(anglerad) * radius;
				angle += step;
                Vector3 point = new Vector3 (x + center.X, y + center.Y, 0f);
                _roundCorner[i] = new GLVertex (point.Xy, tex, getColor(colors, border, point));
			}

            _glUtils.DrawTriangleFan(0, _roundCorner);
		}

        private IGLColor getColor(FourCorners<IGLColor> colors, AGSBoundingBox border, Vector3 point)
		{
			return getColor(colors, MathUtils.Lerp(border.BottomLeft.X, 0f, border.BottomRight.X, 1f, point.X),
				MathUtils.Lerp(border.BottomRight.Y, 0f, border.TopRight.Y, 1f, point.Y));
		}

		private IGLColor getColor(FourCorners<IGLColor> color, float fractionX, float fractionY)
		{
			IGLColor colorBottomX = getColor(color.BottomLeft, color.BottomRight, fractionX);
			IGLColor colorTopX = getColor(color.TopLeft, color.TopRight, fractionX);
			return getColor(colorBottomX, colorTopX, fractionY);
		}
			
		private IGLColor getColor(IGLColor color1, IGLColor color2, float fraction)
		{
            if (color1.Equals(color2)) return color1;
			float r = color1.R + ((color2.R - color1.R) * fraction);
			float g = color1.G + ((color2.G - color1.G) * fraction);
			float b = color1.B + ((color2.B - color1.B) * fraction);
			float a = color1.A + ((color2.A - color1.A) * fraction);
			return new GLColor (r, g, b, a);
		}
	}
}
