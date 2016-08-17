using System.Threading.Tasks;

namespace AGS.API
{
    /// <summary>
    /// A general direction to which a character can face.
    /// </summary>
    public enum Direction
	{
		Down,
		Up,
		Left,
		Right,
		DownLeft,
		DownRight,
		UpLeft,
		UpRight,
	}

    /// <summary>
    /// Gives the ability for a character to change the direction he/she is facing.
    /// </summary>
    /// <seealso cref="AGS.API.IComponent" />
    [RequiredComponent(typeof(IAnimationContainer))]
    [RequiredComponent(typeof(ITransformComponent))]
	public interface IFaceDirectionBehavior : IComponent
	{
        /// <summary>
        /// Gets the current direction that the character is facing.
        /// Down -> the character is looking directly at the camera.
        /// Up -> The character's back is facing the camera.
        /// </summary>
        /// <value>
        /// The direction.
        /// </value>
        /// <example>
        /// <code>
        /// if (cEgo.Direction == Direction.Down)
        /// {
        ///     cEgo.Say("I'm looking right at you!");
        /// }
        /// </code>
        /// </example>
        Direction Direction { get; }
        /// <summary>
        /// Gets or sets the current directional animation used by the character to face to different directions.
        /// </summary>
        /// <value>
        /// The current directional animation.
        /// </value>
        /// <example>
        /// <code>
        /// cEgo.CurrentDirectionalAnimation = cEgo.Outfit.IdleAnimation;
        /// cEgo.FaceDirection(Direction.Right); //character is facing right doing nothing (idle).
        /// cEgo.CurrentDirectionalAnimation = dancingAnimation;
        /// cEgo.FaceDirection(Direction.Down); //character is now doing a dance right to our face!
        /// </code>
        /// </example>
        IDirectionalAnimation CurrentDirectionalAnimation { get; set; }

        /// <summary>
        /// Changes the direction the character is facing to.
        /// </summary>
        /// <param name="direction">The direction to face.</param>
        /// <example>
        /// <code>
        /// cEgo.FaceDirection(Direction.Up);
        /// cEgo.Say("I'm standing with my back at you.");
        /// </code>
        /// </example>
        void FaceDirection(Direction direction);
        /// <summary>
        /// Changes the direction the character is facing to asynchronously (without blocking the game).
        /// </summary>
        /// <param name="direction">The direction to face.</param>
        /// <returns>The task that should be awaited on to signal completion.</returns>
        /// <example>
        /// <code>
        /// private async Task faceDown()
        /// {
        ///     await cEgo.FaceDirectionAsync(Direction.Down);
        /// }
        /// </code>
        /// <code>
        /// private async Task faceDownAndDoStuffInBetween()
        /// {
        ///     Task faceDirectionCompleted = cEgo.FaceDirectionAsync(Direction.Down);
        ///     cSomeOtherDude.Say("Let me know when you finished turning around..");
        ///     await faceDirectionCompleted;;
        ///     cEgo.Say("All done!");
        /// }
        /// </code>
        /// </example>
        Task FaceDirectionAsync(Direction direction);

        /// <summary>
        /// Changes the direction the character is facing to, so it will face the specified object.
        /// </summary>
        /// <param name="obj">The object to face.</param>
        /// <example>
        /// <code>
        /// cEgo.FaceDirection(oMirror);
        /// cEgo.Say("Mirror mirror on the wall, I'm watching you!");
        /// </code>
        /// </example>
        void FaceDirection(IObject obj);
        /// <summary>
        /// Changes the direction the character is facing to asynchronously (without blocking the game), so it will face the specified object.
        /// </summary>
        /// <param name="obj">The object to face.</param>
        /// <returns>The task that should be awaited on to signal completion.</returns>
        /// <example>
        /// <code>
        /// private async Task lookAtMirror()
        /// {
        ///     await cEgo.FaceDirection(oMirror);
        /// }
        /// </code>
        /// <code>
        /// private async Task lookAtMirrorAndDoStuffInBetween()
        /// {
        ///     Task faceMirrorCompleted = cEgo.FaceDirectionAsync(oMirror);
        ///     cSomeOtherDude.Say("Let me know when you see the mirror.");
        ///     await faceMirrorCompleted;
        ///     cEgo.Say("Yes, I see it now!");
        /// }
        /// </code>
        /// </example>
        Task FaceDirectionAsync(IObject obj);

