using System;
using AGS.API;
using System.Text;

namespace AGS.Engine
{
	public class AGSMask : IMask
	{
		private readonly bool[][] _mask;
        private bool[][] _transformedMask;
        private Vector2 _offset;

		public AGSMask(bool[][] mask, IObject debugMask)
		{
			_mask = mask;
            _transformedMask = mask;
			DebugDraw = debugMask;
			refreshMaskBounds(_transformedMask);
		}

		#region IMask implementation

        public void Transform(ITranslate translate, IRotate rotate)
        {
            if (translate == null && rotate == null)
            {
                _transformedMask = _mask;
                return;
            }
            Matrix4 transformation = createMatrix(translate, rotate, false);
            Matrix4 inverseTransform = createMatrix(translate, rotate, true);

            var corner1 = Vector3.Transform(new Vector3(0f, 0f, 0f), transformation);
            var corner2 = Vector3.Transform(new Vector3(0f, _mask[0].Length, 0f), transformation);
            var corner3 = Vector3.Transform(new Vector3(_mask.Length, 0f, 0f), transformation);
            var corner4 = Vector3.Transform(new Vector3(_mask.Length, _mask[0].Length, 0f), transformation);

            var minX = MathUtils.Min(corner1.X, corner2.X, corner3.X, corner4.X);
            var maxX = MathUtils.Max(corner1.X, corner2.X, corner3.X, corner4.X);
			var minY = MathUtils.Min(corner1.Y, corner2.Y, corner3.Y, corner4.Y);
			var maxY = MathUtils.Max(corner1.Y, corner2.Y, corner3.Y, corner4.Y);

            int width = (int)(maxX - minX) + 1;
            int height = (int)(maxY - minY) + 1;
            var offsetX = minX;
            var offsetY = minY;
            _offset = new Vector2(offsetX, offsetY);
            var transformedMask = new bool[width][];
            for (int col = 0; col < width; col++)
            {
                bool[] column = new bool[height];
                transformedMask[col] = column;
                for (int row = 0; row < height; row++)
                {
                    Vector2 vec = Vector3.Transform(new Vector3(col + offsetX, row + offsetY, 0f), inverseTransform).Xy;
                    int x = (int)Math.Round(vec.X, 0);
                    int y = (int)Math.Round(vec.Y, 0);
                    if (x < 0 || x >= _mask.Length || y < 0 || y >= _mask[0].Length)
                    {
                        column[row] = false;
                    }
                    else column[row] = _mask[x][y];
                }
            }
            refreshMaskBounds(transformedMask);
            _transformedMask = transformedMask;
        }

        public bool IsMasked(PointF point, AGSBoundingBox projectionBox, float scaleX, float scaleY)
		{
            point = new PointF(point.X - _offset.X, point.Y - _offset.Y);
			float leftX = scaleX >= 0f ? projectionBox.BottomLeft.X : projectionBox.BottomRight.X;
			float rightX = scaleX >= 0f ? projectionBox.BottomRight.X : projectionBox.BottomLeft.X;
			float bottomY = scaleY >= 0f ? projectionBox.BottomLeft.Y : projectionBox.TopLeft.Y;
			float topY = scaleY >= 0f ? projectionBox.TopLeft.Y : projectionBox.BottomLeft.Y;
			float x = MathUtils.Lerp(leftX, 0, rightX, Width - 1, point.X);
			float y = MathUtils.Lerp(bottomY, 0, topY, Height - 1, point.Y);
			return IsMasked(new PointF (x, y));
		}

		public bool IsMasked(PointF point)
		{
            point = new PointF(point.X - _offset.X, point.Y - _offset.Y);
            return isMasked(point);
		}

		public bool[][] AsJaggedArray()
		{
			return _transformedMask;
		}

		public bool[,] To2DArray()
		{
			bool[,] result = new bool[Width, Height];
			for (int row = 0; row < Height; row++)
			{
				for (int col = 0; col < Width; col++)
				{
					result[row, col] = _transformedMask[row][col];
				}
			}
			return result;
		}

