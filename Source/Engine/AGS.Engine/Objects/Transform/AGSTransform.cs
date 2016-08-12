using AGS.API;

namespace AGS.Engine
{
    public class AGSTransform : ITransform
    {
        public AGSTransform()
        {
            Location = AGSLocation.Empty();
        }

        public ILocation Location { get; set; }

        public float X { get { return Location.X; } set { Location = new AGSLocation(value, Y, Z); } }

        public float Y { get { return Location.Y; } set { Location = new AGSLocation(X, value, Z == Y ? value : Z); } }

        public float Z { get { return Location.Z; } set { Location = new AGSLocation(X, Y, value); } }
    }
}
