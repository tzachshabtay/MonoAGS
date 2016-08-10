using System;
using AGS.API;

namespace AGS.Engine
{
	public class RoomTransitionInstant : IRoomTransition
	{
		#region IRoomTransition implementation

		public bool RenderBeforeLeavingRoom(System.Collections.Generic.List<IObject> displayList, Action<IObject> renderObj)
		{
			return false;
		}

		public bool RenderTransition(IFrameBuffer from, IFrameBuffer to)
		{
			return false;
		}

		public bool RenderAfterEnteringRoom(System.Collections.Generic.List<IObject> displayList, Action<IObject> renderObj)
		{
			return false;
		}

		#endregion
	}
}

