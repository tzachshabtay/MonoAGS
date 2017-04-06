using AGS.API;

namespace AGS.Engine
{
    public class AGSTranslateComponent : AGSComponent, ITranslateComponent
    {
        private ITranslate _translate;

        public AGSTranslateComponent(ITranslate transform)
        {
            _translate = transform;
        }

        public ILocation Location { get { return _translate.Location; } set { _translate.Location = value; } }

        public float X { get { return _translate.X; } set { _translate.X = value; } }

        public float Y { get { return _translate.Y; } set { _translate.Y = value; } }

        public float Z { get { return _translate.Z; } set { _translate.Z = value; } }

        public IEvent<AGSEventArgs> OnLocationChanged { get { return _translate.OnLocationChanged; } }
    }
}
