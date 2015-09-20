using System;
using AGS.API;
using OpenTK;
using System.Drawing;

namespace AGS.Engine
{
	public class AGSViewportFollower : IFollower
	{
		private float speedX, speedY;

		/// <summary>
		/// Initializes a new instance of the <see cref="Engine.AGSViewportFollower"/> class.
		/// </summary>
		/// <param name="startSpeedX">Start speed x (in percentage, i.e 30 is 30% percent movement toward the target).</param>
		/// <param name="startSpeedY">Start speed y (in percentage, i.e 30 is 30% percent movement toward the target).</param>
		public AGSViewportFollower (float startSpeedX = 30f, float startSpeedY = 30f)
		{
			Enabled = true;
			StartSpeedX = startSpeedX;
			StartSpeedY = startSpeedY;
			SpeedX = StartSpeedX;
			SpeedY = StartSpeedY;
		}

		public bool Enabled { get; set; }

		public float StartSpeedX { get; set; }
		public float StartSpeedY { get; set; }

		public float SpeedX { get { return speedX; } set { speedX = value; } }
		public float SpeedY { get { return speedY; } set { speedY = value; } }

		#region IFollower implementation

		public IPoint Follow (IPoint point, Size roomSize, Size virtualResoution, bool resetPosition)
		{
			IObject target = Target == null ? null : Target();
			if (!Enabled || target == null) return point;
			float targetX = getTargetPos(target.X, roomSize.Width, virtualResoution.Width);
			float targetY = getTargetPos(target.Y, roomSize.Height, virtualResoution.Height);
			if (resetPosition)
			{
				return new AGSPoint (targetX, targetY);
			}
			float newX = getPos (point.X, targetX, StartSpeedX, ref speedX);
			float newY = getPos (point.Y, targetY, StartSpeedY, ref speedY);
			return new AGSPoint (newX, newY);
		}

		public Func<IObject> Target { get; set; }

		private float getTargetPos(float target, float maxRoom, float maxResolution)
		{
			return MathUtils.Clamp(target - maxResolution / 2, 0, Math.Max(0, maxRoom - maxResolution));
		}

		private float getPos(float source, float target, float defaultSpeed, ref float speed)
		{
			float distance = Math.Abs (target - source);
			if (distance <= 0.1f) 
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

