using System.Collections.Generic;
using System.ComponentModel;
using AGS.API;

namespace AGS.Engine
{
    public class AGSDepthClipping : IDepthClipping
    {
        private readonly IComparer<IObject> _comparer;

        public AGSDepthClipping()
        {
            _comparer = new RenderOrderSelector();
        }

        public IClippingPlane NearClippingPlane { get; set; }
		public IClippingPlane FarClippingPlane { get; set; }

#pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067

        public bool IsObjectClipped(IObject obj)
        {
            var nearPlane = NearClippingPlane;
			if (nearPlane != null)
			{
				if (nearPlane.PlaneObject == obj) return nearPlane.IsPlaneObjectClipped;
				if (!isLayerClippable(nearPlane, obj.RenderLayer)) return false;
				return _comparer.Compare(obj, nearPlane.PlaneObject) > 0;
			}
			var farPlane = FarClippingPlane;
			if (farPlane != null)
			{
				if (farPlane.PlaneObject == obj) return farPlane.IsPlaneObjectClipped;
				if (!isLayerClippable(farPlane, obj.RenderLayer)) return false;
				return _comparer.Compare(obj, farPlane.PlaneObject) < 0;
			}
			return false;
        }

		private bool isLayerClippable(IClippingPlane plane, IRenderLayer layer)
		{
			if (plane.LayersToClip == null) return true;
			for (int i = 0; i < plane.LayersToClip.Count; i++)
			{
				if (plane.LayersToClip[i] == layer)
				{
					return true;
				}
			}
			return false;
		}


	}
}
