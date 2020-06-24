﻿using System.Collections.Generic;

namespace AGS.API
{
	/// <summary>
	/// This interface allows you to create custom room transitions, to build all kinds of special effects
	/// when transitioning from room to room.
	/// </summary>
	public interface IRoomTransition
	{
		/// <summary>
		/// Will be called repeatedly before leaving the first room, up until this function will return false.
		/// </summary>
		/// <returns><c>true</c>, if we still want control, <c>false</c> otherwise.</returns>
		/// <param name="displayList">The display list of the room we are leaving. The display list contains
		/// all objects that are rendered in the scene, sorted by their order.</param>
		bool RenderBeforeLeavingRoom(List<IObject> displayList);

		/// <summary>
		/// Will be called repeatedly during the rooms transition, up until this function will return false.
		/// </summary>
		/// <returns><c>true</c>, if we still want control, <c>false</c> otherwise.</returns>
		/// <param name="from">A screenshot of the room that we are leaving.</param>
		/// <param name="to">A screenshot of the room that we entering into.</param>
		/// <example>
		/// Let's render the "old" screenshot with half opacity over the "new" screenshot:
		/// <code language="lang-csharp">
		/// QuadVectors _screenVectors = new QuadVectors(AGSGame.Game); //will create a quad covering the entire screen
		/// 
		/// public bool RenderTransition(IFrameBuffer from, IFrameBuffer to)
		/// {
		/// 	_screenVectors.Render(to.Texture);
		/// 	_screenVectors.Render(from.Texture, a: 0.5f);
		/// 
		/// 	return true; //Note that this room transition will never end if we always return true, at some point we need to return false...
		/// }
		/// </code>
		/// </example>
		bool RenderTransition(IFrameBuffer from, IFrameBuffer to);

		/// <summary>
		/// Will be called repeatedly after entering the new room, up until this function will return false.
		/// Once this function returns false, room rendering returns to AGS for a normal rendering loop (the transition is completed).
		/// </summary>
		/// <returns><c>true</c>, if we still want control, <c>false</c> otherwise.</returns>
		/// <param name="displayList">The display list of the room we are leaving. The display list contains
		/// all objects that are rendered in the scene, sorted by their order.</param>
		bool RenderAfterEnteringRoom(List<IObject> displayList);
	}
}

