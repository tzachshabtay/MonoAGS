using System;
using AGS.API;
using System.Text;

namespace AGS.Engine
{
	public class AGSMask : IMask
	{
		bool[][] _mask;

		public AGSMask(bool[][] mask, IObject debugMask)
		{
			_mask = mask;
			DebugDraw = debugMask;
			Width = mask.Length;
			Height = Width == 0 ? 0 : mask[0].Length;
			refreshMaskBounds();
		}

		#region IMask implementation

		public bool IsMasked(PointF point, ISquare projectionBox, float scaleX, float scaleY)
		{
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
			int x = (int)Math.Round(point.X, 0);
			int y = (int)Math.Round(point.Y, 0);
			if (x < 0 || x >= Width)
				return false;
			if (y < 0 || y >= Height)
				return false;
			return _mask [x][y];
		}

		public bool[][] AsJaggedArray()
		{
			return _mask;
		}

		public bool[,] To2DArray()
		{
			bool[,] result = new bool[Width, Height];
			for (int row = 0; row < Height; row++)
			{
				for (int col = 0; col < Width; col++)
				{
					result[row, col] = _mask[row][col];
				}
			}
			return result;
		}

		public void ApplyToMask(bool[][] targetMask)
		{
			for (int i = 0; i < Width; i++) 
			{
				if (i >= targetMask.Length)
					continue;
				if (targetMask [i] == null)
					targetMask [i] = new bool[Height];
				for (int j = 0; j < Height; j++) 
				{
					if (_mask [i] [j])
						targetMask [i] [j] = true;
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
					if (_mask[x][y]) sb.Append('*');
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

		private void refreshMaskBounds()
		{
			MinX = refreshBoundsUpwards(_mask, Width, Height, isThereMaskInRow);
			MaxX = refreshBoundsDownwards(_mask, Width, Height, isThereMaskInRow);
			MinY = refreshBoundsUpwards(_mask, Height, Width, isThereMaskInColumn);
			MaxY = refreshBoundsDownwards(_mask, Height, Width, isThereMaskInColumn);
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

