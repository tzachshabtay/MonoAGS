using System.ComponentModel;
using AGS.API;
using PropertyChanged;

namespace AGS.Engine
{
    public class AGSTranslate : ITranslate
    {
        private Position _position;
        private PropertyChangedEventArgs _argsX, _argsY, _argsZ, _argsLocation;

        public AGSTranslate()
        {
            _position = Position.Empty;
            _argsX = new PropertyChangedEventArgs(nameof(X));
            _argsY = new PropertyChangedEventArgs(nameof(Y));
            _argsZ = new PropertyChangedEventArgs(nameof(Z));
            _argsLocation = new PropertyChangedEventArgs(nameof(Position));
        }

#pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067

#pragma warning disable RECS0018 // Comparison of floating point numbers with equality operator
        [DoNotNotify]
        public Position Position 
        { 
            get => _position; 
            set
            {
                float prevX = _position.X;
                float prevY = _position.Y;
                float prevZ = _position.Z;

                _position = value;

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
            get => _position.X; 
            set
            {
                var prevX = _position.X;
                _position = new Position(value, Y, Z == Y ? (float?)null : Z);
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
            get => _position.Y;
            set
            {
                float prevY = _position.Y;
                float prevZ = _position.Z;
                _position = new Position(X, value, Z == Y ? (float?)null : Z);
                if (prevZ != _position.Z)
                {
                    PropertyChanged(this, _argsZ);
                }
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
            get => _position.Z;
            set 
            {
                float prevZ = _position.Z;
                _position = new Position(X, Y, value == Y ? (float?)null : value);
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