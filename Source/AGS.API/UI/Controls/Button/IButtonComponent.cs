namespace AGS.API
{
    /// <summary>
    /// A button component allows having the entity behave like a button.
    /// </summary>
	[RequiredComponent(typeof(IUIEvents))]
	[RequiredComponent(typeof(IAnimationComponent))]
    [RequiredComponent(typeof(ITextComponent), false)]
    [RequiredComponent(typeof(IImageComponent), false)]
    [RequiredComponent(typeof(IBorderComponent), false)]
	public interface IButtonComponent : IComponent
	{
        /// <summary>
        /// Gets or sets the idle animation of the button (the animation when a button is not pushed or hovered).
        /// </summary>
        /// <value>The idle animation.</value>
        ButtonAnimation IdleAnimation { get; set; }

        /// <summary>
        /// Gets or sets the hover animation of the button (when the mouse hovers over the button).
        /// </summary>
        /// <value>The hover animation.</value>
        ButtonAnimation HoverAnimation { get; set; }

        /// <summary>
        /// Gets or sets the pushed animation of the button (when the user clicks the button).
        /// </summary>
        /// <value>The pushed animation.</value>
        ButtonAnimation PushedAnimation { get; set; }
	}
}

