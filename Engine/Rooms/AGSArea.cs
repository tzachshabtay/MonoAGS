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
			int x = (int)point.X;
			int y = (int)point.Y;
			if (x < 0 || x >= Mask.Length)
				return false;
			if (y < 0 || y >= Mask[0].Length)
				return false;
			return Mask [x][y];
		}

		public IPoint FindClosestPoint (IPoint point, out float distance)
		{
			int x = (int)point.X;
			int y = (int)point.Y;
			int width = Mask.Length;
			int height = Mask[0].Length;
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


		public void ApplyToMask(bool[][] targetMask)
		{
			for (int i = 0; i < Mask.Length; i++) 
			{
				if (i >= targetMask.Length)
					continue;
				if (targetMask [i] == null)
					targetMask [i] = new bool[Mask [0].Length];
				for (int j = 0; j < Mask [0].Length; j++) 
				{
					if (Mask [i] [j])
						targetMask [i] [j] = true;
				}
			}
		}

		public bool[][] Mask { get; set; }
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

			while (!Mask [x] [y]) 
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

