using AGS.API;

namespace AGS.Engine
{
	public class AGSViewport : IViewport
	{
        private float _x, _y, _scaleX, _scaleY, _angle;

        public AGSViewport(IDisplayListSettings displayListSettings, ICamera camera)
		{
			_scaleX = 1f;
			_scaleY = 1f;
            Camera = camera;
            ProjectionBox = new RectangleF(0f, 0f, 1f, 1f);
            OnPositionChanged = new AGSEvent();
            OnScaleChanged = new AGSEvent();
            OnAngleChanged = new AGSEvent();
            DisplayListSettings = displayListSettings;
		}

		#region IViewport implementation

        public float X { get { return _x; } set { refreshValue(ref _x, value, OnPositionChanged);} }

        public float Y { get { return _y; } set { refreshValue(ref _y, value, OnPositionChanged);} }

        public float ScaleX { get { return _scaleX; } set { refreshValue(ref _scaleX, value, OnScaleChanged);} }

        public float ScaleY { get { return _scaleY; } set { refreshValue(ref _scaleY, value, OnScaleChanged);} }

        public float Angle { get { return _angle; } set { refreshValue(ref _angle, value, OnAngleChanged);} }

        public RectangleF ProjectionBox { get; set; }

		public ICamera Camera { get; set; }

        public IEvent OnPositionChanged { get; private set; }
        public IEvent OnScaleChanged { get; private set; }
        public IEvent OnAngleChanged { get; private set; }
        public IRoomProvider RoomProvider { get; set; }
        public IDisplayListSettings DisplayListSettings { get; set; }

        #endregion

        private void refreshValue(ref float currentValue, float newValue, IEvent changeEvent)
        {
#pragma warning disable RECS0018 // Comparison of floating point numbers with equality operator
            if (currentValue == newValue) return;
#pragma warning restore RECS0018 // Comparison of floating point numbers with equality operator
            currentValue = newValue;
            changeEvent.Invoke();
        }
	}
}

