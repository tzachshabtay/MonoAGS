using AGS.API;

namespace AGS.Engine
{
    public class FileIcon : IBorderStyle
    {
        /*     ****+
         *     ****++
         *     ****+++
         *     ********
         *     ********
         *     ********
         */

        private readonly IGLUtils _glUtils;
        private readonly IRuntimeSettings _settings;

        private readonly IGLColor _color = Colors.OldLace.ToGLColor();
        private readonly IGLColor _foldColor = Colors.Gray.ToGLColor();

        private readonly IGLColor _selectedColor = Colors.DeepSkyBlue.ToGLColor();
        private readonly IGLColor _selectedFoldColor = Colors.Blue.ToGLColor();

        private readonly Vector2 _emptyVector = new Vector2();
        private IFrameBuffer _frameBuffer;
        private readonly GLVertex[] _quad = new GLVertex[4];
        private bool _isSelected;

        public FileIcon(IGLUtils glUtils, IRuntimeSettings settings)
        {
            _glUtils = glUtils;
            _settings = settings;
        }

        public float WidthBottom { get { return 0f; } }
        public float WidthLeft { get { return 0f; } }
        public float WidthRight { get { return 0f; } }
        public float WidthTop { get { return 0f; } }
        public bool IsSelected { get { return _isSelected; } set { if (_isSelected == value) return; _isSelected = value; _frameBuffer = null; } }

        public void RenderBorderFront(ISquare square) { }

        public void RenderBorderBack(ISquare square)
        {
            if (_glUtils.DrawQuad(_frameBuffer, square, _quad)) return;

            float width = _settings.VirtualResolution.Width;
            float height = _settings.VirtualResolution.Height;
            float foldWidth = (width) * (1f / 5f);
            float foldHeight = (height) * (1f / 5f);
            IGLColor color = IsSelected ? _selectedColor : _color;
            IGLColor foldColor = IsSelected ? _selectedFoldColor : _foldColor;

            Vector3 foldBottomLeft = new Vector3(width - foldWidth, foldHeight, 0);
            Vector3 foldTopLeft = new Vector3(width - foldWidth, 0, 0);
            Vector3 foldTopRight = new Vector3(width, foldHeight, 0);

            _frameBuffer = _glUtils.BeginFrameBuffer(square, _settings);
            _glUtils.DrawQuad(0, new Vector3(0f,height,0), new Vector3(width - foldWidth,height,0),
                new Vector3(), foldTopLeft,
                color, color, color, color);

            _glUtils.DrawQuad(0, new Vector3(width - foldWidth, height, 0), new Vector3(width,height,0),
                foldBottomLeft, foldTopRight,
                color, color, color, color);

            _glUtils.DrawTriangle(0, new GLVertex[] { new GLVertex(foldBottomLeft.Xy, _emptyVector, foldColor),
                new GLVertex(foldTopLeft.Xy, _emptyVector, foldColor), new GLVertex(foldTopRight.Xy, _emptyVector, foldColor)});
            _frameBuffer.End();

            _glUtils.DrawQuad(_frameBuffer, square, _quad);
        }
    }
}

