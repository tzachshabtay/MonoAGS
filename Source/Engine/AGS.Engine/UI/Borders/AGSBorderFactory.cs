using System;
using System.Diagnostics;
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

        [MethodWizard]
        public IBorderStyle SolidColor([MethodParam(Default = 4278190080u)]Color color, float lineWidth = 10, bool hasRoundCorners = false)
        {
            return new AGSColoredBorder(_gLUtils, lineWidth, new FourCorners<Color>(color),
                new FourCorners<bool>(hasRoundCorners));
        }

        [MethodWizard]
        public IBorderStyle Gradient(FourCorners<Color> color, float lineWidth = 10, bool hasRoundCorners = false)
        {
            return new AGSColoredBorder(_gLUtils, lineWidth, color, new FourCorners<bool>(hasRoundCorners));
        }

        [MethodWizard]
        public IBorderStyle Multiple(params IBorderStyle[] borders) => new AGSMultipleBorders(borders);
    }
}