        /// <summary>
        /// Changes the direction the character is facing to, so it will face (x,y).
        /// </summary>
        /// <param name="x">The x position.</param>
        /// <param name="y">The y position.</param>
        /// <example>
        /// <code>
        /// cEgo.FaceDirection(100f,100f);
        /// </code>
        /// </example>
        void FaceDirection(float x, float y);
        /// <summary>
        /// Changes the direction the character is facing to asynchronously (without blocking the game), so it will face (x,y).
        /// </summary>
        /// <param name="x">The x position.</param>
        /// <param name="y">The y position.</param>
        /// <returns>The task that should be awaited on to signal completion.</returns>
        /// <example>
        /// <code>
        /// private async Task face100100()
        /// {
        ///     await cEgo.FaceDirection(100f,100f);
        /// }
        /// </code>
        /// </example>
        Task FaceDirectionAsync(float x, float y);

        /// <summary>
        /// Changes the direction the character is facing to, so it will face (toX,toY), assuming it is currently facing (fromX,fromY).
        /// </summary>
        /// <param name="fromX">From x.</param>
        /// <param name="fromY">From y.</param>
        /// <param name="toX">To x.</param>
        /// <param name="toY">To y.</param>
        /// <example>
        /// <code>
        /// cGeneral.Say("Everybody, look at the way I'm looking.");
        /// cGeneral.FaceDirection(150f, 150f);
        /// foreach (var soldier in soldiers)
        /// {
        ///     soldier.FaceDirection(cGeneral.X, cGeneral.Y, 150f, 150f);
        /// }
        /// </code>
        /// </example>
        void FaceDirection(float fromX, float fromY, float toX, float toY);
        /// <summary>
        /// Changes the direction the character is facing to asynchronously (without blocking the game), so it will face (toX,toY), assuming it is currently facing (fromX,fromY).
        /// </summary>
        /// <param name="fromX">From x.</param>
        /// <param name="fromY">From y.</param>
        /// <param name="toX">To x.</param>
        /// <param name="toY">To y.</param>
        /// <example>
        /// <code>
        /// private async Task armyLookOneByOne()
        /// {
        ///     cGeneral.Say("Everybody, look at the way I'm looking.");
        ///     cGeneral.FaceDirection(150f, 150f);
        ///     foreach (var soldier in soldiers)
        ///     {
        ///         await soldier.FaceDirectionAsync(cGeneral.X, cGeneral.Y, 150f, 150f); //Awaiting each soldier before going over to the next, meaning the will change directions one by one.
        ///     }    
        ///     cGeneral.Say("Took you long enough!");    
        /// }
        /// </code>
        /// <code>
        /// private async Task armyLookAtTheSameTime()
        /// {
        ///     cGeneral.Say("Everybody, look at the way I'm looking.");
        ///     cGeneral.FaceDirection(150f, 150f);
        ///     List{Task} tasks = new List{Task}(soldiers.Count);
        ///     foreach (var soldier in soldiers)
        ///     {
        ///         tasks.Add(soldier.FaceDirectionAsync(cGeneral.X, cGeneral.Y, 150f, 150f)); 
        ///     }
        ///     await Task.WhenAll(tasks);
        ///     cGeneral.Say("Took you long enough!");
        /// }
        /// </code>
        /// <code>
        /// private async Task armyLookAtRandomTimes()
        /// {
        ///     cGeneral.Say("Everybody, look at the way I'm looking.");
        ///     cGeneral.FaceDirection(150f, 150f);
        ///     List{Task} tasks = new List{Task}(soldiers.Count);
        ///     foreach (var soldier in soldiers)
        ///     {
        ///         tasks.Add(Task.Delay(MathUtils.Random().Next(10,50)).ContinueWith(soldier.FaceDirectionAsync(cGeneral.X, cGeneral.Y, 150f, 150f))); 
        ///     }
        ///     await Task.WhenAll(tasks);
        ///     cGeneral.Say("Took you long enough!");
        /// }
        /// </code>
        /// </example>
		Task FaceDirectionAsync(float fromX, float fromY, float toX, float toY);
	}
}

