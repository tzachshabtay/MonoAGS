using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AGS.API
{
    /// <summary>
    /// The game state. This is the entry point for all "changing" things in the game.
    /// </summary>
    public interface IGameState : IRoomProvider
	{
        /// <summary>
        /// The player character.
        /// </summary>
        /// <value>The player.</value>
        ICharacter Player { get; set; }

        /// <summary>
        /// All of the rooms in the game.
        /// </summary>
        /// <value>The rooms.</value>
		IAGSBindingList<IRoom> Rooms { get; }

        /// <summary>
        /// The viewport decides what part of the room is actually seen on screen, and also
        /// where it's projected on the screen..
        /// </summary>
        /// <value>The viewport.</value>
        IViewport Viewport { get; }

        /// <summary>
        /// Allows adding multiple viewports to the game (so you can have split-screen functionality, for example,
        /// or having a TV in one room showing things happenning in another room).
        /// </summary>
        /// <value>The secondary viewports.</value>
        IAGSBindingList<IViewport> SecondaryViewports { get; }

        /// <summary>
        /// Gets the viewports sorted (in ascending order) by their Z index.
        /// </summary>
        /// <returns>The sorted viewports.</returns>
        List<IViewport> GetSortedViewports();

        /// <summary>
        /// All of the non-room objects in the game. Those are usually GUIs which stay on the screen
        /// even if you switch between rooms (like the top bar, a hotspot label, inventory window, menus, etc).
        /// </summary>
        /// <value>The user interface.</value>
		IConcurrentHashSet<IObject> UI { get; }

        /// <summary>
        /// Gets the UI controls which are currently getting focus (for example if a yes/no dialog pops up, it's focused
        /// and "takes" away the input from the game. If a text box is currently in use, it's focused and "takes" away
        /// keyboard input from the game.
        /// </summary>
        /// <value>The focused user interface.</value>
        IFocusedUI FocusedUI { get; }

        /// <summary>
        /// Custom properties that you can use for the entire game, to set custom behaviors.
        /// For example, you can add a "Score" variable here, to keep the game score.
        /// </summary>
        /// <value>The global variables.</value>
		ICustomProperties GlobalVariables { get; }

        /// <summary>
        /// Controls the cutscenes.
        /// </summary>
        /// <value>The cutscene.</value>
		ICutscene Cutscene { get; }

        /// <summary>
        /// Controls the room transitions (animation effects when moving between rooms).
        /// </summary>
        /// <value>The room transitions.</value>
		IRoomTransitions RoomTransitions { get; }

        /// <summary>
        /// Pause or resume the game. If the game is paused the state of the game will not update.
        /// </summary>
        /// <value><c>true</c> if paused; otherwise, <c>false</c>.</value>
		bool Paused { get; set; }

        /// <summary>
        /// The speed of the game's updates. This is listed in percentage from 60 FPS. By default,
        /// this is 100, so 100% of 60 FPS which is 60 FPS. For 30 FPS, put 50 as your speed. Note
        /// that this is the target FPS, the target which the engine will attempt to accomplish, 
        /// there is no guarantee that it will actually succeed, though (that depends on the hardware
        /// and on how much resource-heavy is your game.
        /// </summary>
        /// <value>The speed.</value>
		int Speed { get; set; }

        /// <summary>
        /// Changes the game's room to a new room. This will trigger the room transition,
        /// and then switch the camera to render the new room.
        /// </summary>
        /// <returns>A task that can be awaited for the room switch to complete.</returns>
        /// <param name="newRoom">New room.</param>
        /// <param name="afterTransitionFadeOut">A possible callback which triggers after the fade out of the old room, but before the fade in of the new room.</param>
        Task ChangeRoomAsync(IRoom newRoom, Action afterTransitionFadeOut = null);

        /// <summary>
        /// Disposes resources for all current rooms and objects. This is called by the engine when loading a new game.
        /// </summary>
		void Clean();

        /// <summary>
        /// Overrides this state from another state. This is called by the engine when loading a new game.
        /// </summary>
        /// <param name="state">State.</param>
		void CopyFrom(IGameState state);

        /// <summary>
        /// Find the entity with the specified id and type.
        /// </summary>
        /// <returns>The entity if found, null if no entity exists with that id.</returns>
        /// <param name="id">The entity's unique identifier.</param>
        /// <typeparam name="TEntity">The entity's type.</typeparam>
		TEntity Find<TEntity>(string id) where TEntity : class, IEntity;
	}
}

