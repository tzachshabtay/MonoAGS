using AGS.API;

namespace AGS.Engine
{
    public class AGSRotateComponent : AGSComponent, IRotateComponent
    {
        private readonly IRotate _rotate;

        public AGSRotateComponent(IRotate rotate)
        {
            _rotate = rotate;
        }

        [Property(Category = "Transform", CategoryZ = -100, CategoryExpand = true)]
        [NumberEditorSlider(sliderMin: -180f, sliderMax: 180f)]
        public float Angle { get => _rotate.Angle; set => _rotate.Angle = value; }
    }
}
