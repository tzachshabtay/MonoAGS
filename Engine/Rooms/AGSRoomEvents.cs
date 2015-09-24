using System;
using AGS.API;

namespace AGS.Engine
{
	public class AGSRoomEvents : IRoomEvents
	{
		public AGSRoomEvents(IEvent<AGSEventArgs> onBeforeFadeIn,
			IEvent<AGSEventArgs> onAfterFadeIn, IEvent<AGSEventArgs> onBeforeFadeOut,
			IEvent<AGSEventArgs> onAfterFadeOut)
		{
			OnBeforeFadeIn = onBeforeFadeIn;
			OnAfterFadeIn = onAfterFadeIn;
			OnBeforeFadeOut = onBeforeFadeOut;
			OnAfterFadeOut = onAfterFadeOut;
		}

		#region IRoomEvents implementation

		public IEvent<AGSEventArgs> OnBeforeFadeIn { get; private set; }

		public IEvent<AGSEventArgs> OnAfterFadeIn { get; private set; }

		public IEvent<AGSEventArgs> OnBeforeFadeOut { get; private set; }

		public IEvent<AGSEventArgs> OnAfterFadeOut { get; private set; }

		#endregion
	}
}

