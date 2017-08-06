using System;
using AGS.API;

namespace AGS.Engine
{
    public class AGSScrollingComponent : AGSComponent, IScrollingComponent
    {
        private ICropChildrenComponent _crop;
        private ISizeWithChildrenComponent _sizeWithChildren;
        private IScaleComponent _size;
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
            entity.Bind<ISizeWithChildrenComponent>(
                c => { _sizeWithChildren = c; c.OnSizeWithChildrenChanged.Subscribe(onSizeChanged); }, 
                c => { c.OnSizeWithChildrenChanged.Unsubscribe(onSizeChanged); _sizeWithChildren = null; });
            entity.Bind<IScaleComponent>(
                c => { _size = c; c.OnScaleChanged.Subscribe(onSizeChanged); },
                c => { c.OnScaleChanged.Unsubscribe(onSizeChanged); _size = null; });
        }

        private void onSizeChanged(object args)
        {
            refreshSliderLimits();
        }

        private void refreshSliderLimits()
        {
            var sizeWithChildren = _sizeWithChildren;
            var size = _size;
            if (sizeWithChildren == null || size == null) return;
            var verticalScrollBar = _verticalScrollBar;
            if (verticalScrollBar != null)
            {
                verticalScrollBar.MaxValue = sizeWithChildren.SizeWithChildren.Height - size.Height;
                verticalScrollBar.Visible = (Math.Abs(verticalScrollBar.MinValue - verticalScrollBar.MaxValue) > 0.0000001f);
            }
            var horizontalScrollBar = _horizontalScrollBar;
            if (horizontalScrollBar != null)
            {
                horizontalScrollBar.MaxValue = sizeWithChildren.SizeWithChildren.Width - size.Width;
                horizontalScrollBar.Visible = (Math.Abs(horizontalScrollBar.MinValue - horizontalScrollBar.MaxValue) > 0.0000001f);
            }
        }

        private void onVerticalSliderChanged(SliderValueEventArgs args)
        {
            var crop = _crop;
            if (crop == null) return;
            var slider = _verticalScrollBar;
            if (slider == null) return;
            crop.StartPoint = new PointF(crop.StartPoint.X, slider.Value);
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
