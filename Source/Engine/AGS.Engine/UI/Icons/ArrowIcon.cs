using System;
using AGS.API;

namespace AGS.Engine
{
    public class ArrowIcon : IBorderStyle
    {
        private readonly IGLUtils _glUtils;
        private Vector2 _emptyVector = new Vector2();

        public ArrowIcon(IGLUtils glUtils)
        {
            _glUtils = glUtils;
            ArrowColor = Colors.White.ToGLColor();
        }

        public enum ArrowDirection { Up, Down, Right, Left }

        public float WidthBottom { get { return 0f; } }
        public float WidthLeft { get { return 0f; } }
        public float WidthRight { get { return 0f; } }
        public float WidthTop { get { return 0f; } }

        public ArrowDirection Direction { get; set; }
        public IGLColor ArrowColor { get; set; }

        public void RenderBorderBack(ISquare square)
        {
            float width = square.MaxX - square.MinX;
            float height = square.MaxY - square.MinY;
            float arrowWidth = width * (1f/2f);
            float arrowHeight = height * (1f/2f);
            float remainingWidth = width - arrowWidth;
            float remainingHeight = height - arrowHeight;

            PointF point1, point2, point3;

            switch (Direction)
            {
                case ArrowDirection.Right:
                    point1 = square.TopLeft + new PointF(remainingWidth / 2f, -remainingHeight / 2f);
                    point2 = square.BottomLeft + new PointF(remainingWidth / 2f, remainingHeight / 2f);
                    point3 = square.BottomRight + new PointF(-remainingWidth / 2f, height / 2f);
                    break;
                case ArrowDirection.Down:
                    point1 = square.TopLeft + new PointF(remainingWidth / 2f, -remainingHeight / 2f);
                    point2 = square.BottomLeft + new PointF(width / 2f, remainingHeight / 2f);
                    point3 = square.TopRight + new PointF(-remainingWidth / 2f, -remainingHeight / 2f);
                    break;
                case ArrowDirection.Left:
                    point1 = square.TopRight + new PointF(-remainingWidth / 2f, -remainingHeight / 2f);
                    point2 = square.BottomRight + new PointF(-remainingWidth / 2f, remainingHeight / 2f);
                    point3 = square.BottomLeft + new PointF(remainingWidth / 2f, height / 2f);
                    break;
                case ArrowDirection.Up:
                    point1 = square.TopLeft + new PointF(width / 2f, -remainingHeight / 2f);
                    point2 = square.BottomLeft + new PointF(remainingWidth / 2f, remainingHeight / 2f);
                    point3 = square.BottomRight + new PointF(-remainingWidth / 2f, remainingHeight / 2f);
                    break;
                default: throw new NotSupportedException(Direction.ToString());
            }

            _glUtils.DrawTriangleFan(0, new GLVertex[] { new GLVertex(point1.ToVector2(), _emptyVector, ArrowColor),
                new GLVertex(point2.ToVector2(), _emptyVector, ArrowColor), new GLVertex(point3.ToVector2(), _emptyVector, ArrowColor)});
        }

        public void RenderBorderFront(ISquare square)
        {
        }
    }
}

