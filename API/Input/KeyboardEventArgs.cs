using System;

namespace AGS.API
{
	public enum Key
	{
		
	}

	public class KeyboardEventArgs : EventArgs
	{
		public KeyboardEventArgs (Key key)
		{
			Key = key;
		}

		public Key Key { get; private set; }
	}
}

