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

        private IGLColor _color = Colors.OldLace.ToGLColor();
        private IGLColor _foldColor = Colors.Gray.ToGLColor();

        private IGLColor _selectedColor = Colors.DeepSkyBlue.ToGLColor();
        private IGLColor _selectedFoldColor = Colors.Blue.ToGLColor();

        private Vector2 _emptyVector = new Vector2();

        public FileIcon(IGLUtils glUtils)
        {
            _glUtils = glUtils;
        }

        public float WidthBottom { get { return 0f; } }
        public float WidthLeft { get { return 0f; } }
        public float WidthRight { get { return 0f; } }
        public float WidthTop { get { return 0f; } }
        public bool IsSelected { get; set; }

        public void RenderBorderFront(ISquare square) { }

        public void RenderBorderBack(ISquare square)
        {
            float foldWidth = (square.MaxX - square.MinX) * (1f / 5f);
            float foldHeight = (square.MaxY - square.MinY) * (1f / 5f);
            IGLColor color = IsSelected ? _selectedColor : _color;
            IGLColor foldColor = IsSelected ? _selectedFoldColor : _foldColor;

            PointF foldBottomLeft = square.TopRight - new PointF(foldWidth, foldHeight);
            PointF foldTopLeft = square.TopRight - new PointF(foldWidth, 0f);
            PointF foldTopRight = square.TopRight - new PointF(0f, foldHeight);

            _glUtils.DrawQuad(0, square.BottomLeft.ToVector3(), (square.BottomRight - new PointF(foldWidth, 0f)).ToVector3(),
                square.TopLeft.ToVector3(), foldTopLeft.ToVector3(),
                color, color, color, color);

            _glUtils.DrawQuad(0, (square.BottomRight - new PointF(foldWidth, 0f)).ToVector3(), square.BottomRight.ToVector3(),
                foldBottomLeft.ToVector3(), foldTopRight.ToVector3(),
                color, color, color, color);

            _glUtils.DrawTriangleFan(0, new GLVertex[] { new GLVertex(foldBottomLeft.ToVector2(), _emptyVector, foldColor),
                    new GLVertex(foldTopLeft.ToVector2(), _emptyVector, foldColor), new GLVertex(foldTopRight.ToVector2(), _emptyVector, foldColor)});
        }
    }
}

