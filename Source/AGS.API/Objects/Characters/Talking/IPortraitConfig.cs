namespace AGS.API
{
    /// <summary>
    /// Configures how the portraits will be positioned.
    /// </summary>
    public enum PortraitPositioning
    {
        /// <summary>
        /// If the speaker is in the left side of the screen the portrait will be positioned on the left,
        /// otherwise on the right.
        /// Note that the portrait's anchor will be automacially set to left in order for the portrait
        /// to be properly positioned.
        /// </summary>
        SpeakerPosition,
        /// <summary>
        /// The portraits will alternate in positions so if two characters are having a dialog,
        /// each of them will have the portrait on a sepearate side.
        /// Note that the portrait's anchor will be automacially set to left in order for the portrait
        /// to be properly positioned.
        /// </summary>
        Alternating,
        /// <summary>
        /// Portrait positioning will not be altered by the engine, it is up to the game developer to choose
        /// how to position the portrait before speaking.
        /// </summary>
        Custom,
    }

    /// <summary>
    /// Configuration for how the speech portrait is rendered.
    /// </summary>
    public interface IPortraitConfig
    {
        /// <summary>
        /// Gets or sets the portrait.
        /// </summary>
        /// <value>The portrait.</value>
        IObject Portrait { get; set; }

        /// <summary>
        /// Gets or sets how the portrait will be positioned.
        /// </summary>
        /// <value>The positioning.</value>
        PortraitPositioning Positioning { get; set; }

        /// <summary>
        /// Gets or sets the portrait offset from the screen borders.
        /// </summary>
        /// <value>The portrait offset.</value>
        PointF PortraitOffset { get; set; }

        /// <summary>
        /// Gets or sets the text offset from the portrait (on the x side) and from
        /// the screen top border (on the y side).
        /// </summary>
        /// <value>The text offset.</value>
        PointF TextOffset { get; set; }
    }
}

