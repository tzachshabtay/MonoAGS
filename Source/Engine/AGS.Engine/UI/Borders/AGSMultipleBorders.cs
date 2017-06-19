using System.Collections.Generic;
using System.Linq;
using AGS.API;

namespace AGS.Engine
{
    public class AGSMultipleBorders : IBorderStyle
    {
        public AGSMultipleBorders(params IBorderStyle[] borders)
        {
            Borders = new List<IBorderStyle>(borders);
        }

        public List<IBorderStyle> Borders { get; private set; }

        public float WidthBottom { get { return Borders.Max(b => b.WidthBottom); } }

        public float WidthTop { get { return Borders.Max(b => b.WidthTop); } }

        public float WidthLeft { get { return Borders.Max(b => b.WidthLeft); } }

        public float WidthRight { get { return Borders.Max(b => b.WidthRight); } }

        public void RenderBorderBack(ISquare square)
        {
            for (int i = Borders.Count - 1; i >= 0; i--)
            {
                Borders[i].RenderBorderBack(square);
            }
        }

        public void RenderBorderFront(ISquare square)
        {
            foreach (var border in Borders)
            {
                border.RenderBorderFront(square);
            }
        }
    }
}
