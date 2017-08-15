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
            entity.Bind<IBoundingBoxWithChildrenComponent>(
                c => { _boundingBoxWithChildren = c; c.OnBoundingBoxWithChildrenChanged.Subscribe(onSizeChanged); refreshSliderLimits(); }, 
                c => { c.OnBoundingBoxWithChildrenChanged.Unsubscribe(onSizeChanged); _boundingBoxWithChildren = null; });
            entity.Bind<IBoundingBoxComponent>(
                c => { _boundingBox = c; refreshSliderLimits(); },
                _ => { _boundingBox = null; });
        }

        private void onSizeChanged()
        {
            refreshSliderLimits();
        }

        private void refreshSliderLimits()
        {
            var boundingBoxWithChildren = _boundingBoxWithChildren;
            var boundingBox = _boundingBox;
            if (boundingBoxWithChildren == null || boundingBox == null) return;
            var boundingBoxes = boundingBox.GetBoundingBoxes();
            if (boundingBoxes == null) return;
			var withChildren = boundingBoxWithChildren.PreCropBoundingBoxWithChildren;
			var container = boundingBoxes.RenderBox;

			var verticalScrollBar = _verticalScrollBar;
            if (verticalScrollBar != null)
            {
                float maxValue = withChildren.Height - container.Height;
                verticalScrollBar.MaxValue = maxValue;
                verticalScrollBar.Visible = (Math.Abs(verticalScrollBar.MinValue - maxValue) > 0.0000001f);
            }
            var horizontalScrollBar = _horizontalScrollBar;
            if (horizontalScrollBar != null)
            {
                float maxValue = withChildren.Width - container.Width;
                horizontalScrollBar.MaxValue = maxValue;
                horizontalScrollBar.Visible = (Math.Abs(horizontalScrollBar.MinValue - maxValue) > 0.0000001f);
            }
        }

        private void onVerticalSliderChanged(SliderValueEventArgs args)
        {
            var crop = _crop;
            if (crop == null) return;
            var slider = _verticalScrollBar;
            if (slider == null) return;
            crop.StartPoint = new PointF(crop.StartPoint.X, -(slider.MaxValue - slider.Value));
        }

        private void onHorizontalSliderChanged(SliderValueEventArgs args)
        {
			var crop = _crop;
			if (crop == null) return;
            var slider = _horizontalScrollBar;
			if (slider == null) return;
			crop.StartPoint = new PointF(slider.Value, crop.StartPoint.Y);
        }
    }
}
