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
        private AGSBoundingBox _lastSquare;
        private Size _lastWindowSize;

        public ArrowIcon(IGLUtils glUtils, IRuntimeSettings settings, ArrowDirection direction = default,
                        Color? color = null)
        {
            _glUtils = glUtils;
            _settings = settings;
            ArrowColor = (color ?? (Color?)Colors.White).Value.ToGLColor();
            _direction = direction;
        }

        public float WidthBottom => 0f;
        public float WidthLeft => 0f;
        public float WidthRight => 0f;
        public float WidthTop => 0f;

        public ArrowDirection Direction { get => _direction; set { if (_direction == value) return; _direction = value; _frameBuffer = null; } }
        public IGLColor ArrowColor { get => _color; set { if (_color == value) return; _color = value; _frameBuffer = null; } }

        public void RenderBorderBack(AGSBoundingBox square)
        {
        }

        public void RenderBorderFront(AGSBoundingBox square)
        {
            if (_settings.WindowSize.Equals(_lastWindowSize) && _lastSquare.SameSize(square) 
                && _glUtils.DrawQuad(_frameBuffer, square, _quad)) return;

            _frameBuffer?.Dispose();
            _lastSquare = square;
            _lastWindowSize = _settings.WindowSize;
            float width = _glUtils.CurrentResolution.Width;
            float height = _glUtils.CurrentResolution.Height;
            float arrowWidth = width * (1f / 2f);
            float arrowHeight = height * (1f / 2f);
            float remainingWidth = width - arrowWidth;
            float remainingHeight = height - arrowHeight;

            PointF point1, point2, point3;

            switch (Direction)
            {
                case ArrowDirection.Right:
                    point1 = new PointF(remainingWidth / 2f, remainingHeight / 2f);// square.TopLeft + new PointF(remainingWidth / 2f, -remainingHeight / 2f);
                    point2 = new PointF(remainingWidth / 2f, height - remainingHeight / 2f); //square.BottomLeft + new PointF(remainingWidth / 2f, remainingHeight / 2f);
                    point3 = new PointF(width - remainingWidth / 2f, height / 2f); //square.BottomRight + new PointF(-remainingWidth / 2f, height / 2f);
                    break;
                case ArrowDirection.Down:
                    point1 = new PointF(remainingWidth / 2f, remainingHeight / 2f);
                    point2 = new PointF(width / 2f, height - remainingHeight / 2f);
                    point3 = new PointF(width - remainingWidth / 2f, remainingHeight / 2f);
                    break;
                case ArrowDirection.Left:
                    point1 = new PointF(width - remainingWidth / 2f, height - remainingHeight / 2f);
                    point2 = new PointF(width - remainingWidth / 2f, remainingHeight / 2f);
                    point3 = new PointF(remainingWidth / 2f, height / 2f);
                    break;
                case ArrowDirection.Up:
                    point1 = new PointF(remainingWidth / 2f, height - remainingHeight / 2f);
                    point2 = new PointF(width / 2f, remainingHeight / 2f);
                    point3 = new PointF(width - remainingWidth / 2f, height - remainingHeight / 2f);
                    break;
                default: throw new NotSupportedException(Direction.ToString());
            }

            _frameBuffer = _glUtils.BeginFrameBuffer(square, _settings);
            if (_frameBuffer == null) return;
            _glUtils.DrawTriangle(0, new[] { new GLVertex((point1).ToVector2(), _emptyVector, ArrowColor),
                new GLVertex((point2).ToVector2(), _emptyVector, ArrowColor), new GLVertex((point3).ToVector2(), _emptyVector, ArrowColor)});
            _frameBuffer.End();

            _glUtils.DrawQuad(_frameBuffer, square, _quad);
        }
    }
}

