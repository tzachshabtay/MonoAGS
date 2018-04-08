using System.ComponentModel;
using AGS.API;
using PropertyChanged;

namespace AGS.Engine
{
    public class AGSTranslate : ITranslate
    {
        private ILocation _location;
        private PropertyChangedEventArgs _argsX, _argsY, _argsZ, _argsLocation;

        public AGSTranslate()
        {
            _location = AGSLocation.Empty();
            _argsX = new PropertyChangedEventArgs(nameof(X));
            _argsY = new PropertyChangedEventArgs(nameof(Y));
            _argsZ = new PropertyChangedEventArgs(nameof(Z));
            _argsLocation = new PropertyChangedEventArgs(nameof(Location));
        }

#pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067

#pragma warning disable RECS0018 // Comparison of floating point numbers with equality operator
        [DoNotNotify]
        public ILocation Location 
        { 
            get => _location; 
            set
            {
                float prevX = _location.X;
                float prevY = _location.Y;
                float prevZ = _location.Z;

                _location = value;

                bool hasChanged = false;
                if (prevX != value.X)
                {
                    hasChanged = true;
                    PropertyChanged(this, _argsX);
                }
                if (prevY != value.Y)
                {
                    hasChanged = true;
                    PropertyChanged(this, _argsY);
                }
                if (prevZ != value.Z)
                {
                    hasChanged = true;
                    PropertyChanged(this, _argsZ);
                }
                if (hasChanged)
                {
                    PropertyChanged(this, _argsLocation);
                }
            }
        }

        [Property(Browsable = false)]
        [DoNotNotify]
        public float X 
        { 
            get => Location.X; 
            set
            {
                var prevX = _location.X;
                _location = new AGSLocation(value, Y, Z);
                if (prevX != value)
                {
                    PropertyChanged(this, _argsX);
                    PropertyChanged(this, _argsLocation);
                }
            }
        }

        [Property(Browsable = false)]
        [DoNotNotify]
        public float Y
        {
            get => Location.Y;
            set
            {
                float prevY = _location.Y;
                _location = new AGSLocation(X, value, Z == Y ? value : Z);
                if (prevY != value)
                {
                    PropertyChanged(this, _argsY);
                    PropertyChanged(this, _argsLocation);
                }
            }
        }

        [Property(Browsable = false)]
        [DoNotNotify]
        public float Z
        {
            get => Location.Z;
            set 
            {
                float prevZ = _location.Z;
                _location = new AGSLocation(X, Y, value);
                if (prevZ != value)
                {
                    PropertyChanged(this, _argsZ);
                    PropertyChanged(this, _argsLocation);
                }
            }
        }

#pragma warning restore RECS0018 // Comparison of floating point numbers with equality operator
    }
}
