﻿using System;
using AGS.API;
using System.Collections.Generic;

namespace AGS.Engine
{
    public class AGSAreaComponent : AGSComponent, IAreaComponent
	{
		private static List<Tuple<int,int>> _searchVectors;
        private ITranslateComponent _translate;

		static AGSAreaComponent()
		{
			_searchVectors = new List<Tuple<int,int>>
			{
				new Tuple<int, int>(0, -1),
				new Tuple<int, int>(0, 1),
				new Tuple<int, int>(1, 0),
				new Tuple<int, int>(-1, 0),
				new Tuple<int, int>(-1, -1),
				new Tuple<int, int>(1, 1),
			};
		}

		public AGSAreaComponent ()
		{
			Enabled = true;
		}

        public override void Init(IEntity entity)
        {
            base.Init(entity);
            entity.Bind<ITranslateComponent>(
                c => { _translate = c; c.OnLocationChanged.Subscribe(onLocationChanged); },
                c => { _translate = null; c.OnLocationChanged.Unsubscribe(onLocationChanged); });
        }

		#region IArea implementation

		public bool IsInArea (PointF point)
		{
            var translate = _translate;
            if (translate != null) point = new PointF(point.X - translate.X, point.Y - translate.Y);
			return Mask.IsMasked(point);
		}

		public bool IsInArea(PointF point, AGSBoundingBox projectionBox, float scaleX, float scaleY)
		{
			var translate = _translate;
			if (translate != null) point = new PointF(point.X - translate.X, point.Y - translate.Y);
			return Mask.IsMasked(point, projectionBox, scaleX, scaleY);
		}

		public PointF? FindClosestPoint (PointF point, out float distance)
		{
			var translate = _translate;
			if (translate != null) point = new PointF(point.X - translate.X, point.Y - translate.Y);
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
			PointF? result = findClosestPoint(x, y, width, height, out insideDistance);
			distance += insideDistance;
			return result;
		}

		public IMask Mask { get; set; }
		public bool Enabled { get; set; }

		#endregion

        private void onLocationChanged()
        {
            var translate = _translate;
            var obj = Mask.DebugDraw;
            if (translate == null || obj == null) return;
            obj.Location = translate.Location;
        }

		private PointF? findClosestPoint(int x, int y, int width, int height, out float distance)
		{
			//todo: This will not always give the real closest position.
			//It's "good enough" most of the time, but can be improved (it only searches using straight lines currently).
			distance = float.MaxValue;
			PointF? closestPoint = null;
			foreach (var vector in _searchVectors) 
			{
				float tmpDistance;
				PointF? point = findClosestPoint (x, y, width, height, vector.Item1, vector.Item2, out tmpDistance);
				if (tmpDistance < distance) 
				{
					closestPoint = point;
					distance = tmpDistance;
				}
			}
			return closestPoint;
		}
			
		private PointF? findClosestPoint(int x, int y, int width, int height, int stepX, int stepY,
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
			return new PointF (x, y);
		}
	}
}

