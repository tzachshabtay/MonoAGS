using System;

namespace AGS.API
{
	public interface IMask
	{
		int Width { get; }
		int Height { get; }
		IObject DebugDraw { get; }

		float MinX { get; }
		float MaxX { get; }
		float MinY { get; }
		float MaxY { get; }

		bool IsMasked(IPoint point);
		bool[][] AsJaggedArray();
		bool[,] To2DArray();

		void ApplyToMask(bool[][] mask);
	}
}

