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
            get
            {
                throw new NotImplementedException();
            }
        }

        public IBlend Blend
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public PointF CenterPoint
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public Color Color { get; private set; }

        public PointF FocusScales
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool GammaCorrection
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public HatchStyle HatchStyle
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IColorBlend InterpolationColors
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public Color[] LinearColors
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public ITransformMatrix Transform
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public BrushType Type
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public WrapMode WrapMode
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
