using System.ComponentModel;
using AGS.API;
using PropertyChanged;

namespace AGS.Engine
{
    [RequiredComponent(typeof(IBoundingBoxComponent))]
    [RequiredComponent(typeof(IImageComponent))]
    public class AGSWorldPositionComponent : AGSComponent, IWorldPositionComponent
    {
        private PointF _worldXY;
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
            get => _worldXY.X;
            //todo: set
        }

        [DoNotNotify]
        public float WorldY 
        { 
            get => _worldXY.Y;
            //todo: set
        }

        public PointF WorldXY
        {
            get => _worldXY;
        }

        private void refresh()
        {
            var boxComponent = _box;
            var image = _image;
            if (boxComponent == null || image == null) return;
            var box = _box.HitTestBoundingBox;
            var pivot = _image.Pivot;
            var oldXY = _worldXY;
            var worldX = MathUtils.Lerp(0f, box.MinX, 1f, box.MaxX, pivot.X);
            var worldY = MathUtils.Lerp(0f, box.MinY, 1f, box.MaxY, pivot.Y);
            _worldXY = new PointF(worldX, worldY);
            bool changed = false;
            if (!MathUtils.FloatEquals(oldXY.X, worldX))
            {
                OnPropertyChanged(nameof(WorldX));
                changed = true;
            }
            if (!MathUtils.FloatEquals(oldXY.Y, worldY))
            {
                OnPropertyChanged(nameof(WorldY));
                changed = true;
            }
            if (changed) OnPropertyChanged(nameof(WorldXY));
        }

        private void onImagePropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName != nameof(IImageComponent.Pivot)) return;
            refresh();
        }
    }
}