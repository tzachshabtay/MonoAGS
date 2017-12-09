using System;
using AGS.API;

namespace AGS.Engine
{
	public class AGSRoomEvents : IRoomEvents
	{
        public AGSRoomEvents(IBlockingEvent onBeforeFadeIn,
			IEvent onAfterFadeIn, IBlockingEvent onBeforeFadeOut,
			IBlockingEvent onAfterFadeOut)
		{
			OnBeforeFadeIn = onBeforeFadeIn;
			OnAfterFadeIn = onAfterFadeIn;
			OnBeforeFadeOut = onBeforeFadeOut;
			OnAfterFadeOut = onAfterFadeOut;
		}

		#region IRoomEvents implementation

		public IBlockingEvent OnBeforeFadeIn { get; private set; }

		public IEvent OnAfterFadeIn { get; private set; }

		public IBlockingEvent OnBeforeFadeOut { get; private set; }

        public IBlockingEvent OnAfterFadeOut { get; private set; }

		#endregion
	}
}

