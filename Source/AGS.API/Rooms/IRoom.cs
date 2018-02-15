using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace AGS.API
{
    /// <summary>
    /// The rooms are where the game takes place. A room can be a room in a house, or an outdoor location, or 
    /// anything in between.
    /// The game can show only one room at a time.This would usually be the room where the player is, 
    /// though you can change to a different room than the one the player is in if you want. 
    /// Note: this is different than "Classic" AGS in which the room is explicitly tied to where the player is.
    /// </summary>
	public interface IRoom : IDisposable, INotifyPropertyChanged
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
        /// A background graphic for the room. 
        /// The background is actually just a regular object so it can be animated or use any of the properties 
        /// available on regular objects.
        /// </summary>
        /// <seealso cref="IObject"/>
        /// <value>The background.</value>
		IObject Background  { get; set; }

        /// <summary>
        /// Gets the room limits (a rectangle defining the room area)
        /// The room limits are used to limit the camera from moving too much to the left or to the right.
        /// By default the room limits are bound to the room background size and start from (0,0).
        /// This can be changed, however, by setting a custom <see cref="RoomLimitsProvider"/>. 
        /// </summary>
        /// <value>The limits.</value>
        RectangleF Limits { get; }

        /// <summary>
        /// Allows changing the default behavior which defines how the room limits are provided.
        /// The room <see cref="Limits"/>  are used to limit the camera from moving too much to the left or to the right.
        /// By default the room limits are bound to the room background size and start from (0,0).
        /// This can be changed, however, by setting this property to a different room limits provider.
        /// </summary>
        /// <example>
        /// You can use the AGSRoomLimits class, which provides some built in options for room limits:
        /// <code language="lang-csharp">
        /// room.RoomLimitsProvider = AGSRoomLimits.FromBackground; //This is the default option
        /// 
        /// room.RoomLimitsProvider = AGSRoomLimits.Infinite; //A room "without" limits. The only limits enforced are due to the coordinates supplied in floating points, so the actual limits for the room will be -3.402823e38 - 3.402823e38 (i.e 3402823 + 38 zeroes).
        /// 
        /// room.RoomLimitsProvider = AGSRoomLimits.Custom(new RectangleF(-50f, 0f, 100f, 200f)); //The room starts at (-50,0) and has a size of (100,200).
        /// </code>
        /// </example>
        /// <value>The room limits provider.</value>
        IRoomLimitsProvider RoomLimitsProvider { get; set; }

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
		IAGSBindingList<IArea> Areas { get; }

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
        /// Fins a room object with the specified id.
        /// </summary>
        /// <returns>The object if found, null otherwise.</returns>
        /// <param name="id">Identifier.</param>
        /// <typeparam name="TObject">The object's type.</typeparam>
		TObject Find<TObject>(string id) where TObject : class, IObject;
	}
}

