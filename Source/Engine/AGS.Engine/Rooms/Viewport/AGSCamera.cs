using System;
using AGS.API;
using OpenTK;


namespace AGS.Engine
{
	public class AGSCamera : ICamera
	{
		private float _speedX, _speedY, _speedScaleX, _speedScaleY;

		/// <summary>
		/// Initializes a new instance of the <see cref="Engine.AGSViewportFollower"/> class.
		/// </summary>
		/// <param name="startSpeedX">Start speed x (in percentage, i.e 30 is 30% percent movement toward the target).</param>
		/// <param name="startSpeedY">Start speed y (in percentage, i.e 30 is 30% percent movement toward the target).</param>
		/// <param name="startSpeedScale">Start speed scale (in percentage, i.e 30 is 30% percent zoom toward the target).</param>
		public AGSCamera (float startSpeedX = 30f, float startSpeedY = 30f, float startSpeedScale = 8f)
		{
			Enabled = true;
			StartSpeedX = startSpeedX;
			StartSpeedY = startSpeedY;
			StartSpeedScale = startSpeedScale;
			SpeedX = StartSpeedX;
			SpeedY = StartSpeedY;
			SpeedScaleX = StartSpeedScale;
			SpeedScaleY = StartSpeedScale;
		}

		public bool Enabled { get; set; }

		public float StartSpeedX { get; set; }
		public float StartSpeedY { get; set; }
		public float StartSpeedScale { get; set; }

		public float SpeedX { get { return _speedX; } set { _speedX = value; } }
		public float SpeedY { get { return _speedY; } set { _speedY = value; } }
		public float SpeedScaleX { get { return _speedScaleX; } set { _speedScaleX = value; } }
		public float SpeedScaleY { get { return _speedScaleY; } set { _speedScaleY = value; } }

		#region ICamera implementation

		public void Tick (IViewport viewport, AGS.API.Size roomSize, AGS.API.Size virtualResoution, bool resetPosition)
		{
			IObject target = Target == null ? null : Target();
			if (!Enabled || target == null) return;

			setScale(target, viewport, resetPosition);

			//todo: Allow control over which point in the target to follow
			float targetX = target.X;//target.CenterPoint == null ? target.X : target.CenterPoint.X;
			float targetY = target.Y;//target.CenterPoint == null ? target.Y : target.CenterPoint.Y;
			float maxResolutionX = virtualResoution.Width / viewport.ScaleX;
			float maxResolutionY = virtualResoution.Height / viewport.ScaleY;
			targetX = getTargetPos(targetX, roomSize.Width, maxResolutionX);
			targetY = getTargetPos(targetY, roomSize.Height, maxResolutionY);
			if (resetPosition)
			{
				viewport.X = targetX;
				viewport.Y = targetY;
				return;
			}
			float newX = getPos (viewport.X, targetX, StartSpeedX, 0.1f, ref _speedX);
			float newY = getPos (viewport.Y, targetY, StartSpeedY, 0.1f, ref _speedY);
			viewport.X = clamp(newX, roomSize.Width, maxResolutionX);
			viewport.Y = clamp(newY, roomSize.Height, maxResolutionY);
		}

		public Func<IObject> Target { get; set; }

		private void setScale(IObject target, IViewport viewport, bool resetPosition)
		{
			float scale = getTargetZoom(target, viewport);
			if (resetPosition)
			{
				viewport.ScaleX = scale;
				viewport.ScaleY = scale;
				return;
			}
			float newScaleX = getPos(viewport.ScaleX, scale, StartSpeedScale, 0.001f, ref _speedScaleX);
			float newScaleY = getPos(viewport.ScaleY, scale, StartSpeedScale, 0.001f, ref _speedScaleY);
			viewport.ScaleX = newScaleX;
			viewport.ScaleY = newScaleY;
		}

		private float getTargetZoom(IObject target, IViewport viewport)
		{
			if (target.Room == null || target.Room.Areas == null) return viewport.ScaleX;

            foreach (var area in target.Room.GetMatchingAreas(target.Location.XY))
			{
                var zoomArea = area.GetComponent<IZoomArea>();
                if (zoomArea == null || !zoomArea.ZoomCamera) continue;
                float scale = zoomArea.GetZoom(target.Y);
				return scale;
			}

			return 1;
		}

		private float getTargetPos(float target, float maxRoom, float maxResolution)
		{
			float value = target - maxResolution / 2;
			return clamp(value, maxRoom, maxResolution);
		}

		private float clamp(float target, float maxRoom, float maxResolution)
		{
			float max = Math.Max(0, maxRoom - maxResolution);
			return MathUtils.Clamp(target, 0, max);
		}

		private float getPos(float source, float target, float defaultSpeed, float minDistance, ref float speed)
		{
			float distance = Math.Abs (target - source);
			if (distance <= minDistance) 
			{
				speed = defaultSpeed;
				return target;
			}

			float offset = speed / 100f * distance;
			if (target > source)
				source += offset;
			else
				source -= offset;
			if (offset > 2)
				speed *= (95f / 100f);
			return source;
		}

		#endregion
	}
}

