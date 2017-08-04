using AGS.API;

namespace AGS.Engine
{
    public class AGSScrollingComponent : AGSComponent, IScrollingComponent
    {
        private ICropChildrenComponent _crop;
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
