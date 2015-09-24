using System;

namespace AGS.API
{
	public enum Key
	{
		
	}

	public class KeyboardEventArgs : AGSEventArgs
	{
		public KeyboardEventArgs (Key key)
		{
			Key = key;
		}

		public Key Key { get; private set; }
	}
}

