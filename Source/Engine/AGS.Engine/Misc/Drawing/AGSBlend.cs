using AGS.API;

namespace AGS.Engine
{
	public class AGSBlend : IBlend
	{
		public AGSBlend(float[] factors, float[] positions)
		{
			Factors = factors;
			Positions = positions;
		}

		#region IBlend implementation

		public float[] Factors { get; private set; }

		public float[] Positions { get; private set; }

		#endregion
	}
}

