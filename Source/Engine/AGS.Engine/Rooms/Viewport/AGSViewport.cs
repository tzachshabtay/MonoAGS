using AGS.API;

namespace AGS.Engine
{
	public class AGSViewport : IViewport
	{
        private float _x, _y, _scaleX, _scaleY, _angle;
        private RectangleF _projectionBox;
        private IObject _parent;

        public AGSViewport(IDisplayListSettings displayListSettings, ICamera camera)
		{
			_scaleX = 1f;
			_scaleY = 1f;
            Camera = camera;
            _projectionBox = new RectangleF(0f, 0f, 1f, 1f);
            OnProjectionBoxChanged = new AGSEvent();
            OnPositionChanged = new AGSEvent();
            OnScaleChanged = new AGSEvent();
            OnAngleChanged = new AGSEvent();
            OnParentChanged = new AGSEvent();
            DisplayListSettings = displayListSettings;
            Interactive = true;
		}

		#region IViewport implementation

        public float X { get { return _x; } set { refreshValue(ref _x, value, OnPositionChanged);} }

        public float Y { get { return _y; } set { refreshValue(ref _y, value, OnPositionChanged);} }

        public float ScaleX { get { return _scaleX; } set { refreshValue(ref _scaleX, value, OnScaleChanged);} }

        public float ScaleY { get { return _scaleY; } set { refreshValue(ref _scaleY, value, OnScaleChanged);} }

        public float Angle { get { return _angle; } set { refreshValue(ref _angle, value, OnAngleChanged);} }

        public bool Interactive { get; set; }

        public RectangleF ProjectionBox 
        { 
            get { return _projectionBox; } 
            set
            {
                if (_projectionBox.Equals(value)) return;
                _projectionBox = value;
                OnProjectionBoxChanged.Invoke();
            }
        }

		public ICamera Camera { get; set; }

        public IObject Parent { get { return _parent; } set { if (_parent == value) return; _parent = value; OnParentChanged.Invoke();} }

        public IEvent OnPositionChanged { get; private set; }
        public IEvent OnScaleChanged { get; private set; }
        public IEvent OnAngleChanged { get; private set; }
        public IEvent OnProjectionBoxChanged { get; private set; }
        public IEvent OnParentChanged { get; private set; }
        public IRoomProvider RoomProvider { get; set; }
        public IDisplayListSettings DisplayListSettings { get; set; }

        public bool IsObjectVisible(IObject obj)
        {
            return obj.Visible && !DisplayListSettings.RestrictionList.IsRestricted(obj.ID)
                      && !DisplayListSettings.DepthClipping.IsObjectClipped(obj);
        }

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

