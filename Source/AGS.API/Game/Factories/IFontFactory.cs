using System;
namespace AGS.API
{
	public interface IFontFactory : IFontLoader
    {
        /// <summary>
        /// Gets configuration for displaying text on screen.
        /// </summary>
        /// <returns>The text config.</returns>
        /// <param name="brush">Brush.</param>
        /// <param name="font">Font.</param>
        /// <param name="outlineBrush">Outline brush.</param>
        /// <param name="outlineWidth">Outline width.</param>
        /// <param name="shadowBrush">Shadow brush.</param>
        /// <param name="shadowOffsetX">Shadow offset x.</param>
        /// <param name="shadowOffsetY">Shadow offset y.</param>
        /// <param name="alignment">Alignment.</param>
        /// <param name="autoFit">Auto fit.</param>
        /// <param name="paddingLeft">Padding left.</param>
        /// <param name="paddingRight">Padding right.</param>
        /// <param name="paddingTop">Padding top.</param>
        /// <param name="paddingBottom">Padding bottom.</param>
        /// <param name="labelMinSize">Label minimum size.</param>
        ITextConfig GetTextConfig(IBrush brush = null, IFont font = null, IBrush outlineBrush = null, float outlineWidth = 0f,
            IBrush shadowBrush = null, float shadowOffsetX = 0f, float shadowOffsetY = 0f,
            Alignment alignment = Alignment.TopLeft, AutoFit autoFit = AutoFit.NoFitting,
            float paddingLeft = 2f, float paddingRight = 2f, float paddingTop = 2f, float paddingBottom = 2f, SizeF? labelMinSize = null);
    }
}