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

                var propertyChanged = PropertyChanged;
                if (propertyChanged != null)
                {
                    bool hasChanged = false;
                    if (!MathUtils.FloatEquals(prevX, value.X))
                    {
                        hasChanged = true;
                        propertyChanged(this, _argsX);
                    }
                    if (!MathUtils.FloatEquals(prevY, value.Y))
                    {
                        hasChanged = true;
                        propertyChanged(this, _argsY);
                    }
                    if (!MathUtils.FloatEquals(prevZ, value.Z))
                    {
                        hasChanged = true;
                        propertyChanged(this, _argsZ);
                    }
                    if (hasChanged)
                    {
                        propertyChanged(this, _argsLocation);
                    }
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
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                _position = new Position(value, Y, Z == Y ? (float?)null : Z);
                var propertyChanged = PropertyChanged;
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (prevX != value && propertyChanged != null)
                {
                    propertyChanged(this, _argsX);
                    propertyChanged(this, _argsLocation);
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
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                _position = new Position(X, value, Z == Y ? value : Z);
                var propertyChanged = PropertyChanged;
                if (propertyChanged == null) return;
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (prevZ != _position.Z)
                {
                    propertyChanged(this, _argsZ);
                }
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (prevY != value)
                {
                    propertyChanged(this, _argsY);
                    propertyChanged(this, _argsLocation);
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
                _position = new Position(X, Y, value);
                var propertyChanged = PropertyChanged;
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (prevZ != value && propertyChanged != null)
                {
                    propertyChanged(this, _argsZ);
                    propertyChanged(this, _argsLocation);
                }
            }
        }

#pragma warning restore RECS0018 // Comparison of floating point numbers with equality operator

        public void Dispose()
        {
            PropertyChanged = null;
            _argsLocation = null;
            _argsX = null;
            _argsY = null;
            _argsZ = null;
        }
    }
}
