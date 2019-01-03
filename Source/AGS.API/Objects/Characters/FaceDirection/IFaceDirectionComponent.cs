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
    [RequiredComponent(typeof(IAnimationComponent))]
    [RequiredComponent(typeof(ITranslateComponent))]
	public interface IFaceDirectionComponent : IComponent
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
        /// <code language="lang-csharp">
        /// if (cEgo.Direction == Direction.Down)
        /// {
        ///     await cEgo.SayAsync("I'm looking right at you!");
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
        /// <code language="lang-csharp">
        /// cEgo.CurrentDirectionalAnimation = cEgo.Outfit.IdleAnimation;
        /// await cEgo.FaceDirectionAsync(Direction.Right); //character is facing right doing nothing (idle).
        /// cEgo.CurrentDirectionalAnimation = dancingAnimation;
        /// awaut cEgo.FaceDirectionAsync(Direction.Down); //character is now doing a dance right to our face!
        /// </code>
        /// </example>
        IDirectionalAnimation CurrentDirectionalAnimation { get; set; }

        /// <summary>
        /// Changes the direction the character is facing to asynchronously (without blocking the game).
        /// </summary>
        /// <param name="direction">The direction to face.</param>
        /// <returns>The task that should be awaited on to signal completion.</returns>
        /// <example>
        /// <code language="lang-csharp">
        /// private async Task faceDown()
        /// {
        ///     await cEgo.FaceDirectionAsync(Direction.Down);
        /// }
        /// </code>
        /// <code language="lang-csharp">
        /// private async Task faceDownAndDoStuffInBetween()
        /// {
        ///     Task faceDirectionCompleted = cEgo.FaceDirectionAsync(Direction.Down);
        ///     await cSomeOtherDude.SayAsync("Let me know when you finished turning around..");
        ///     await faceDirectionCompleted;;
        ///     await cEgo.SayAsync("All done!");
        /// }
        /// </code>
        /// </example>
        Task FaceDirectionAsync(Direction direction);

        /// <summary>
        /// Changes the direction the character is facing to asynchronously (without blocking the game), so it will face the specified object.
        /// </summary>
        /// <param name="obj">The object to face.</param>
        /// <returns>The task that should be awaited on to signal completion.</returns>
        /// <example>
        /// <code language="lang-csharp">
        /// private async Task lookAtMirror()
        /// {
        ///     await cEgo.FaceDirectionAsync(oMirror);
        /// }
        /// </code>
        /// <code language="lang-csharp">
        /// private async Task lookAtMirrorAndDoStuffInBetween()
        /// {
        ///     Task faceMirrorCompleted = cEgo.FaceDirectionAsync(oMirror);
        ///     await cSomeOtherDude.SayAsync("Let me know when you see the mirror.");
        ///     await faceMirrorCompleted;
        ///     await cEgo.SayAsync("Yes, I see it now!");
        /// }
        /// </code>
        /// </example>
        Task FaceDirectionAsync(IObject obj);

        /// <summary>
        /// Changes the direction the character is facing to asynchronously (without blocking the game), so it will face (x,y).
        /// </summary>
        /// <param name="x">The x position.</param>
        /// <param name="y">The y position.</param>
        /// <returns>The task that should be awaited on to signal completion.</returns>
        /// <example>
        /// <code language="lang-csharp">
        /// private async Task face100100()
        /// {
        ///     await cEgo.FaceDirectionAsync(100f,100f);
        /// }
        /// </code>
        /// </example>
        Task FaceDirectionAsync(float x, float y);

        /// <summary>
        /// Changes the direction the character is facing to asynchronously (without blocking the game), so it will face (toX,toY), assuming it is currently facing (fromX,fromY).
        /// </summary>
        /// <param name="fromX">From x.</param>
        /// <param name="fromY">From y.</param>
        /// <param name="toX">To x.</param>
        /// <param name="toY">To y.</param>
        /// <example>
        /// <code language="lang-csharp">
        /// private async Task armyLookOneByOne()
        /// {
        ///     await cGeneral.SayAsync("Everybody, look at the way I'm looking.");
        ///     await cGeneral.FaceDirectionAsync(150f, 150f);
        ///     foreach (var soldier in soldiers)
        ///     {
        ///         await soldier.FaceDirectionAsync(cGeneral.X, cGeneral.Y, 150f, 150f); //Awaiting each soldier before going over to the next, meaning the will change directions one by one.
        ///     }    
        ///     await cGeneral.SayAsync("Took you long enough!");    
        /// }
        /// </code>
        /// <code language="lang-csharp">
        /// private async Task armyLookAtTheSameTime()
        /// {
        ///     await cGeneral.SayAsync("Everybody, look at the way I'm looking.");
        ///     await cGeneral.FaceDirectionAsync(150f, 150f);
        ///     List{Task} tasks = new List{Task}(soldiers.Count);
        ///     foreach (var soldier in soldiers)
        ///     {
        ///         tasks.Add(soldier.FaceDirectionAsync(cGeneral.X, cGeneral.Y, 150f, 150f)); 
        ///     }
        ///     await Task.WhenAll(tasks);
        ///     await cGeneral.SayAsync("Took you long enough!");
        /// }
        /// </code>
        /// <code language="lang-csharp">
        /// private async Task armyLookAtRandomTimes()
        /// {
        ///     await cGeneral.SayAsync("Everybody, look at the way I'm looking.");
        ///     await cGeneral.FaceDirectionAsync(150f, 150f);
        ///     List{Task} tasks = new List{Task}(soldiers.Count);
        ///     foreach (var soldier in soldiers)
        ///     {
        ///         tasks.Add(Task.Delay(MathUtils.Random().Next(10,50)).ContinueWith(soldier.FaceDirectionAsync(cGeneral.X, cGeneral.Y, 150f, 150f))); 
        ///     }
        ///     await Task.WhenAll(tasks);
        ///     await cGeneral.SayAsync("Took you long enough!");
        /// }
        /// </code>
        /// </example>
		Task FaceDirectionAsync(float fromX, float fromY, float toX, float toY);
	}
}
