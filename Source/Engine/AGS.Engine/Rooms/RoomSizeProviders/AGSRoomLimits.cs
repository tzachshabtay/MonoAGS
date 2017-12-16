using AGS.API;

namespace AGS.Engine
{
    /// <summary>
    /// Built-in room limits providers.
    /// </summary>
    public static class AGSRoomLimits
    {
        /// <summary>
        /// room starts from (0,0) and ends with the background image size.
        /// This is the default <see cref="IRoom.RoomLimitsProvider"/> for each room.
        /// </summary>
        public static readonly IRoomLimitsProvider FromBackground = new RoomLimitsFromBackground();

        /// <summary>
        /// Room limits will be customized based on the given rectangle.
        /// This needs to be set in <see cref="IRoom.RoomLimitsProvider"/>. 
        /// </summary>
        /// <returns>The custom.</returns>
        /// <param name="customLimits">Custom limits.</param>
        public static IRoomLimitsProvider Custom(RectangleF customLimits) => new RoomCustomLimits(customLimits);

        /// <summary>
        /// A room "without" limits. 
        /// The only limits enforced are due to the coordinates supplied in floating points, so the actual limits for the room will be -3.402823e38 - 3.402823e38 (i.e 3402823 + 38 zeroes).
        /// This needs to be set in <see cref="IRoom.RoomLimitsProvider"/>.
        /// </summary>
        public static readonly IRoomLimitsProvider Infinite = Custom(RoomCustomLimits.MaxLimits);
    }
}
