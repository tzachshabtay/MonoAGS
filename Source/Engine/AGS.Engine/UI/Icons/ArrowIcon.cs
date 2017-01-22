using System;
using AGS.API;

namespace AGS.Engine
{
    public class ArrowIcon : IBorderStyle
    {
        private readonly IGLUtils _glUtils;
        private Vector2 _emptyVector = new Vector2();
        private IFrameBuffer _frameBuffer;
        private GLVertex[] _quad = new GLVertex[4];
        private IRuntimeSettings _settings;
        private ArrowDirection _direction;
        private IGLColor _color;

        public ArrowIcon(IGLUtils glUtils, IRuntimeSettings settings)
        {
            _glUtils = glUtils;
            _settings = settings;
            ArrowColor = Colors.White.ToGLColor();
        }

        public enum ArrowDirection { Up, Down, Right, Left }

        public float WidthBottom { get { return 0f; } }
        public float WidthLeft { get { return 0f; } }
        public float WidthRight { get { return 0f; } }
        public float WidthTop { get { return 0f; } }

        public ArrowDirection Direction { get { return _direction; } set { if (_direction == value) return; _direction = value; _frameBuffer = null; } }
        public IGLColor ArrowColor { get { return _color; } set { if (_color == value) return; _color = value; _frameBuffer = null; } }

        public void RenderBorderBack(ISquare square)
        {
            if (_glUtils.DrawQuad(_frameBuffer, square, _quad)) return;

            float width = _settings.VirtualResolution.Width;
            float height = _settings.VirtualResolution.Height;
            float arrowWidth = width * (1f/2f);
            float arrowHeight = height * (1f/2f);
            float remainingWidth = width - arrowWidth;
            float remainingHeight = height - arrowHeight;

            PointF point1, point2, point3;

            switch (Direction)
            {
                case ArrowDirection.Right:
                    point1 = new PointF(remainingWidth/2f, remainingHeight/2f);// square.TopLeft + new PointF(remainingWidth / 2f, -remainingHeight / 2f);
                    point2 = new PointF(remainingWidth/2f, height - remainingHeight/2f); //square.BottomLeft + new PointF(remainingWidth / 2f, remainingHeight / 2f);
                    point3 = new PointF(width - remainingWidth/2f, height/2f); //square.BottomRight + new PointF(-remainingWidth / 2f, height / 2f);
                    break;
                case ArrowDirection.Down:
                    point1 = new PointF(remainingWidth/2f, remainingHeight/2f);
                    point2 = new PointF(width/2f, height - remainingHeight/2f);
                    point3 = new PointF(width - remainingWidth/2f, remainingHeight/2f);
                    break;
                case ArrowDirection.Left:
                    point1 = new PointF(width - remainingWidth/2f,height - remainingHeight/2f);
                    point2 = new PointF(width - remainingWidth/2f, remainingHeight/2f);
                    point3 = new PointF(remainingWidth/2f,height/2f);
                    break;
                case ArrowDirection.Up:
                    point1 = new PointF(remainingWidth/2f, height - remainingHeight/2f);
                    point2 = new PointF(width/2f, remainingHeight/2f);
                    point3 = new PointF(width - remainingWidth/2f, height - remainingHeight/2f);
                    break;
                default: throw new NotSupportedException(Direction.ToString());
            }

            _frameBuffer = _glUtils.BeginFrameBuffer(square, _settings);
            _glUtils.DrawTriangle(0, new GLVertex[] { new GLVertex((point1).ToVector2(), _emptyVector, ArrowColor),
                new GLVertex((point2).ToVector2(), _emptyVector, ArrowColor), new GLVertex((point3).ToVector2(), _emptyVector, ArrowColor)});
            _frameBuffer.End();

            _glUtils.DrawQuad(_frameBuffer, square, _quad);
        }

        public void RenderBorderFront(ISquare square)
        {
        }
    }
}

