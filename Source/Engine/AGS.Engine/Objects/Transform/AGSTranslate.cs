using AGS.API;

namespace AGS.Engine
{
    public class AGSTranslate : ITranslate
    {
        private readonly AGSEventArgs _args = new AGSEventArgs();
        private ILocation _location;

        public AGSTranslate()
        {
            Location = AGSLocation.Empty();
            OnLocationChanged = new AGSEvent<AGSEventArgs>();
        }

        public ILocation Location { get { return _location; } set { _location = value; fireLocationChange(); } }

        public float X { get { return Location.X; } set { Location = new AGSLocation(value, Y, Z); } }

        public float Y { get { return Location.Y; } set { Location = new AGSLocation(X, value, Z == Y ? value : Z); } }

        public float Z { get { return Location.Z; } set { Location = new AGSLocation(X, Y, value); } }

        public IEvent<AGSEventArgs> OnLocationChanged { get; private set; }

        private void fireLocationChange()
        {
            OnLocationChanged.FireEvent(this, _args);
        }
    }
}
