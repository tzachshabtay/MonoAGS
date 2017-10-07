using AGS.API;

namespace AGS.Engine
{
    [PropertyFolder]
	public class AGSSpriteSheet : ISpriteSheet
	{
		public AGSSpriteSheet (string path, int cellWidth, int cellHeight, int startFromCell = 0, int cellsToGrab = -1, SpriteSheetOrder order = SpriteSheetOrder.TopLeftGoRight)
		{
			CellWidth = cellWidth;
			CellHeight = cellHeight;
			Order = order;
			StartFromCell = startFromCell;
			CellsToGrab = cellsToGrab;
			Path = path;
		}

		#region ISpriteSheet implementation

		public int CellWidth { get; private set; }

		public int CellHeight { get; private set; }

		public SpriteSheetOrder Order { get; private set; }

		public int StartFromCell { get; private set; }

		public int CellsToGrab { get; private set; }

		public string Path { get; private set; }

		#endregion
	}
}

