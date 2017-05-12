using AGS.API;

namespace AGS.Engine
{
    public class AGSWalkableArea : AGSComponent, IWalkableArea
    {
        public AGSWalkableArea()
        {
            IsWalkable = true;
        }

        public bool IsWalkable { get; set; }
    }
}
