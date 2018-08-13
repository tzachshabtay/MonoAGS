using System;
using AGS.API;

namespace AGS.Engine
{
	public class AGSBorderFactory : IBorderFactory
    {
        private readonly IGLUtils _gLUtils;

        public AGSBorderFactory(IGLUtils glUtils)
        {
            _gLUtils = glUtils;
        }

        public IBorderStyle SolidColor(Color? color = null, float lineWidth = 10, bool hasRoundCorners = false)
        {
            return new AGSColoredBorder(_gLUtils, lineWidth, new FourCorners<Color>(color == null ? Colors.Black : color.Value),
                new FourCorners<bool>(hasRoundCorners));
        }

        public IBorderStyle Gradient(FourCorners<Color> color, float lineWidth = 10, bool hasRoundCorners = false)
        {
            return new AGSColoredBorder(_gLUtils, lineWidth, color, new FourCorners<bool>(hasRoundCorners));
        }

        public IBorderStyle Multiple(params IBorderStyle[] borders) => new AGSMultipleBorders(borders);
    }
}