using System;
using AGS.API;

namespace AGS.Engine
{
	public class AGSRoomEvents : IRoomEvents
	{
		public AGSRoomEvents(IEvent onBeforeFadeIn,
			IEvent onAfterFadeIn, IEvent onBeforeFadeOut,
			IEvent onAfterFadeOut)
		{
			OnBeforeFadeIn = onBeforeFadeIn;
			OnAfterFadeIn = onAfterFadeIn;
			OnBeforeFadeOut = onBeforeFadeOut;
			OnAfterFadeOut = onAfterFadeOut;
		}

		#region IRoomEvents implementation

		public IEvent OnBeforeFadeIn { get; private set; }

		public IEvent OnAfterFadeIn { get; private set; }

		public IEvent OnBeforeFadeOut { get; private set; }

		public IEvent OnAfterFadeOut { get; private set; }

		#endregion
	}
}

