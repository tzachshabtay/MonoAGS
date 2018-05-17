using System;
using AGS.API;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing.Drawing.Brushes;

namespace AGS.Engine
{
    public class ImageSharpBrush : IBrush
    {
        public ImageSharpBrush(SolidBrush<Rgba32> brush)
        {
            Color = brush.Color.Convert();
            Type = BrushType.Solid;
            InnerBrush = brush;
        }

        public IBrush<Rgba32> InnerBrush { get; }

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