        public void ApplyToMask(bool[][] targetMask, Point globalOffset)
		{
            int offsetX = (int)_offset.X - globalOffset.X;
            int offsetY = (int)_offset.Y - globalOffset.Y;
			for (int i = 0; i < Width; i++) 
			{
                int targetX = i + offsetX;
                if (targetX < 0 || targetX >= targetMask.Length)
					continue;
                int height = targetMask[targetX].Length;
				for (int j = 0; j < Height; j++) 
				{
                    int targetY = j + offsetY;
                    if (targetY < 0 || targetY >= height) continue;
					if (_transformedMask[i][j])
                        targetMask[targetX][targetY] = true;
				}
			}
		}

		public string DebugString()
		{
			StringBuilder sb = new StringBuilder ();
			for (int y = Height - 1; y >= 0; y--)
			{
				for (int x = 0; x < Width; x++)
				{
					if (_transformedMask[x][y]) sb.Append('*');
					else sb.Append(' ');
				}
				sb.AppendLine();
			}
			return sb.ToString();
		}

		public int Width { get; private set; }

		public int Height { get; private set; }

		public IObject DebugDraw { get; private set; }

		public float MinX { get; private set; }
		public float MaxX { get; private set; }
		public float MinY { get; private set; }
		public float MaxY { get; private set; }

		#endregion

		private Matrix4 createMatrix(ITranslate translate, IRotate rotate, bool inverse)
		{
			if (translate == null && rotate == null) return Matrix4.Identity;
			float factor = inverse ? -1f : 1f;

            var radians = rotate == null ? 0f : MathUtils.DegreesToRadians(rotate.Angle);
			Matrix4 rotation;
			Matrix4.CreateRotationZ(radians * factor, out rotation);
			Matrix4 translation = Matrix4.CreateTranslation(new Vector3(translate == null ? 0f : translate.X * factor,
																	  translate == null ? 0f : translate.Y * factor, 0f));
			Matrix4 transformation = inverse ? translation * rotation : rotation * translation;
			return transformation;
		}

		private bool isMasked(PointF point)
		{
			int x = (int)Math.Round(point.X, 0);
			int y = (int)Math.Round(point.Y, 0);
			if (x < 0 || x >= Width)
				return false;
			if (y < 0 || y >= Height)
				return false;
			return _transformedMask[x][y];
		}

		private void refreshMaskBounds(bool[][] mask)
		{
            Width = mask.Length;
			Height = Width == 0 ? 0 : mask[0].Length;
            MinX = refreshBoundsUpwards(mask, Width, Height, isThereMaskInRow) + (int)_offset.X;
            MaxX = refreshBoundsDownwards(mask, Width, Height, isThereMaskInRow) + (int)_offset.X;
            MinY = refreshBoundsUpwards(mask, Height, Width, isThereMaskInColumn) + (int)_offset.Y;
            MaxY = refreshBoundsDownwards(mask, Height, Width, isThereMaskInColumn) + (int)_offset.Y;
		}

		private int refreshBoundsUpwards(bool[][] mask, int length, int crossingLength, Func<bool[][], int, int, bool> isMasked)
		{
			for (int index = 0; index < length; index++)
			{
				if (isMasked(mask, crossingLength, index)) return index;
			}
			return 0;
		}

		private int refreshBoundsDownwards(bool[][] mask, int length, int crossingLength, Func<bool[][], int, int, bool> isMasked)
		{
			for (int index = length - 1; index >= 0; index--)
			{
				if (isMasked(mask, crossingLength, index)) return index;
			}
			return length - 1;
		}

		private bool isThereMaskInRow(bool[][] mask, int width, int row)
		{
			for (int col = 0; col < width; col++)
			{
				if (mask[row][col]) return true;
			}
			return false;
		}

		private bool isThereMaskInColumn(bool[][] mask, int height, int col)
		{
			for (int row = 0; row < height; row++)
			{
				if (mask[row][col]) return true;
			}
			return false;
		}
	}
}

