using System;
using API;

namespace Engine
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
		}

		#region IMask implementation

		public bool IsMasked(IPoint point)
		{
			int x = (int)point.X;
			int y = (int)point.Y;
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

		public int Width { get; private set; }

		public int Height { get; private set; }

		public IObject DebugDraw { get; private set; }

		#endregion
	}
}

