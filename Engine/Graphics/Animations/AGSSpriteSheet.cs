using System;
using AGS.API;

namespace AGS.Engine
{
	public class AGSSpriteSheet : ISpriteSheet
	{
		public AGSSpriteSheet (int cellWidth, int cellHeight, int startFromCell = 0, int cellsToGrab = -1, SpriteSheetOrder order = SpriteSheetOrder.TopLeftGoRight)
		{
			CellWidth = cellWidth;
			CellHeight = cellHeight;
			Order = order;
			StartFromCell = startFromCell;
			CellsToGrab = cellsToGrab;
		}

		#region ISpriteSheet implementation

		public int CellWidth { get; private set; }

		public int CellHeight { get; private set; }

		public SpriteSheetOrder Order { get; private set; }

		public int StartFromCell { get; private set; }

		public int CellsToGrab { get; private set; }

		#endregion
	}
}

