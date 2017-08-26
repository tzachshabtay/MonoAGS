namespace AGS.API
{
    /// <summary>
    /// Provides a room.
    /// </summary>
    public interface IRoomProvider
    {
		/// <summary>
		/// The current room.
		/// </summary>
		/// <value>The room.</value>
		IRoom Room { get; }
    }
}
