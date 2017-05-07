namespace AGS.API
{
    /// <summary>
    /// Determines the order in which a sprite sheet is read.
    /// </summary>
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

    /// <summary>
    /// Represents a sprite sheet: An image that contains multiple sprites in it, with some
    /// rules on how to read the sprites to perform an animation (usually).
    /// </summary>
	public interface ISpriteSheet
	{
        /// <summary>
        /// Gets the width of the cell (in pixels).
        /// </summary>
        /// <value>
        /// The width of the cell (in pixels).
        /// </value>
        int CellWidth { get; }

        /// <summary>
        /// Gets the height of the cell (in pixels).
        /// </summary>
        /// <value>
        /// The height of the cell (in pixels).
        /// </value>
        int CellHeight { get; }

        /// <summary>
        /// Tells the engine how to read the sprite sheet (in what order).
        /// </summary>
        /// <value>
        /// The reading order.
        /// </value>
        SpriteSheetOrder Order { get; }

        /// <summary>
        /// Which cell should we start from, when reading the sprite sheet?
        /// </summary>
        /// <value>
        /// The 'start from' cell.
        /// </value>
        int StartFromCell { get; }

        /// <summary>
        /// Gets the number of cells to grab when reading the sprite sheet.
        /// </summary>
        /// <value>
        /// The number of cells to grab.
        /// </value>
        int CellsToGrab { get; }

        /// <summary>
        /// Gets the sprite sheet path (either the resource path, or the machine path).
        /// </summary>
        /// <value>
        /// The path.
        /// </value>
        string Path { get; }
	}
}

