using System;
using System.Collections.Generic;

namespace AGS.API
{
    /// <summary>
    /// The rooms are where the game takes place. A room can be a room in a house, or an outdoor location, or 
    /// anything in between.
    /// The game can show only one room at a time.This would usually be the room where the player is, 
    /// though you can change to a different room than the one the player is in if you want. 
    /// Note: this is different than "Classic" AGS in which the room is explicitly tied to where the player is.
    /// </summary>
	public interface IRoom : IDisposable
	{
        /// <summary>
        /// Each rooms should have a unique id (string) which identifies it to the engine.
        /// </summary>
        /// <value>The identifier.</value>
		string ID { get; }

        /// <summary>
        /// A flag indicating whether the player is to be shown in the room, for hiding the player from a map, for example. 
        /// This is a convenience flag as this can be achieved by simply switching the room without moving the player 
        /// to another room.
        /// </summary>
        /// <value><c>true</c> if show player; otherwise, <c>false</c>.</value>
		bool ShowPlayer { get; set; }

        /// <summary>
        /// An optional music clip you can play when switching to the room.
        /// If moved from another room with a music clip, there clips will be cross-faded, 
        /// and you can configure the cross-fading duration and easing functions from 
        /// <see cref="IAudioSettings.RoomMusicCrossFading"/>.
        /// </summary>
        /// <value>The audio clip.</value>
		IAudioClip MusicOnLoad { get; set; }

        /// <summary>
        /// It might be that not all of the room is shown on the screen at once (for example, a scrolling room). 
        /// A viewport to the room instructs the engine on what parts of the room to show.
        /// </summary>
        /// <value>The viewport.</value>
		IViewport Viewport { get; }

        /// <summary>
        /// A background graphic for the room. 
        /// The background is actually just a regular object so it can be animated or use any of the properties 
        /// available on regular objects.
        /// </summary>
        /// <seealso cref="IObject"/>
        /// <value>The background.</value>
		IObject Background  { get; set; }

        /// <summary>
        /// A list of objects that are placed in the room (note that unlike "Classic" AGS, objects can be moved between rooms).
        /// </summary>
        /// <value>The objects.</value>
		IConcurrentHashSet<IObject> Objects { get; }

        /// <summary>
        /// Each room has specific events which you can subscribe to and code stuff to happen on those events.
        /// </summary>
        /// <value>The events.</value>
		IRoomEvents Events { get; }

        /// <summary>
        /// Special properties which you can attach to a room which might be useful for special coding tasks.
        /// </summary>
        /// <value>The properties.</value>
		ICustomProperties Properties { get; }

        /// <summary>
        /// A list of areas (or regions) that exist in the room (like areas in the room that the characters can walk in). 
        /// There are several types of areas and things you can do with them.
        /// </summary>
        /// <seealso cref="IArea"/>
        /// <seealso cref="IWalkableArea"/>
        /// <seealso cref="IWalkBehindArea"/>
        /// <seealso cref="IScalingArea"/>
        /// <seealso cref="IZoomArea"/>
        /// <seealso cref="IAreaRestriction"/>
        /// <value>The areas.</value>
		IList<IArea> Areas { get; }

        /// <summary>
        /// Room edges are a convenient way for scripting a room change once a player walks beyond an edge. 
        /// You can set the 'X' for the left and right edge, and the 'Y' for the top and bottom edge and subscribe to 
        /// events when the player crosses the edge to change the room.
        /// </summary>
        /// <value>The edges.</value>
		IEdges Edges { get; }

        /// <summary>
        /// Get all room areas that contain the specified point (and apply to the optional specified entity). 
        /// </summary>
        /// <returns>The matching areas.</returns>
        /// <param name="point">Point.</param>
        /// <param name="entityId">Entity identifier.</param>
        IEnumerable<IArea> GetMatchingAreas(PointF point, string entityId);

        /// <summary>
        /// Gets all the visible objects in the room, ordered from front to back.
        /// </summary>
        /// <returns>The visible objects front to back.</returns>
        /// <param name="includeUi">If set to <c>true</c> include global user interface objects (<see cref="IGameState.UI"/>) as well as room objects.</param>
		IEnumerable<IObject> GetVisibleObjectsFrontToBack(bool includeUi = true);

        /// <summary>
        /// Gets the top-most visible object at the specified point (in room coordinates).
        /// </summary>
        /// <returns>The top-most visible <see cref="T:AGS.API.IObject"/>.</returns>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="onlyEnabled">If set to <c>true</c> then ignore disabled objects.</param>
        /// <param name="includeUi">If set to <c>true</c> include global user interface objects (<see cref="IGameState.UI"/>).</param>
		IObject GetObjectAt(float x, float y, bool onlyEnabled = true, bool includeUi = true);

        /// <summary>
        /// Fins a room object with the specified id.
        /// </summary>
        /// <returns>The object if found, null otherwise.</returns>
        /// <param name="id">Identifier.</param>
        /// <typeparam name="TObject">The object's type.</typeparam>
		TObject Find<TObject>(string id) where TObject : class, IObject;
	}
}

