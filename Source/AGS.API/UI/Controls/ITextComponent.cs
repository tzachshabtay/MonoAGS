namespace AGS.API
{
    /// <summary>
    /// A text component allows displaying text on the screen.
    /// </summary>
    [RequiredComponent(typeof(IImageComponent))]
    [RequiredComponent(typeof(IDrawableInfoComponent))]
    [RequiredComponent(typeof(ICropSelfComponent), false)]
    [RequiredComponent(typeof(IModelMatrixComponent))]
    [RequiredComponent(typeof(IVisibleComponent))]
    public interface ITextComponent : IComponent
    {
        /// <summary>
        /// Gets or sets the text configuration (font, color, outline, etc).
        /// </summary>
        /// <value>The text config.</value>
        ITextConfig TextConfig { get; set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        string Text { get; set; }

        /// <summary>
        /// Gets or sets the size of the label that hosts the text (that label can have a color and a border,
        /// and is also used for text alignment calculations).
        /// </summary>
        /// <value>The size of the label render.</value>
        SizeF LabelRenderSize { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show/hide the text..
        /// </summary>
        /// <value><c>true</c> if text visible; otherwise, <c>false</c>.</value>
        bool TextVisible { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the background of the text should be rendered.
        /// </summary>
        /// <value><c>true</c> if label visible; otherwise, <c>false</c>.</value>
        bool TextBackgroundVisible { get; set; }

        /// <summary>
        /// Gets the height of the text.
        /// </summary>
        /// <value>The height of the text.</value>
        float TextHeight { get; }

        /// <summary>
        /// Gets the width of the text.
        /// </summary>
        /// <value>The width of the text.</value>
        float TextWidth { get; }

        /// <summary>
        /// Event which fires whenever the label size changes.
        /// </summary>
        /// <value>The on label size changed.</value>
        IBlockingEvent OnLabelSizeChanged { get; }

        /// <summary>
        /// The text component can supply a custom image size to override the image size used by calculations in <see cref="IModelMatrixComponent"/>.
        /// </summary>
        /// <value>The custom size.</value>
        SizeF? CustomImageSize { get; }

        /// <summary>
        /// The text component can supply a custom resolution factor (due to text scaling) to override the resolution factor used by calculations in <see cref="IModelMatrixComponent"/>.
        /// </summary>
        /// <value>The custom image resolution factor.</value>
        PointF? CustomImageResolutionFactor { get; }

        /// <summary>
        /// Allows to set a custom cropper for the text (used by <see cref="ICropChildrenComponent"/>) to crop the text of the child.
        /// </summary>
        /// <value>The custom text crop.</value>
        ICropSelfComponent CustomTextCrop { get; set; }

        /// <summary>
        /// Gets the bounding boxes which surround the text..
        /// </summary>
        /// <seealso cref="PrepareTextBoundingBoxes"/>
        /// <value>The text bounding boxes.</value>
        AGSBoundingBoxes TextBoundingBoxes { get; }

        /// <summary>
        /// The position of the caret inside the string (if the caret is displayed- i.e if it's a textbox). A value of 3, for example, means the caret will be shown after the 3rd character.
        /// The default is 0, meaning the caret will be placed at the start of the string.
        /// The caret position automatically changes based on the keyboard input.
        /// </summary>
        /// <seealso cref="RenderCaret"/>
        int CaretPosition { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:AGS.API.ITextComponent"/> should render a caret 
        /// (a vertical line, used for displaying text boxes).
        /// </summary>
        /// <seealso cref="CaretPosition"/>
        /// <value><c>true</c> if render caret; otherwise, <c>false</c>.</value>
        bool RenderCaret { get; set; }

        /// <summary>
        /// Horizontal offset in pixels for the caret.
        /// </summary>
        /// <value>The caret X offset.</value>
        int CaretXOffset { get; set; }

        /// <summary>
        /// Prepares the bounding boxes for the text. This is called repeatedly by the engine,
        /// but you can call it yourself if you need accurate measurements NOW.
        /// </summary>
        /// <seealso cref="TextBoundingBoxes"/>
        void PrepareTextBoundingBoxes();

        /// <summary>
        /// Allows locking the component from changing (to allow for changing multiple components "at once").
        /// </summary>
        /// <value>The lock step.</value>
        ILockStep TextLockStep { get; }
    }
}