namespace AGS.API
{
    /// <summary>
    /// Allow providing custom room limits. 
    /// The room limits are used to limit the camera from moving too much to the left or to the right.
    /// By default the room limits are bound to the room background size and start from (0,0), however this
    /// can be changed by setting a custom room limits provider to a room.
    /// </summary>
    /// <seealso cref="IRoom.RoomLimitsProvider"/>
    public interface IRoomLimitsProvider
    {
        /// <summary>
        /// Provides the room limits (a rectangle that defines the room area).
        /// </summary>
        /// <returns>The room limits.</returns>
        /// <param name="room">The room for which limits should be provided.</param>
        RectangleF ProvideRoomLimits(IRoom room);
    }
}
