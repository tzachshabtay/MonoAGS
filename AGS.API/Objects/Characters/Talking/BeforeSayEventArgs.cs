using System;

namespace AGS.API
{
	public class BeforeSayEventArgs : AGSEventArgs
	{
		public BeforeSayEventArgs(ILabel label, Action skip)
		{
			Label = label;
			Skip = skip;
		}

		public ILabel Label { get; set; }
		public Action Skip { get; private set; }
	}
}

