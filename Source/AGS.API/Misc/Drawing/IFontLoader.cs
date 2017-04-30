namespace AGS.API
{
    /// <summary>
    /// Allows loading fonts.
    /// </summary>
	public interface IFontLoader
	{
        /// <summary>
        /// Installs the specified fonts on the computer. This is required on the Mac, as there is no way to load 
        /// a font without it being installed. If the font is not installed on a Mac it will be installed 
        /// and the game will restart (otherwise the font will not render), therefore all fonts are recommended to be installed
        /// at the start of the game to make the experience seamless for the user.
        /// </summary>
        /// <param name="paths">Paths.</param>
		void InstallFonts(params string[] paths);

        /// <summary>
        /// Loads a font (which should be already installed).
        /// </summary>
        /// <returns>The font.</returns>
        /// <param name="fontFamily">Font family.</param>
        /// <param name="sizeInPoints">Size in points (http://www.computerhope.com/jargon/f/font-size.htm).</param>
        /// <param name="style">Style.</param>
		IFont LoadFont(string fontFamily, float sizeInPoints, FontStyle style = FontStyle.Regular);

        /// <summary>
        /// Loads the font from resource/file path (<see cref="IResourceLoader"/> . If the font is not installed on a Mac it will be installed
        /// and the game will be restarted, therefore it is recommended to use <see cref="InstallFonts"/> at
        /// the start of the game).
        /// </summary>
        /// <returns>The font.</returns>
        /// <param name="path">Resource/file Path.</param>
        /// <param name="sizeInPoints">Size in points (http://www.computerhope.com/jargon/f/font-size.htm).</param>
        /// <param name="style">Style.</param>
		IFont LoadFontFromPath(string path, float sizeInPoints, FontStyle style = FontStyle.Regular);
	}
}

