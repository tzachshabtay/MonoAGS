using System.ComponentModel;
using AGS.API;
using PropertyChanged;

namespace AGS.Engine
{
    [RequiredComponent(typeof(IBoundingBoxComponent))]
    [RequiredComponent(typeof(IImageComponent))]
    public class AGSWorldPositionComponent : AGSComponent, IWorldPositionComponent
    {
        private float _worldX, _worldY;
        private IBoundingBoxComponent _box;
        private IImageComponent _image;

		public override void Init(IEntity entity)
		{
            base.Init(entity);
            entity.Bind<IBoundingBoxComponent>(
                c => { _box = c; c.OnBoundingBoxesChanged.Subscribe(refresh); refresh(); },
                _ => _box = null);
            entity.Bind<IImageComponent>(
                c => { _image = c; c.PropertyChanged += onImagePropertyChanged; refresh(); },
                _ => _image = null);
		}

        [DoNotNotify]
		public float WorldX 
        { 
            get => _worldX;
            //todo: set
        }

        [DoNotNotify]
        public float WorldY 
        { 
            get => _worldY;
            //todo: set
        }

        private void refresh()
        {
            var boxComponent = _box;
            var image = _image;
            if (boxComponent == null || image == null) return;
            var box = _box.HitTestBoundingBox;
            var pivot = _image.Pivot;
            var oldX = _worldX;
            var oldY = _worldY;
            _worldX = MathUtils.Lerp(0f, box.MinX, 1f, box.MaxX, pivot.X);
            _worldY = MathUtils.Lerp(0f, box.MinY, 1f, box.MaxY, pivot.Y);
            if (!MathUtils.FloatEquals(oldX, _worldX))
            {
                OnPropertyChanged(nameof(WorldX));
            }
            if (!MathUtils.FloatEquals(oldY, _worldY))
            {
                OnPropertyChanged(nameof(WorldY));
            }
        }

        private void onImagePropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName != nameof(IImageComponent.Pivot)) return;
            refresh();
        }
    }
}