using AGS.API;

namespace AGS.Engine
{
    public class FolderIcon : IBorderStyle
    {
        /*     ++++++++
         *     **++++**
         *     ***++***
         *     ********
         *     ********
         *     ********
         */

        private readonly IGLUtils _glUtils;

        private IGLColor _color = Colors.Gold.ToGLColor();
        private IGLColor _foldColor = Colors.DarkGoldenrod.ToGLColor();
        private IGLColor _selectedColor = Colors.DeepSkyBlue.ToGLColor();
        private IGLColor _selectedFoldColor = Colors.Blue.ToGLColor();

        private Vector2 _emptyVector = new Vector2();

        public FolderIcon(IGLUtils glUtils)
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
            IGLColor color = IsSelected ? _selectedColor : _color;
            IGLColor foldColor = IsSelected ? _selectedFoldColor : _foldColor;

            _glUtils.DrawQuad(0, square.BottomLeft.ToVector3(), square.BottomRight.ToVector3(),
                    square.TopLeft.ToVector3(), square.TopRight.ToVector3(),
                    color, color, color, color);

            float foldHeight = (square.MaxY - square.MinY) * (1f / 5f);
            PointF foldBottom = new PointF((square.TopLeft.X + square.TopRight.X) / 2f, square.TopLeft.Y - foldHeight);

            _glUtils.DrawTriangle(0, new GLVertex[] { new GLVertex(square.TopLeft.ToVector2(), _emptyVector, foldColor),
                    new GLVertex(foldBottom.ToVector2(), _emptyVector, foldColor), new GLVertex(square.TopRight.ToVector2(), _emptyVector, foldColor)});
        }
    }
}

