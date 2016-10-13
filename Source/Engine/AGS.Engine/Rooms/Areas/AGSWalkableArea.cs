using AGS.API;

namespace AGS.Engine
{
    public class AGSWalkableArea : AGSComponent, IWalkableArea
    {
        public AGSWalkableArea()
        {
            IsWalkable = true;
        }

        public static IArea Create(string id, IMask mask)
        {
            var area = new AGSArea(id, AGSGame.Resolver) { Mask = mask };
            area.AddComponent<IWalkableArea>();
            return area;
        }

        public bool IsWalkable { get; set; }
    }
}
