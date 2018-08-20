using System.Collections.Generic;
using System.Linq;
using AGS.API;

namespace AGS.Engine
{
    [PropertyFolder]
    [ConcreteImplementation(Browsable = false)]
    public class AGSMultipleBorders : IBorderStyle
    {
        public AGSMultipleBorders(params IBorderStyle[] borders)
        {
            Borders = new List<IBorderStyle>(borders);
        }

        public List<IBorderStyle> Borders { get; }

        public float WidthBottom => Borders.Max(b => b.WidthBottom);

        public float WidthTop => Borders.Max(b => b.WidthTop);

        public float WidthLeft => Borders.Max(b => b.WidthLeft);

        public float WidthRight => Borders.Max(b => b.WidthRight);

        public void RenderBorderBack(AGSBoundingBox square)
        {
            for (int i = Borders.Count - 1; i >= 0; i--)
            {
                Borders[i].RenderBorderBack(square);
            }
        }

        public void RenderBorderFront(AGSBoundingBox square)
        {
            foreach (var border in Borders)
            {
                border.RenderBorderFront(square);
            }
        }

        public override string ToString() => string.Join(" + ", Borders);
    }
}