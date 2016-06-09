using System;
using AGS.API;

namespace AGS.Engine
{
	public class AGSScalingArea : IScalingArea
	{
		private readonly IArea _area;

		public AGSScalingArea(IArea area)
		{
			_area = area;
			Enabled = true;
		}

		public static AGSScalingArea Create(IArea area, float minScaling, float maxScaling, bool scaleObjects = true, bool zoomCamera = false, bool scaleVolume = true)
		{
			//cloning to be able to enable/disable the same area as scalable and (probably) walkable
			AGSArea clone = new AGSArea { Mask = area.Mask };
			return new AGSScalingArea (clone) { MinScaling = minScaling, MaxScaling = maxScaling, ScaleObjects = scaleObjects, 
				ZoomCamera = zoomCamera, ScaleVolume = scaleVolume };
		}

		public static AGSScalingArea CreateZoom(IArea area, float minZoom, float maxZoom)
		{
			return Create(area, minZoom, maxZoom, false, true, false);
		}

		#region IArea implementation

		public bool IsInArea(PointF point)
		{
			return _area.IsInArea(point);
		}

		public bool IsInArea(PointF point, ISquare projectionBox, float scaleX, float scaleY)
		{
			return _area.IsInArea(point, projectionBox, scaleX, scaleY);
		}

		public PointF? FindClosestPoint(PointF point, out float distance)
		{
			return _area.FindClosestPoint(point, out distance);
		}
        
        public float GetScaling(float value)
        {
            return MathUtils.Lerp(Mask.MaxY, MinScaling, Mask.MinY, MaxScaling, value);
        }

		public IMask Mask { get { return _area.Mask; } set { _area.Mask = value; } }

		public bool Enabled { get { return _area.Enabled; } set { _area.Enabled = value; } }

		#endregion

		#region IScalingArea implementation

		public float MinScaling { get; set; }

		public float MaxScaling { get; set; }

		public bool ScaleObjects { get; set; }

		public bool ZoomCamera { get; set; }

		public bool ScaleVolume { get; set; }

		#endregion
	}
}

