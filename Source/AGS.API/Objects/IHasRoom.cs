using System.Threading.Tasks;

namespace AGS.API
{
    /// <summary>
    /// This component associates an entity with the game's room system.
    /// It means that the entity can be belong to a room and moved between rooms.
    /// </summary>
    public interface IHasRoom : IComponent
    {
        /// <summary>
        /// The current room that the entity resides in.
        /// If this is entity is to be rendered to the screen it will only be rendered if it's in the same room as the player (or if it's in the UI).       
        /// </summary>
		IRoom Room { get; }

        /// <summary>
        /// The previous room that the entity used to reside in until it moved to the current room.
        /// This will be null if the entity has never moved rooms.
        /// </summary>
        IRoom PreviousRoom { get; }

        /// <summary>
        /// Move the entity to a new room.
        /// If the entity that's moved to a new room is the player, the new room will be the one shown on the screen.
        /// </summary>
        /// <param name="room">The room to move to.</param>
        /// <param name="x">An optional x co-ordinate for the entity to move inside the new room.</param>
        /// <param name="y">An optional y co-ordinate for the entity to move inside the new room.</param>
        /// <example>
        /// <code>
        /// private void onDoorHotspotClicked()
        /// {
        ///     cPlayer.Say("Ok, time to get inside the house.");
        ///     cPlayer.ChangeRoom(rHouse, 120, 100); //Will move the player to the house (to co-ordinates: 120,100).
        /// }
        /// </code>
        /// </example>
		Task ChangeRoomAsync(IRoom room, float? x = null, float? y = null);
    }
}