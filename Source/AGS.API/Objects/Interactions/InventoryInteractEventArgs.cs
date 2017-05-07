namespace AGS.API
{
    /// <summary>
    /// Event arguments for inventory interaction
    /// </summary>
    public class InventoryInteractEventArgs : ObjectEventArgs
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="T:AGS.API.InventoryInteractEventArgs"/> class.
        /// </summary>
        /// <param name="obj">Object.</param>
        /// <param name="item">Item.</param>
		public InventoryInteractEventArgs (IObject obj, IInventoryItem item) : base(obj)
		{
			Item = item;
		}

        /// <summary>
        /// The inventory item being interacted with.
        /// </summary>
        /// <value>The item.</value>
		public IInventoryItem Item { get; private set; }

		public override string ToString ()
		{
			return string.Format ("{0} interacted with {1}", base.ToString(), 
				Item == null ? "null" : Item.ToString());
		}
	}
}

