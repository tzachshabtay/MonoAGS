using System;
namespace AGS.API
{
    /// <summary>
    /// The game starter interface is the interface that a game project should implement so that it can be loaded
    /// by the editor.
    /// When loading a game from the editor, the editor searches for an implementation of this interface in the game,
    /// and calls the "StartGame" method with the game it created so that the editor and the game will have a shared interface to the game.
    /// </summary>
    /// <example>
    /// An implementation of the game starter interface will look like this in its most basic form:
    /// 
    /// <code language="lang-csharp">
    /// public class MyGameStarter : IGameStarter
    /// {
    ///     IGameSettings Settings => new AGSGameSettings("My Game", new Size(1280, 800));
    /// 
    ///     public void StartGame(IGame game)
    ///     {
    ///         game.Events.OnLoad.Subscribe(onGameLoaded); //Here is where you create all of the rooms, characters, etc in the game.
    ///     }
    /// }
    /// <code>
    /// </example>
    public interface IGameStarter
    {
        /// <summary>
        /// Gets the settings for the game.
        /// </summary>
        /// <value>The settings.</value>
        IGameSettings Settings { get; }

        /// <summary>
        /// Creates the game.
        /// </summary>
        /// <returns>The game.</returns>
        void StartGame(IGame game);
    }
}
