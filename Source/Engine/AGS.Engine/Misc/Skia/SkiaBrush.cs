using System;
using AGS.API;

namespace AGS.Engine
{
    public class SkiaBrush : IBrush
    {
        public SkiaBrush(Color color)
        {
            Type = BrushType.Solid;
            Color = color;
        }

        public BrushType Type { get; }

        public Color Color { get; }

        public IBlend Blend { get; }

        public bool GammaCorrection { get; }

        public IColorBlend InterpolationColors { get; }

        public Color[] LinearColors { get; }

        public ITransformMatrix Transform { get; }

        public WrapMode WrapMode { get; }

        public Color BackgroundColor { get; }

        public HatchStyle HatchStyle { get; }

        public PointF CenterPoint { get; }

        public PointF FocusScales { get; }
    }
}