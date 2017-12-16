using AGS.API;
using PropertyChanged;

namespace AGS.Engine
{
    [DoNotNotify]
    public class AGSTranslateComponent : AGSComponent, ITranslateComponent
    {
        private ITranslate _translate;

        public AGSTranslateComponent(ITranslate translate)
        {
            _translate = translate;
            _translate.PropertyChanged += (sender, e) => OnPropertyChanged(e);
        }

        [Property(Category = "Transform")]
        public ILocation Location { get => _translate.Location; set => _translate.Location = value; }

        [Property(Browsable = false)]
        public float X { get => _translate.X; set => _translate.X = value; }

        [Property(Browsable = false)]
        public float Y { get => _translate.Y; set => _translate.Y = value; }

        [Property(Browsable = false)]
        public float Z { get => _translate.Z; set => _translate.Z = value; }
    }
}
