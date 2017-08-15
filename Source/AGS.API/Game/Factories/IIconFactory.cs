namespace AGS.API
{
    public enum ArrowDirection { Up, Down, Right, Left }        
        
    /// <summary>
    /// A factory for creating icons.
    /// </summary>
    public interface IIconFactory
    {
        /// <summary>
        /// Creates a file icon.
        /// </summary>
        /// <returns>The file icon.</returns>
        /// <param name="isSelected">Is the icon selected.</param>
        /// <param name="color">Color.</param>
        /// <param name="foldColor">Fold color.</param>
        /// <param name="selectedColor">Seleced color.</param>
        /// <param name="selectedFoldColor">Selected fold color.</param>
        ISelectableIcon GetFileIcon(bool isSelected = false, Color? color = null, Color? foldColor = null, 
                                    Color? selectedColor = null, Color? selectedFoldColor = null);

        /// <summary>
        /// Creates a folder icon.
        /// </summary>
        /// <returns>The folder icon.</returns>
        /// <param name="isSelected">Is the icon selected.</param>
        /// <param name="color">Color.</param>
        /// <param name="foldColor">Fold color.</param>
        /// <param name="selectedColor">Seleced color.</param>
        /// <param name="selectedFoldColor">Selected fold color.</param>
        ISelectableIcon GetFolderIcon(bool isSelected = false, Color? color = null, Color? foldColor = null,
                                      Color? selectedColor = null, Color? selectedFoldColor = null);

        /// <summary>
        /// Creates an arrow icon.
        /// </summary>
        /// <returns>The arrow icon.</returns>
        /// <param name="direction">Direction.</param>
        /// <param name="color">Color.</param>
        IBorderStyle GetArrowIcon(ArrowDirection direction, Color? color = null);

        /// <summary>
        /// Create an "X" icon (for close windows buttons, for example).
        /// </summary>
        /// <returns>The icon.</returns>
        /// <param name="lineWidth">Line width.</param>
        /// <param name="padding">Padding.</param>
        /// <param name="color">Color (leave empty for red).</param>
        IBorderStyle GetXIcon(float lineWidth = 3f, float padding = 3f, Color? color = null);
    }
}
