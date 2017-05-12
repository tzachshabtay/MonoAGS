using AGS.API;

namespace AGS.Engine
{
	public class AGSWalkBehindArea : AGSComponent, IWalkBehindArea
	{
		#region IWalkBehindArea implementation

		public float? Baseline { get; set; }

		#endregion
	}
}

