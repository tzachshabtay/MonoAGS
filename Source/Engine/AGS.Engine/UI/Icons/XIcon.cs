using AGS.API;

namespace AGS.Engine
{
    public class XIcon : IBorderStyle
    {
        private readonly IGLUtils _glUtils;
        private readonly IRuntimeSettings _settings;

        private readonly IGLColor _color;

        private IFrameBuffer _frameBuffer;
        private readonly GLVertex[] _quad = new GLVertex[4];
        private readonly float _lineWidth, _padding;

        public XIcon(IGLUtils glUtils, IRuntimeSettings settings, float lineWidth, float padding, Color? color)
        {
            _glUtils = glUtils;
            _settings = settings;
            color = color ?? Colors.Red;
            _color = color.Value.ToGLColor();
            _lineWidth = lineWidth;
            _padding = padding;
        }

        public float WidthBottom { get { return 0f; } }
        public float WidthLeft { get { return 0f; } }
        public float WidthRight { get { return 0f; } }
        public float WidthTop { get { return 0f; } }

        public void RenderBorderFront(AGSSquare square) { }

        public void RenderBorderBack(AGSSquare square)
        {
            if (_glUtils.DrawQuad(_frameBuffer, square, _quad)) return;

            float width = _glUtils.CurrentResolution.Width - _padding;
            float height = _glUtils.CurrentResolution.Height - _padding;

            _frameBuffer = _glUtils.BeginFrameBuffer(square, _settings);
            if (_frameBuffer == null) return;
            _glUtils.DrawLine(_padding, _padding, width, height, _lineWidth, _color.R, _color.G, _color.B, _color.A);
            _glUtils.DrawLine(_padding, height, width, 0f, _lineWidth, _color.R, _color.G, _color.B, _color.A);

            _frameBuffer.End();

            _glUtils.DrawQuad(_frameBuffer, square, _quad);
        }
    }
}
