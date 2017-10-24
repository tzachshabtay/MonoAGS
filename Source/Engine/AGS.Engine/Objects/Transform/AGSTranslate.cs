using AGS.API;

namespace AGS.Engine
{
    public class AGSTranslate : ITranslate
    {
        private ILocation _location;

        public AGSTranslate()
        {
            _location = AGSLocation.Empty();
            OnLocationChanged = new AGSEvent();
        }

        public ILocation Location { get { return _location; } set { _location = value; fireLocationChange(); } }

        [Property(Browsable = false)]
        public float X { get { return Location.X; } 
            set { if (value == X) return; Location = new AGSLocation(value, Y, Z); } }

        [Property(Browsable = false)]
        public float Y { get { return Location.Y; } set { if (value == Y) return; Location = new AGSLocation(X, value, Z == Y ? value : Z); } }

        [Property(Browsable = false)]
        public float Z { get { return Location.Z; } set { if (value == Z) return; Location = new AGSLocation(X, Y, value); } }

        public IEvent OnLocationChanged { get; private set; }

        private void fireLocationChange()
        {
            OnLocationChanged.Invoke();
        }
    }
}
