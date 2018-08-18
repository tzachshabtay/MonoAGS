using AGS.API;

namespace AGS.Engine
{
    public class XIcon : IBorderStyle
    {
        private readonly IGLUtils _glUtils;
        private readonly IRuntimeSettings _settings;

        private readonly GLColor _color;

        private IFrameBuffer _frameBuffer;
        private readonly GLVertex[] _quad = new GLVertex[4];
        private readonly float _lineWidth, _padding;

        private AGSBoundingBox _lastSquare;
        private Size _lastWindowSize;

        public XIcon(IGLUtils glUtils, IRuntimeSettings settings, float lineWidth, float padding, Color? color)
        {
            _glUtils = glUtils;
            _settings = settings;
            color = color ?? Colors.Red;
            _color = color.Value.ToGLColor();
            _lineWidth = lineWidth;
            _padding = padding;
        }

        public float WidthBottom => 0f;
        public float WidthLeft => 0f;
        public float WidthRight => 0f;
        public float WidthTop => 0f;

        public void RenderBorderBack(AGSBoundingBox square) { }

        public void RenderBorderFront(AGSBoundingBox square)
        {
            if (_settings.WindowSize.Equals(_lastWindowSize) && _lastSquare.SameSize(square)
                && _glUtils.DrawQuad(_frameBuffer, square, _quad)) return;

            _frameBuffer?.Dispose();
            _lastSquare = square;
            _lastWindowSize = _settings.WindowSize;

            float width = _glUtils.CurrentResolution.Width - _padding;
            float height = _glUtils.CurrentResolution.Height - _padding;

            _frameBuffer = _glUtils.BeginFrameBuffer(square, _settings);
            if (_frameBuffer == null) return;
            _glUtils.DrawLine(_padding, _padding, width, height, _lineWidth, _color.R, _color.G, _color.B, _color.A);
            _glUtils.DrawLine(_padding, height, width, _padding, _lineWidth, _color.R, _color.G, _color.B, _color.A);

            _frameBuffer.End();

            _glUtils.DrawQuad(_frameBuffer, square, _quad);
        }
    }
}
