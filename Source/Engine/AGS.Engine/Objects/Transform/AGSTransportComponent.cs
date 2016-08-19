using AGS.API;

namespace AGS.Engine
{
    public class AGSTransportComponent : AGSComponent, ITranslateComponent
    {
        private ITranslate _transform;

        public AGSTransportComponent(ITranslate transform)
        {
            _transform = transform;
        }

        public ILocation Location { get { return _transform.Location; } set { _transform.Location = value; } }

        public float X { get { return _transform.X; } set { _transform.X = value; } }

        public float Y { get { return _transform.Y; } set { _transform.Y = value; } }

        public float Z { get { return _transform.Z; } set { _transform.Z = value; } }
    }
}
