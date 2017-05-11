namespace AGS.API
{
    /// <summary>
    /// The game factory.
    /// This factory contains all of the other factories.
    /// Factories are used to offer convience methods for creating game objects.
    /// </summary>
    public interface IGameFactory
	{
        /// <summary>
        /// Gets the resources factory (for loading resources).
        /// </summary>
        /// <value>The resources.</value>
        IResourceLoader Resources { get; }

        /// <summary>
        /// Gets the graphics factory (for loading images/animations).
        /// </summary>
        /// <value>The graphics factory.</value>
		IGraphicsFactory Graphics { get; }

        /// <summary>
        /// Gets the audio factory (for loading sounds).
        /// </summary>
        /// <value>The sound factory.</value>
		IAudioFactory Sound { get; }

        /// <summary>
        /// Gets the inventory factory (for creating the inventory window and inventory items).
        /// </summary>
        /// <value>The inventory factory.</value>
		IInventoryFactory Inventory { get; }

        /// <summary>
        /// Gets the UI factory (for creating UI controls like buttons, checkboxes, etc).
        /// </summary>
        /// <value>The user interface factory.</value>
		IUIFactory UI { get; }

        /// <summary>
        /// Gets the fonts factory (for loading fonts).
        /// </summary>
        /// <value>The fonts factory.</value>
        IFontLoader Fonts { get; }

        /// <summary>
        /// Gets the object factory (for creating objects and characters).
        /// </summary>
        /// <value>The object factory.</value>
		IObjectFactory Object { get; }

        /// <summary>
        /// Gets the room factory (for creating the rooms and their edges)
        /// </summary>
        /// <value>The room factory.</value>
		IRoomFactory Room { get; }

        /// <summary>
        /// Gets the outfit factory (for creating animation outfits for characters).
        /// </summary>
        /// <value>The outfit.</value>
		IOutfitFactory Outfit { get; }

        /// <summary>
        /// Gets the dialog factory (for creating dialogs and dialog options).
        /// </summary>
        /// <value>The dialog factory.</value>
		IDialogFactory Dialog { get; }
	}
}

