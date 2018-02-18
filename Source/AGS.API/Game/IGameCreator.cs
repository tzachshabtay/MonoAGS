using System;
namespace AGS.API
{
    /// <summary>
    /// The game creator interface is the interface that a game project should implement so that it can be loaded
    /// by the editor.
    /// When loading a game from the editor, the editor searches for an implementation of this interface in the game,
    /// and calls the "CreateGame" method which creates the game and gives a reference of the game to the editor.
    /// </summary>
    /// <example>
    /// An implementation of the game creator interface will look like this in its most basic form:
    /// 
    /// <code language="lang-csharp">
    /// public class MyGameCreator : IGameCreator
    /// {
    ///     public IGame CreateGame()
    ///     {
    ///         IGame game = AGSGame.CreateEmpty();
    ///         game.Events.OnLoad.Subscribe(onGameLoaded); //Here is where you create all of the rooms, characters, etc in the game.
    ///         return game;
    ///     }
    /// }
    /// <code>
    /// </example>
    public interface IGameCreator
    {
        /// <summary>
        /// Creates the game.
        /// </summary>
        /// <returns>The game.</returns>
        IGame CreateGame();
    }
}
