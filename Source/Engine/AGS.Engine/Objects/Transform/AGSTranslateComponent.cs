using AGS.API;
using PropertyChanged;

namespace AGS.Engine
{
    public class AGSTranslateComponent : AGSComponent, ITranslateComponent
    {
        private ITranslate _translate;

        public AGSTranslateComponent(ITranslate transform)
        {
            _translate = transform;
        }

        [Property(Category = "Transform")]
        public ILocation Location { get { return _translate.Location; } set { _translate.Location = value; } }

        [Property(Browsable = false)]
        [AlsoNotifyFor(nameof(Location))]
        public float X { get { return _translate.X; } 
            set { _translate.X = value; } }

        [Property(Browsable = false)]
        [AlsoNotifyFor(nameof(Location))]
        public float Y { get { return _translate.Y; } set { _translate.Y = value; } }

        [Property(Browsable = false)]
        [AlsoNotifyFor(nameof(Location))]
        public float Z { get { return _translate.Z; } set { _translate.Z = value; } }

        public IEvent OnLocationChanged { get { return _translate.OnLocationChanged; } }
    }
}
