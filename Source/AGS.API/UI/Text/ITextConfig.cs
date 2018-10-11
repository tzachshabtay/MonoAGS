using System.ComponentModel;

namespace AGS.API
{
    /// <summary>
    /// Allows for a label and its text to fit together in various way.
    /// </summary>
    public enum AutoFit
	{
        /// <summary>
        /// There will be no custom fitting: the user will select a label size, and the text size
        /// will be based on the font, it's size and the text.
        /// </summary>
		NoFitting,
        /// <summary>
        /// The label size will be ignored. The label will resize to fit the text inside it.
        /// </summary>
		LabelShouldFitText,
        /// <summary>
        /// The label width will be used as a maximum width for the text. If the text width is bigger,
        /// it will wrap to the next line. The label height will then be resized to fit the text.
        /// This is the default for speech and dialog options.
        /// </summary>
		TextShouldWrapAndLabelShouldFitHeight,
        /// <summary>
        /// The text will be resized (but only downscaled, not upscaled) to fit the label size.
        /// This can be useful when you lay out multiple buttons when you want to have them at the same size,
        /// and have various lengths of text.
        /// </summary>
		TextShouldFitLabel,
        /// <summary>
        /// The text will be cropped to fit the label size.
        /// This is the default for textboxes.
        /// </summary>
        TextShouldCrop,
	}

    /// <summary>
    /// Configuration which is used when rendering text.
    /// </summary>
    [HasFactory(FactoryType = nameof(IFontFactory), DisplayName = "Text Configuration", MethodName = nameof(IFontFactory.GetTextConfig))]
    public interface ITextConfig : INotifyPropertyChanged
	{
        /// <summary>
        /// Gets the brush for drawing the text.
        /// The brush is usually a solid color (for example, a blue brush), but 
        /// can potentially be a different brush (like a gradient brush).
        /// </summary>
        /// <value>The brush.</value>
        IBrush Brush { get; set; }

        /// <summary>
        /// Gets the font used to render the text.
        /// </summary>
        /// <value>The font.</value>
        IFont Font { get; set; }

        /// <summary>
        /// Gets the alignment of the text (the text will be aligned to its hosting label.
        /// </summary>
        /// <value>The alignment.</value>
        Alignment Alignment { get; set; }

        /// <summary>
        /// Gets an optional brush for drawing an outline for the text.
        /// An outline is the same text drawn behind the original text, only slightly bigger
        /// and usually with a different color, to provide better contrast for the rendered text.
        /// </summary>
        /// <value>The outline brush.</value>
        IBrush OutlineBrush { get; set; }

        /// <summary>
        /// Gets the width of the outline.
        /// </summary>
        /// <value>The width of the outline.</value>
        float OutlineWidth { get; set; }

        /// <summary>
        /// Gets an optional brush for drawing a shadow for the text.
        /// The shadow is the same text drawn behind the original text with an offset,
        /// giving the appearance of a shadow.
        /// </summary>
        /// <seealso cref="ShadowOffsetX"/>
        /// <seealso cref="ShadowOffsetY"/>
        /// <value>The shadow brush.</value>
        IBrush ShadowBrush { get; set; }

        /// <summary>
        /// Gets the shadow x offset. This is the number of horizontal pixels that the shadow
        /// will be drawn away from the text (assuming <see cref="ShadowBrush"/> is used). 
        /// </summary>
        /// <value>The shadow x offset.</value>
        float ShadowOffsetX { get; set; }

        /// <summary>
        /// Gets the shadow y offset. This is the number of vertical pixels that the shadow
        /// will be drawn away from the text (assuming <see cref="ShadowBrush"/> is used). 
        /// </summary>
        /// <value>The shadow y offset.</value>
        float ShadowOffsetY { get; set; }

        /// <summary>
        /// Allows for a label and its text to fit together in various way  
        /// </summary>
        AutoFit AutoFit { get; set; }

        /// <summary>
        /// Gets a left padding (in pixels) for the text from its hosting label.
        /// </summary>
        /// <value>The left padding.</value>
        float PaddingLeft { get; set; }

        /// <summary>
        /// Gets a right padding (in pixels) for the text from its hosting label.
        /// </summary>
        /// <value>The right padding.</value>
        float PaddingRight { get; set; }

        /// <summary>
        /// Gets a top padding (in pixels) for the text from its hosting label.
        /// </summary>
        /// <value>The top padding.</value>
        float PaddingTop { get; set; }

        /// <summary>
        /// Gets a bottom padding (in pixels) for the text from its hosting label.
        /// </summary>
        /// <value>The bottom padding.</value>
        float PaddingBottom { get; set; }

        /// <summary>
        /// An optional minimum size for the label containing the text.
        /// This will be enforced after <see cref="AutoFit"/> has taken place.
        /// </summary>
        /// <value>The minimum size of the label.</value>
        SizeF? LabelMinSize { get; set; }
	}
}