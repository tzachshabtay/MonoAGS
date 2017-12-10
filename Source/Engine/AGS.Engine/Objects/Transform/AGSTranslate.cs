using System.ComponentModel;
using AGS.API;
using PropertyChanged;

namespace AGS.Engine
{
    public class AGSTranslate : ITranslate
    {
        private ILocation _location;

        public AGSTranslate()
        {
            _location = AGSLocation.Empty();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [AlsoNotifyFor(nameof(X), nameof(Y), nameof(Z))]
        public ILocation Location { get { return _location; } set { _location = value; } }

        [Property(Browsable = false)]
        [AlsoNotifyFor(nameof(Location))]
        public float X { get { return Location.X; } set { _location = new AGSLocation(value, Y, Z); } }

        [Property(Browsable = false)]
        [AlsoNotifyFor(nameof(Location))]
        public float Y { get { return Location.Y; } set { _location = new AGSLocation(X, value, Z == Y ? value : Z); } }

        [Property(Browsable = false)]
        [AlsoNotifyFor(nameof(Location))]
        public float Z { get { return Location.Z; } set { _location = new AGSLocation(X, Y, value); } }
    }
}
