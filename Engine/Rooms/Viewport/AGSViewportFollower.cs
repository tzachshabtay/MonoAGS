using System;
using API;
using OpenTK;

namespace Engine
{
	public class AGSViewportFollower : IFollower
	{
		private float speedX, speedY;

		public AGSViewportFollower (float startSpeedX, float startSpeedY)
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

		public IPoint Follow (IPoint point)
		{
			if (!Enabled || Target == null) return point;
			float newX = getPos (point.X, Target.X, StartSpeedX, ref speedX);
			float newY = getPos (point.Y, Target.Y, StartSpeedY, ref speedY);
			return new AGSPoint (newX, newY);
		}

		public IObject Target { get; set; }

		private float getPos(float source, float target, float defaultSpeed, ref float speed)
		{
			float distance = Math.Abs (target - source);
			if (distance <= 0.5f) 
			{
				speed = defaultSpeed;
				return target;
			}

			float offset = speed / 100f * distance;
			if (target > source)
				source += offset;
			else
				source -= offset;
			speed *= (9f / 10f);
			return source;
		}

		#endregion
	}
}

