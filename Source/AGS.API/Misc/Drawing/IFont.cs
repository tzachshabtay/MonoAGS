namespace AGS.API
{
    /// <summary>
    /// Represents a font which is used when drawing text.
    /// </summary>
    [HasFactory(FactoryType = nameof(IFontLoader), MethodName = nameof(IFontLoader.LoadFont))]
    public interface IFont
	{
        /// <summary>
        /// Gets the font family.
        /// </summary>
        /// <value>The font family.</value>
		string FontFamily { get; }

        /// <summary>
        /// Gets the style of the font (regular, bold, italic, underline or strikeout).
        /// </summary>
        /// <value>The style.</value>
		FontStyle Style { get; }

        /// <summary>
        /// Gets the size of the font in points (http://www.computerhope.com/jargon/f/font-size.htm).
        /// </summary>
        /// <value>The size in points.</value>
		float SizeInPoints { get; }

        /// <summary>
        /// Measures the size that the text will take when using this font.
        /// </summary>
        /// <returns>The size of the expected text.</returns>
        /// <param name="text">Text.</param>
        /// <param name="maxWidth">Max width if the text is expected to be wrapped, or int.MaxValue for unlimited width.</param>
		SizeF MeasureString(string text, int maxWidth = int.MaxValue);

        /// <summary>
        /// Returns a new font with the same properties as the current font, but with a new size.
        /// </summary>
        /// <returns>The resized font.</returns>
        /// <param name="sizeInPoints">Size in points.</param>
        IFont Resize(float sizeInPoints);
	}
}