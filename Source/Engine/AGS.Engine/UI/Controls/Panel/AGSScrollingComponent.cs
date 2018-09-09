using System;
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
                _verticalScrollBar?.OnValueChanging.Unsubscribe(onVerticalSliderChanged);
                if (value != null)
                {
                    value.OnValueChanging.Subscribe(onVerticalSliderChanged);
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
                _horizontalScrollBar?.OnValueChanging.Unsubscribe(onHorizontalSliderChanged);
                if (value != null)
                {
                    value.OnValueChanging.Subscribe(onHorizontalSliderChanged);
                }
				_horizontalScrollBar = value;
                refreshSliderLimits();
			}
		}

        public override void Init()
        {
            base.Init();
            Entity.Bind<ICropChildrenComponent>(c => _crop = c, _ => _crop = null);

            //Note: we're subscribing both to the bounding box changed and bounding box with children events.
            //The bounding box event is for the scrolling container, and the box with children is for the scrolling content.
            //One might wonder why do we need to subscribe to the bounding box change, won't the box with children cover that?
            //The answer is: not necessarily. It might happen that the container changes size but still completely contained in the bounding box with all the children.
            Entity.Bind<IBoundingBoxWithChildrenComponent>(
                c => { _boundingBoxWithChildren = c; c.OnBoundingBoxWithChildrenChanged.Subscribe(onSizeChanged); refreshSliderLimits(); }, 
                c => { c.OnBoundingBoxWithChildrenChanged.Unsubscribe(onSizeChanged); _boundingBoxWithChildren = null; });
            Entity.Bind<IBoundingBoxComponent>(
                c => { _boundingBox = c; c.OnBoundingBoxesChanged.Subscribe(onSizeChanged); refreshSliderLimits(); },
                c => { c.OnBoundingBoxesChanged.Unsubscribe(onSizeChanged); _boundingBox = null; });
            Entity.Bind<IStackLayoutComponent>(c => _layout = c, _ => _layout = null);
            Entity.Bind<IInObjectTreeComponent>(c => _tree = c, _ => _tree = null);
        }

        private void onSizeChanged()
        {
            refreshSliderLimits();
        }

        private void refreshSliderLimits()
        {
            if (_inEvent) return;
            var container = _boundingBox?.GetBoundingBoxes(_state.Viewport)?.ViewportBox;

            //Uncomment for debug:
            //if (_boundingBoxWithChildren is AGSBoundingBoxWithChildrenComponent x)
            //    x.DebugPrintouts = true;

            var withChildren = _boundingBoxWithChildren?.PreCropBoundingBoxWithChildren;
            var verticalScrollBar = _verticalScrollBar;
            var horizontalScrollBar = _horizontalScrollBar;
            if (container == null || !container.Value.IsValid || withChildren == null || !withChildren.Value.IsValid)
            {
                if (verticalScrollBar != null) verticalScrollBar.Visible = false;
                if (horizontalScrollBar != null) horizontalScrollBar.Visible = false;
                return;
            }

            var layout = _layout;
			if (verticalScrollBar != null)
            {
                float childrenHeight = withChildren.Value.Height;
                if (layout != null && layout.Direction == LayoutDirection.Vertical && MathUtils.FloatEquals(layout.RelativeSpacing, 0f))
                {
                    float layoutHeight = layout.DryRun();
                    childrenHeight = Math.Max(layoutHeight, childrenHeight);
                }
                refreshSliderLimits(verticalScrollBar, childrenHeight, container.Value.Height);
            }
            if (horizontalScrollBar != null)
            {
                float childrenWidth = withChildren.Value.Width;
                if (layout != null && layout.Direction == LayoutDirection.Horizontal && MathUtils.FloatEquals(layout.RelativeSpacing, 0f))
                {
                    float layoutWidth = layout.DryRun();
                    childrenWidth = Math.Max(layoutWidth, childrenWidth);
                }
                refreshSliderLimits(horizontalScrollBar, childrenWidth, container.Value.Width);
            }
        }

        private void refreshSliderLimits(ISlider slider, float withChildrenSize, float containerSize)
        {
            float maxValue = Math.Max(0f, withChildrenSize - containerSize);
            bool visible = !MathUtils.FloatEquals(slider.MinValue, maxValue);
            slider.Visible = visible;
            bool clampValueToMax = maxValue > slider.MaxValue && 
                                   MathUtils.FloatEquals(slider.MaxValue, slider.Value) &&
                                   !MathUtils.FloatEquals(slider.MinValue, slider.MaxValue);
            slider.MaxValue = maxValue;
            if (clampValueToMax)
            {
                slider.Value = maxValue;
            }
            float handleSize = (containerSize / withChildrenSize) * (slider.IsHorizontal() ? slider.Graphics.Image.Width : slider.Graphics.Image.Height);
            slider.HandleGraphics.Image = new EmptyImage(slider.IsHorizontal() ? handleSize : slider.HandleGraphics.Image.Width,
                                                         slider.IsHorizontal() ? slider.HandleGraphics.Image.Height : handleSize);
            slider.MinHandleOffset = handleSize;
        }

        private void onVerticalSliderChanged(SliderValueEventArgs args)
        {
            onSliderChanged(_verticalScrollBar);
        }

        private void onHorizontalSliderChanged(SliderValueEventArgs args)
        {
            onSliderChanged(_horizontalScrollBar);
        }

        private void onSliderChanged(ISlider slider)
        {
            if (slider == null) return;
            var crop = _crop;
            if (crop == null) return;
            var layout = _layout;
            var locker = new TreeLockStep(_tree, o => o.Visible);
            layout?.StopLayout();
            locker.Lock();
            _inEvent = true;
            crop.StartPoint = slider == _verticalScrollBar ? 
                new PointF(crop.StartPoint.X, -slider.Value):
                new PointF(slider.Value, crop.StartPoint.Y);
            unlock(layout, locker);
        }

        private void unlock(IStackLayoutComponent layout, TreeLockStep locker)
        {
            locker.Unlock();
            layout?.StartLayout();
            _inEvent = false;
            refreshSliderLimits();
        }
    }
}
