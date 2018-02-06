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

#pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067

        [AlsoNotifyFor(nameof(X), nameof(Y), nameof(Z))]
        public ILocation Location { get => _location; set => _location = value; }

        [Property(Browsable = false)]
        [AlsoNotifyFor(nameof(Location))]
        public float X { get => Location.X; set => _location = new AGSLocation(value, Y, Z); }

        [Property(Browsable = false)]
        [AlsoNotifyFor(nameof(Location))]
        public float Y { get => Location.Y; set => _location = new AGSLocation(X, value, Z == Y ? value : Z); }

        [Property(Browsable = false)]
        [AlsoNotifyFor(nameof(Location))]
        public float Z { get => Location.Z; set => _location = new AGSLocation(X, Y, value); }
    }
}
