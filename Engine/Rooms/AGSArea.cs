using System;
using API;
using System.Collections.Generic;

namespace Engine
{
	public class AGSArea : IArea
	{
		private List<Tuple<int,int>> searchVectors;

		public AGSArea ()
		{
			Enabled = true;
			searchVectors = new List<Tuple<int,int>>
			{
				new Tuple<int, int>(0, -1),
				new Tuple<int, int>(0, 1),
				new Tuple<int, int>(1, 0),
				new Tuple<int, int>(-1, 0),
				new Tuple<int, int>(-1, -1),
				new Tuple<int, int>(1, 1),
			};
		}

		#region IArea implementation

		public bool IsInArea (IPoint point)
		{
			return Mask.IsMasked(point);
		}

		public IPoint FindClosestPoint (IPoint point, out float distance)
		{
			int x = (int)point.X;
			int y = (int)point.Y;
			int width = Mask.Width;
			int height = Mask.Height;
			distance = 0f;
			if (x < 0) 
			{
				distance -= x;
				x = 0;
			}
			if (x >= width) 
			{
				distance += (width - x);
				x = width - 1;
			}
			if (y < 0) 
			{
				distance -= y;
				y = 0;
			}

			if (y >= height) 
			{
				distance += (height - y);
				y = height - 1;
			}
			float insideDistance;
			IPoint result = findClosestPoint(x, y, width, height, out insideDistance);
			distance += insideDistance;
			return result;
		}

		public IMask Mask { get; set; }
		public bool Enabled { get; set; }

		#endregion

		private IPoint findClosestPoint(int x, int y, int width, int height, out float distance)
		{
			//todo: This will not always give the real closest position.
			//It's "good enough" most of the time, but can be improved (it only searches using straight lines currently).
			distance = float.MaxValue;
			IPoint closestPoint = null;
			foreach (var vector in searchVectors) 
			{
				float tmpDistance;
				IPoint point = findClosestPoint (x, y, width, height, vector.Item1, vector.Item2, out tmpDistance);
				if (tmpDistance < distance) 
				{
					closestPoint = point;
					distance = tmpDistance;
				}
			}
			return closestPoint;
		}
			
		private IPoint findClosestPoint(int x, int y, int width, int height, int stepX, int stepY,
			out float distance)
		{
			distance = 0f;
			bool[][] mask = Mask.AsJaggedArray();

			while (!mask [x] [y]) 
			{
				x += stepX;
				y += stepY;
				distance++;
				if (x < 0 || x >= width || y < 0 || y >= height) 
				{
					distance = float.MaxValue;
					return null;
				}
			}
			return new AGSPoint (x, y);
		}
	}
}

