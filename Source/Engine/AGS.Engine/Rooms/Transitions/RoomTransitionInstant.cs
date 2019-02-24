using System.Collections.Generic;
using AGS.API;

namespace AGS.Engine
{
	public class RoomTransitionInstant : IRoomTransition
	{
		#region IRoomTransition implementation

		public bool RenderBeforeLeavingRoom(List<IObject> displayList)
		{
			return false;
		}

		public bool RenderTransition(IFrameBuffer from, IFrameBuffer to)
		{
			return false;
		}

		public bool RenderAfterEnteringRoom(List<IObject> displayList)
		{
			return false;
		}

		#endregion
	}
}

