using System;
using AGS.API;

namespace AGS.Engine
{
    public class SkiaBrushLoader : IBrushLoader
    {
        public SkiaBrushLoader()
        {
        }

        public IBrush LoadHatchBrush(HatchStyle hatchStyle, Color color, Color backgroundColor)
        {
            throw new NotImplementedException();
        }

        public IBrush LoadLinearBrush(Point point1, Point point2, Color color1, Color color2)
        {
            throw new NotImplementedException();
        }

        public IBrush LoadLinearBrush(Color[] linearColors, IBlend blend, IColorBlend interpolationColors, ITransformMatrix transform, WrapMode wrapMode, bool gammaCorrection)
        {
            throw new NotImplementedException();
        }

        public IBrush LoadPathsGradientBrush(Point[] points)
        {
            throw new NotImplementedException();
        }

        public IBrush LoadPathsGradientBrush(Color centerColor, PointF centerPoint, IBlend blend, PointF focusScales, Color[] surroundColors, IColorBlend interpolationColors, ITransformMatrix transform, WrapMode wrapMode)
        {
            throw new NotImplementedException();
        }

        public IBrush LoadSolidBrush(Color color)
        {
            return new SkiaBrush(color);
        }
    }
}