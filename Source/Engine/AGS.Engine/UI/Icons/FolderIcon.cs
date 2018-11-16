using AGS.API;

namespace AGS.Engine
{
    [ConcreteImplementation(Browsable = false)]
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

        private readonly GLColor _color = Colors.Gold.ToGLColor();
        private readonly GLColor _foldColor = Colors.DarkGoldenrod.ToGLColor();
        private readonly GLColor _selectedColor = Colors.DeepSkyBlue.ToGLColor();
        private readonly GLColor _selectedFoldColor = Colors.Blue.ToGLColor();

        private readonly Vector2 _emptyVector = new Vector2();
        private IFrameBuffer _frameBuffer;
        private readonly GLVertex[] _quad = new GLVertex[4];
        private bool _isSelected;

        private AGSBoundingBox _lastSquare;
        private Size _lastWindowSize;

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

        public float WidthBottom => 0f;
        public float WidthLeft => 0f;
        public float WidthRight => 0f;
        public float WidthTop => 0f;
        public bool IsSelected { get => _isSelected; set { if (_isSelected == value) return; _isSelected = value; _frameBuffer = null; } }

        public void RenderBorderFront(AGSBoundingBox square) { }

        public void RenderBorderBack(AGSBoundingBox square)
        {
            if (_settings.WindowSize.Equals(_lastWindowSize) && _lastSquare.SameSize(square)
                && _glUtils.DrawQuad(_frameBuffer, square, _quad)) return;

            _frameBuffer?.Dispose();
            _lastSquare = square;
            _lastWindowSize = _settings.WindowSize;

            float width = _glUtils.CurrentResolution.Width;
            float height = _glUtils.CurrentResolution.Height;
            GLColor color = IsSelected ? _selectedColor : _color;
            GLColor foldColor = IsSelected ? _selectedFoldColor : _foldColor;
            float foldHeight = height * (1f / 5f);
            PointF foldBottom = new PointF(width / 2f, foldHeight);

            _frameBuffer = _glUtils.BeginFrameBuffer(square, _settings);
            if (_frameBuffer == null) return;
            _glUtils.DrawQuad(0, new Vector3(0,height,0), new Vector3(width,height,0),
                              new Vector3(), new Vector3(width,0,0),
                    color, color, color, color);

            _glUtils.DrawTriangle(0, new[] { new GLVertex(new Vector2(), _emptyVector, foldColor),
                new GLVertex(foldBottom.ToVector2(), _emptyVector, foldColor), new GLVertex(new Vector2(width,0), _emptyVector, foldColor)});
            _frameBuffer.End();

            _glUtils.DrawQuad(_frameBuffer, square, _quad);
        }
    }
}

