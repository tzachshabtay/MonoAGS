using System;
using AGS.API;

namespace AGS.Engine
{
	public static class AGSBorders
	{
		public static AGSColoredBorder SolidColor(Color? color = null, float lineWidth = 10f, bool hasRoundCorners = false)
		{
            return SolidColor(AGSGame.GLUtils, AGSGame.Game.Settings, color, lineWidth, hasRoundCorners);
		}

        public static AGSColoredBorder SolidColor(IGLUtils glUtils, IGameSettings settings, Color? color = null, float lineWidth = 10f, bool hasRoundCorners = false)
        {
            return new AGSColoredBorder(glUtils, lineWidth, new FourCorners<Color>(color == null ? Colors.Black : color.Value),
                new FourCorners<bool>(hasRoundCorners));
        }

		public static AGSColoredBorder Gradient(FourCorners<Color> color, float lineWidth = 10f, bool hasRoundCorners = false)
		{
            return Gradient(AGSGame.GLUtils, AGSGame.Game.Settings, color, lineWidth, hasRoundCorners);
		}

        public static AGSColoredBorder Gradient(IGLUtils glUtils, IGameSettings settings, FourCorners<Color> color, float lineWidth = 10f, bool hasRoundCorners = false)
        {
            return new AGSColoredBorder(glUtils, lineWidth, color, new FourCorners<bool>(hasRoundCorners));
        }

        public static AGSMultipleBorders Multiple(params IBorderStyle[] borders)
        {
            return new AGSMultipleBorders(borders);
        }
	}
}

