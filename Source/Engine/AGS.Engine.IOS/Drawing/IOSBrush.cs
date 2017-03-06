using System;
using AGS.API;

namespace AGS.Engine.IOS
{
    public class IOSBrush : IBrush
    {
        public IOSBrush(Color color)
        {
            Color = color;
        }

        public Color BackgroundColor
        {
            get; private set;
        }

        public IBlend Blend
        {
            get; private set;
        }

        public PointF CenterPoint
        {
            get; private set;
        }

        public Color Color { get; private set; }

        public PointF FocusScales
        {
            get; private set;
        }

        public bool GammaCorrection
        {
            get; private set;
        }

        public HatchStyle HatchStyle
        {
            get; private set;
        }

        public IColorBlend InterpolationColors
        {
            get; private set;
        }

        public Color[] LinearColors
        {
            get; private set;
        }

        public ITransformMatrix Transform
        {
            get; private set;
        }

        public BrushType Type
        {
            get; private set;
        }

        public WrapMode WrapMode
        {
            get; private set;
        }
    }
}
