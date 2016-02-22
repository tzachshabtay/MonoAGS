using System;

namespace AGS.API
{
	public enum SpriteSheetOrder
	{
		TopLeftGoRight,
		TopLeftGoDown,
		TopRightGoLeft,
		TopRightGoDown,
		BottomLeftGoRight,
		BottomLeftGoUp,
		BottomRightGoLeft,
		BottomRightGoUp,
	}

	public interface ISpriteSheet
	{
		int CellWidth { get; }
		int CellHeight { get; }

		SpriteSheetOrder Order { get; }
		int StartFromCell { get; }
		int CellsToGrab { get; }

		string Path { get; }
	}
}

