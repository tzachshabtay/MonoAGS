using System;

namespace AGS.API
{
	public enum ListChangeType
	{
		Add,
		Remove,
	}

	public class AGSListChangedEventArgs<TItem> : AGSEventArgs
	{
		public AGSListChangedEventArgs(ListChangeType changeType, TItem item, int index)
		{
			ChangeType = changeType;
			Item = item;
			Index = index;
		}

		public ListChangeType ChangeType { get; private set; }
		public TItem Item { get; private set; }
		public int Index { get; private set; }
	}
}

