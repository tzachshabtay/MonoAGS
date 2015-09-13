using System;

namespace API
{
	public interface IMask
	{
		int Width { get; }
		int Height { get; }
		IObject DebugDraw { get; }

		bool IsMasked(IPoint point);
		bool[][] AsJaggedArray();
		bool[,] To2DArray();

		void ApplyToMask(bool[][] mask);
	}
}

