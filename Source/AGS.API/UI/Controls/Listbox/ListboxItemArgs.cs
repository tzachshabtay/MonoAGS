namespace AGS.API
{
	/// <summary>
	/// Event arguments for when a selected item changes in a listbox/combobox.
	/// </summary>
	public class ListboxItemArgs : AGSEventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:AGS.API.ListboxItemArgs"/> class.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <param name="index">Index.</param>
        public ListboxItemArgs(IStringItem item, int index)
		{
			Item = item;
			Index = index;
		}

		/// <summary>
		/// The newly selected item.
		/// </summary>
		/// <value>The item.</value>
        public IStringItem Item { get; private set; }

		/// <summary>
		/// The selected index.
		/// </summary>
		/// <value>The index.</value>
		public int Index { get; private set; }
	}
}
