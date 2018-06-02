using AGS.API;

namespace AGS.Engine
{
    public class AGSScrollingComponent : AGSComponent, IScrollingComponent
    {
        private ICropChildrenComponent _crop;
        private IBoundingBoxWithChildrenComponent _boundingBoxWithChildren;
        private IBoundingBoxComponent _boundingBox;
        private ISlider _verticalScrollBar, _horizontalScrollBar;
        private IStackLayoutComponent _layout;
        private readonly IGameState _state;
        private IInObjectTreeComponent _tree;
        private IEntity _entity;
        private bool _inEvent;

        public AGSScrollingComponent(IGameState state)
        {
            _state = state;
        }

        public ISlider VerticalScrollBar 
        {
            get => _verticalScrollBar;
            set 
            {
                _verticalScrollBar?.OnValueChanged.Unsubscribe(onVerticalSliderChanged);
                if (value != null)
                {
                    value.OnValueChanged.Subscribe(onVerticalSliderChanged);
                }
                _verticalScrollBar = value;
                refreshSliderLimits();
            }
        }

        public ISlider HorizontalScrollBar
		{
            get => _horizontalScrollBar;
            set
			{
				_horizontalScrollBar?.OnValueChanged.Unsubscribe(onHorizontalSliderChanged);
                if (value != null)
                {
                    value.OnValueChanged.Subscribe(onHorizontalSliderChanged);
                }
				_horizontalScrollBar = value;
                refreshSliderLimits();
			}
		}

        public override void Init(IEntity entity)
        {
            base.Init(entity);
            _entity = entity;
            entity.Bind<ICropChildrenComponent>(c => _crop = c, _ => _crop = null);

            //Note: we're subscribing both to the bounding box changed and bounding box with children events.
            //The bounding box event is for the scrolling container, and the box with children is for the scrolling content.
            //One might wonder why do we need to subscribe to the bounding box change, won't the box with children cover that?
            //The answer is: not necessarily. It might happen that the container changes size but still completely contained in the bounding box with all the children.
            entity.Bind<IBoundingBoxWithChildrenComponent>(
                c => { _boundingBoxWithChildren = c; c.OnBoundingBoxWithChildrenChanged.Subscribe(onSizeChanged); refreshSliderLimits(); }, 
                c => { c.OnBoundingBoxWithChildrenChanged.Unsubscribe(onSizeChanged); _boundingBoxWithChildren = null; });
            entity.Bind<IBoundingBoxComponent>(
                c => { _boundingBox = c; c.OnBoundingBoxesChanged.Subscribe(onSizeChanged); refreshSliderLimits(); },
                c => { c.OnBoundingBoxesChanged.Unsubscribe(onSizeChanged); _boundingBox = null; });
            entity.Bind<IStackLayoutComponent>(c => _layout = c, _ => _layout = null);
            entity.Bind<IInObjectTreeComponent>(c => _tree = c, _ => _tree = null);
        }

        public override void Dispose()
        {
            base.Dispose();
            _entity = null;
        }

        private void onSizeChanged()
        {
            refreshSliderLimits();
        }

        private void refreshSliderLimits()
        {
            if (_inEvent) return;
            var container = _boundingBox?.GetBoundingBoxes(_state.Viewport)?.ViewportBox;
            var withChildren = _boundingBoxWithChildren?.PreCropBoundingBoxWithChildren;
            if (container == null || !container.Value.IsValid || withChildren == null || !withChildren.Value.IsValid) return;

			var verticalScrollBar = _verticalScrollBar;
            if (verticalScrollBar != null)
            {
                float maxValue = withChildren.Value.Height - container.Value.Height;
                bool visible = !MathUtils.FloatEquals(verticalScrollBar.MinValue, maxValue);
                verticalScrollBar.Visible = visible;
                verticalScrollBar.MaxValue = maxValue;
            }
            var horizontalScrollBar = _horizontalScrollBar;
            if (horizontalScrollBar != null)
            {
                float maxValue = withChildren.Value.Width - container.Value.Width;
                bool visible = !MathUtils.FloatEquals(horizontalScrollBar.MinValue, maxValue);
                horizontalScrollBar.Visible = visible;
                horizontalScrollBar.MaxValue = maxValue;
            }
        }

        private void onVerticalSliderChanged(SliderValueEventArgs args)
        {
            var crop = _crop;
            if (crop == null) return;
            var slider = _verticalScrollBar;
            if (slider == null) return;
            var layout = _layout;
            var locker = new TreeLockStep(_tree, o => o.Visible);
            layout?.StopLayout();
            locker.Lock();
            _inEvent = true;
            crop.StartPoint = new PointF(crop.StartPoint.X, -slider.Value);
            unlock(layout, locker);
        }

        private void unlock(IStackLayoutComponent layout, TreeLockStep locker)
        {
            locker.Unlock();
            layout?.StartLayout();
            _inEvent = false;
            refreshSliderLimits();
        }

        private void onHorizontalSliderChanged(SliderValueEventArgs args)
        {
			var crop = _crop;
			if (crop == null) return;
            var slider = _horizontalScrollBar;
			if (slider == null) return;
            var layout = _layout;
            var locker = new TreeLockStep(_tree, o => o.Visible);
            layout?.StopLayout();
            locker.Lock();
            _inEvent = true;
            crop.StartPoint = new PointF(slider.Value, crop.StartPoint.Y);
            unlock(layout, locker);
        }
    }
}
