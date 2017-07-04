using System;
using AGS.API;

namespace AGS.Engine
{
	public class AGSRoomEvents : IRoomEvents
	{
		public AGSRoomEvents(IEvent<object> onBeforeFadeIn,
			IEvent<object> onAfterFadeIn, IEvent<object> onBeforeFadeOut,
			IEvent<object> onAfterFadeOut)
		{
			OnBeforeFadeIn = onBeforeFadeIn;
			OnAfterFadeIn = onAfterFadeIn;
			OnBeforeFadeOut = onBeforeFadeOut;
			OnAfterFadeOut = onAfterFadeOut;
		}

		#region IRoomEvents implementation

		public IEvent<object> OnBeforeFadeIn { get; private set; }

		public IEvent<object> OnAfterFadeIn { get; private set; }

		public IEvent<object> OnBeforeFadeOut { get; private set; }

		public IEvent<object> OnAfterFadeOut { get; private set; }

		#endregion
	}
}

