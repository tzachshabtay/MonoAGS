using AGS.API;

namespace AGS.Engine
{
    public class FolderIcon : ISelectableIcon
    {
        /*     ++++++++
         *     **++++**
         *     ***++***
         *     ********
         *     ********
         *     ********
         */

        private readonly IGLUtils _glUtils;
        private readonly IRuntimeSettings _settings;

        private readonly IGLColor _color = Colors.Gold.ToGLColor();
        private readonly IGLColor _foldColor = Colors.DarkGoldenrod.ToGLColor();
        private readonly IGLColor _selectedColor = Colors.DeepSkyBlue.ToGLColor();
        private readonly IGLColor _selectedFoldColor = Colors.Blue.ToGLColor();

        private readonly Vector2 _emptyVector = new Vector2();
        private IFrameBuffer _frameBuffer;
        private readonly GLVertex[] _quad = new GLVertex[4];
        private bool _isSelected;

        public FolderIcon(IGLUtils glUtils, IRuntimeSettings settings, Color? color = null, Color? foldColor = null, 
                          Color? selectedColor = null, Color? selectedFoldColor = null)
        {
            _glUtils = glUtils;
            _settings = settings;

            _color = (color ?? (Color?)Colors.Gold).Value.ToGLColor();
            _foldColor = (foldColor ?? (Color?)Colors.DarkGoldenrod).Value.ToGLColor();
            _selectedColor = (selectedColor ?? (Color?)Colors.DeepSkyBlue).Value.ToGLColor();
            _selectedFoldColor = (selectedFoldColor ?? (Color?)Colors.Blue).Value.ToGLColor();
        }

        public float WidthBottom { get { return 0f; } }
        public float WidthLeft { get { return 0f; } }
        public float WidthRight { get { return 0f; } }
        public float WidthTop { get { return 0f; } }
        public bool IsSelected { get { return _isSelected; } set { if (_isSelected == value) return; _isSelected = value; _frameBuffer = null; } }

        public void RenderBorderFront(AGSSquare square) { }

        public void RenderBorderBack(AGSSquare square)
        {
            if (_glUtils.DrawQuad(_frameBuffer, square, _quad)) return;

            float width = _glUtils.CurrentResolution.Width;
            float height = _glUtils.CurrentResolution.Height;
            IGLColor color = IsSelected ? _selectedColor : _color;
            IGLColor foldColor = IsSelected ? _selectedFoldColor : _foldColor;
            float foldHeight = height * (1f / 5f);
            PointF foldBottom = new PointF(width / 2f, foldHeight);

            _frameBuffer = _glUtils.BeginFrameBuffer(square, _settings);
            if (_frameBuffer == null) return;
            _glUtils.DrawQuad(0, new Vector3(0,height,0), new Vector3(width,height,0),
                              new Vector3(), new Vector3(width,0,0),
                    color, color, color, color);

            _glUtils.DrawTriangle(0, new GLVertex[] { new GLVertex(new Vector2(), _emptyVector, foldColor),
                new GLVertex(foldBottom.ToVector2(), _emptyVector, foldColor), new GLVertex(new Vector2(width,0), _emptyVector, foldColor)});
            _frameBuffer.End();

            _glUtils.DrawQuad(_frameBuffer, square, _quad);
        }
    }
}

