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
        private FourCorners<Color> _color;
        private FourCorners<GLColor> _glColor;

        public AGSColoredBorder(IGLUtils glUtils, float lineWidth, FourCorners<Color> color, FourCorners<bool> hasRoundCorner)
        {
            _glUtils = glUtils;
            LineWidth = lineWidth;
            Color = color;
            HasRoundCorner = hasRoundCorner;
            _roundCorner = new GLVertex[ROUND_CORNER_SAMPLE_SIZE + 1];
        }

        public float LineWidth { get; set; }
        public FourCorners<Color> Color { get => _color; set { _color = value; _glColor = Color.Convert(c => c.ToGLColor()); } }
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

        public override string ToString()
        {
            if (Color.IsOneValue) return $"Solid {Color.TopLeft.ToString()}";
            return "Multiple Colors";
        }

        private void drawBorders(AGSBoundingBox square)
        {
            FourCorners<GLColor> colors = _glColor;

            float farBottomLeftX = square.BottomLeft.X - LineWidth;
            float farBottomLeftY = square.BottomLeft.Y - LineWidth;
            float farBottomRightX = square.BottomRight.X + LineWidth;
            float farBottomRightY = square.BottomRight.Y - LineWidth;
            float farTopLeftX = square.TopLeft.X - LineWidth;
            float farTopLeftY = square.TopLeft.Y + LineWidth;
            float farTopRightX = square.TopRight.X + LineWidth;
            float farTopRightY = square.TopRight.Y + LineWidth;

            AGSBoundingBox colorBox = new AGSBoundingBox(new Vector2(farBottomLeftX, farBottomLeftY), new Vector2(farBottomRightX, farBottomRightY),
                new Vector2(farTopLeftX, farTopLeftY), new Vector2(farTopRightX, farTopRightY));

            float topQuadLeftX = HasRoundCorner.TopLeft ? square.TopLeft.X : farTopLeftX;
            float topQuadRightX = HasRoundCorner.TopRight ? square.TopRight.X : farTopRightX;
            AGSBoundingBox topQuad = new AGSBoundingBox(
                new Vector2(topQuadLeftX, square.TopLeft.Y), new Vector2(topQuadRightX, square.TopRight.Y),
                new Vector2(topQuadLeftX, farTopLeftY), new Vector2(topQuadRightX, farTopRightY));

            float bottomQuadLeftX = HasRoundCorner.BottomLeft ? square.BottomLeft.X : farBottomLeftX;
            float bottomQuadRightX = HasRoundCorner.BottomRight ? square.BottomRight.X : farBottomRightX;
            AGSBoundingBox bottomQuad = new AGSBoundingBox(
                new Vector2(bottomQuadLeftX, farBottomLeftY), new Vector2(bottomQuadRightX, farBottomRightY),
                new Vector2(bottomQuadLeftX, square.BottomLeft.Y), new Vector2(bottomQuadRightX, square.BottomRight.Y));

            float leftQuadBottomY = square.BottomLeft.Y;
            float leftQuadTopY = square.TopLeft.Y;
            AGSBoundingBox leftQuad = new AGSBoundingBox(
                new Vector2(farBottomLeftX, leftQuadBottomY), new Vector2(square.BottomLeft.X, leftQuadBottomY),
                new Vector2(farTopLeftX, leftQuadTopY), new Vector2(square.TopLeft.X, leftQuadTopY));

            float rightQuadBottomY = square.BottomRight.Y;
            float rightQuadTopY = square.TopRight.Y;
            AGSBoundingBox rightQuad = new AGSBoundingBox(
                new Vector2(square.BottomRight.X, rightQuadBottomY), new Vector2(farBottomRightX, rightQuadBottomY),
                new Vector2(square.TopRight.X, rightQuadTopY), new Vector2(farTopRightX, rightQuadTopY));

            if (HasRoundCorner.TopLeft) drawRoundCorner(square.TopLeft, LineWidth, 270f, colorBox, colors);
            if (HasRoundCorner.TopRight) drawRoundCorner(square.TopRight, LineWidth, 0f, colorBox, colors);
            if (HasRoundCorner.BottomLeft) drawRoundCorner(square.BottomLeft, LineWidth, 180f, colorBox, colors);
            if (HasRoundCorner.BottomRight) drawRoundCorner(square.BottomRight, LineWidth, 90f, colorBox, colors);
            drawQuad(topQuad, colorBox, colors);
            drawQuad(bottomQuad, colorBox, colors);
            drawQuad(leftQuad, colorBox, colors);
            drawQuad(rightQuad, colorBox, colors);
        }

        private void drawQuad(AGSBoundingBox quad, AGSBoundingBox border, FourCorners<GLColor> colors)
        {
            GLColor bottomLeftColor = getColor(colors, border, quad.BottomLeft);
            GLColor bottomRightColor = getColor(colors, border, quad.BottomRight);
            GLColor topRightColor = getColor(colors, border, quad.TopRight);
            GLColor topLeftColor = getColor(colors, border, quad.TopLeft);

            _glUtils.DrawQuad(0, quad.BottomLeft, quad.BottomRight,
                quad.TopLeft, quad.TopRight, bottomLeftColor, bottomRightColor, topLeftColor, topRightColor);
        }

        private void drawRoundCorner(Vector3 center, float radius, float angle, AGSBoundingBox border, FourCorners<GLColor> colors)
        {
            Vector2 tex = new Vector2();
            GLVertex centerVertex = new GLVertex(center.Xy, tex, getColor(colors, border, center));
            _roundCorner[0] = centerVertex;
            float step = (90f / (_roundCorner.Length - 2));
            for (int i = 1; i < _roundCorner.Length; i++)
            {
                float anglerad = (float)Math.PI * angle / 180.0f;
                float x = (float)Math.Sin(anglerad) * radius;
                float y = (float)Math.Cos(anglerad) * radius;
                angle += step;
                Vector3 point = new Vector3(x + center.X, y + center.Y, 0f);
                _roundCorner[i] = new GLVertex(point.Xy, tex, getColor(colors, border, point));
            }

            _glUtils.DrawTriangleFan(0, _roundCorner);
        }

        private GLColor getColor(FourCorners<GLColor> colors, AGSBoundingBox border, Vector3 point)
        {
            return getColor(colors, MathUtils.Lerp(border.BottomLeft.X, 0f, border.BottomRight.X, 1f, point.X),
                MathUtils.Lerp(border.BottomRight.Y, 0f, border.TopRight.Y, 1f, point.Y));
        }

        private GLColor getColor(FourCorners<GLColor> color, float fractionX, float fractionY)
        {
            GLColor colorBottomX = getColor(color.BottomLeft, color.BottomRight, fractionX);
            GLColor colorTopX = getColor(color.TopLeft, color.TopRight, fractionX);
            return getColor(colorBottomX, colorTopX, fractionY);
        }

        private GLColor getColor(GLColor color1, GLColor color2, float fraction)
        {
            float r = color1.R + ((color2.R - color1.R) * fraction);
            float g = color1.G + ((color2.G - color1.G) * fraction);
            float b = color1.B + ((color2.B - color1.B) * fraction);
            float a = color1.A + ((color2.A - color1.A) * fraction);
            return new GLColor(r, g, b, a);
        }
    }
}