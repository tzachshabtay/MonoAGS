using AGS.API;

namespace AGS.Engine
{
	public class AGSColorBlend : IColorBlend
	{
		public AGSColorBlend(Color[] colors, float[] positions)
		{
			Colors = colors;
			Positions = positions;
		}

		#region IColorBlend implementation

		public Color[] Colors { get; private set; }

		public float[] Positions { get; private set; }

		#endregion
	}
}

