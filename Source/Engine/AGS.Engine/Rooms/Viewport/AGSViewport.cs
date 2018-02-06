using System.ComponentModel;
using AGS.API;

namespace AGS.Engine
{
	public class AGSViewport : IViewport
	{
        public AGSViewport(IDisplayListSettings displayListSettings, ICamera camera)
		{
			ScaleX = 1f;
			ScaleY = 1f;
            Camera = camera;
            ProjectionBox = new RectangleF(0f, 0f, 1f, 1f);
            DisplayListSettings = displayListSettings;
            Interactive = true;
		}

		#region IViewport implementation

#pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067

        public float X { get; set; }

        public float Y { get; set; }

        public float ScaleX { get; set; }

        public float ScaleY { get; set; }

        public float Angle { get; set; }

        public bool Interactive { get; set; }

        public RectangleF ProjectionBox { get; set; }

		public ICamera Camera { get; set; }

        public IObject Parent { get; set; }

        public IRoomProvider RoomProvider { get; set; }
        public IDisplayListSettings DisplayListSettings { get; set; }

        public bool IsObjectVisible(IObject obj)
        {
            return obj.Visible && !DisplayListSettings.RestrictionList.IsRestricted(obj.ID)
                      && !DisplayListSettings.DepthClipping.IsObjectClipped(obj);
        }

        #endregion
	}
}

