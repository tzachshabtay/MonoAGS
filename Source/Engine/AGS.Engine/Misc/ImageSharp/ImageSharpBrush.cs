using System;
using AGS.API;
using SixLabors.ImageSharp.Drawing.Processing;

namespace AGS.Engine
{
    public class ImageSharpBrush : API.IBrush
    {
        public ImageSharpBrush(SolidBrush brush)
        {
            Color = brush.Color.Convert();
            Type = BrushType.Solid;
            InnerBrush = brush;
        }

        public SixLabors.ImageSharp.Drawing.Processing.IBrush InnerBrush { get; }

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