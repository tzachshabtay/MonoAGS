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
        private IInObjectTree _tree;
        private IEntity _entity;
        private bool _inEvent;

        public AGSScrollingComponent(IGameState state)
        {
            _state = state;
        }

        public ISlider VerticalScrollBar 
        { 
            get { return _verticalScrollBar; }
            set 
            {
                var scrollBar = _verticalScrollBar;
                if (scrollBar != null)
                {
                    scrollBar.OnValueChanged.Unsubscribe(onVerticalSliderChanged);
                }
                var crop = _crop;
                if (crop != null) crop.EntitiesToSkipCrop.Add(value.ID);
                value.OnValueChanged.Subscribe(onVerticalSliderChanged);
                _verticalScrollBar = value;
                refreshSliderLimits();
            }
        }

        public ISlider HorizontalScrollBar
		{
            get { return _horizontalScrollBar; }
			set
			{
				var scrollBar = _horizontalScrollBar;
				if (scrollBar != null)
				{
					scrollBar.OnValueChanged.Unsubscribe(onHorizontalSliderChanged);
				}
				var crop = _crop;
				if (crop != null) crop.EntitiesToSkipCrop.Add(value.ID);
				value.OnValueChanged.Subscribe(onHorizontalSliderChanged);
				_horizontalScrollBar = value;
                refreshSliderLimits();
			}
		}

        public override void Init(IEntity entity)
        {
            base.Init(entity);
            _entity = entity;
            entity.Bind<ICropChildrenComponent>(c => {
                _crop = c;
                var verticalScrollBar = VerticalScrollBar;
                if (verticalScrollBar != null)
                {
                    c.EntitiesToSkipCrop.Add(verticalScrollBar.ID);
                }
                var horizontalScrollBar = HorizontalScrollBar;
                if (horizontalScrollBar != null)
                {
                    c.EntitiesToSkipCrop.Add(horizontalScrollBar.ID);
                }
            }, _ => _crop = null);

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
            entity.Bind<IInObjectTree>(c => _tree = c, _ => _tree = null);
        }

        private void onSizeChanged()
        {
            refreshSliderLimits();
        }

        private void refreshSliderLimits()
        {
            if (_inEvent) return;
            var boundingBoxWithChildren = _boundingBoxWithChildren;
            var boundingBox = _boundingBox;
            if (boundingBoxWithChildren == null || boundingBox == null) return;
            var boundingBoxes = boundingBox.GetBoundingBoxes(_state.Viewport);
            if (boundingBoxes == null) return;
            var withChildren = boundingBoxWithChildren.PreCropBoundingBoxWithChildren;
			var container = boundingBoxes.RenderBox;

			var verticalScrollBar = _verticalScrollBar;
            if (verticalScrollBar != null)
            {
                float maxValue = withChildren.Height - container.Height;
                bool visible = !MathUtils.FloatEquals(verticalScrollBar.MinValue, maxValue);
                verticalScrollBar.Visible = visible;
                verticalScrollBar.MaxValue = maxValue;
            }
            var horizontalScrollBar = _horizontalScrollBar;
            if (horizontalScrollBar != null)
            {
                float maxValue = withChildren.Width - container.Width;
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
            if (layout != null) layout.StopLayout();
            locker.Lock();
            _inEvent = true;
            crop.StartPoint = new PointF(crop.StartPoint.X, -slider.Value);
            unlock(layout, locker);
        }

        private void unlock(IStackLayoutComponent layout, TreeLockStep locker)
        {
            locker.Unlock();
            if (layout != null) layout.StartLayout();
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
            if (layout != null) layout.StopLayout();
            locker.Lock();
            _inEvent = true;
            crop.StartPoint = new PointF(slider.Value, crop.StartPoint.Y);
            unlock(layout, locker);
        }
    }
}
