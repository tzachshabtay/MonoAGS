using AGS.API;

namespace AGS.Engine
{
	public class AGSWalkBehindArea : AGSComponent, IWalkBehindArea
	{
        public static IArea Create(string id, IMask mask)
        {
            var area = new AGSArea(id, AGSGame.Resolver) { Mask = mask };
            area.AddComponent<IWalkBehindArea>();
            return area;
        }

		#region IWalkBehindArea implementation

		public float? Baseline { get; set; }

		#endregion
	}
}

