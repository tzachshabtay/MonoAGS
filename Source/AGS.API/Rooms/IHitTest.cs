using System;

namespace AGS.API
{
    /// <summary>
    /// Responsible for returning the current object that is located under the mouse position.
    /// </summary>
    public interface IHitTest
    {
        /// <summary>
        /// Gets the object at the mouse position (this ignores objects that are not visible or enabled).
        /// If there is more than one object in that position it returns the one in front.
        /// </summary>
        /// <value>The object at mouse position.</value>
        IObject ObjectAtMousePosition { get; }

        /// <summary>
        /// Gets the object at the (x,y) world position. If there is more than one object in that position it returns the
        /// one in front.
        /// You can also optionally pass a filter which allows to ignore specific objects from the hit tests.
        /// 
        /// Note that if you just need the object at the mouse position, you should use <see cref="ObjectAtMousePosition"/>, this is cached every tick and will be faster.
        /// </summary>
        /// <returns>The <see cref="T:AGS.API.IObject"/> at that world position.</returns>
        /// <param name="x">The x coordinate (in world co-ordinates).</param>
        /// <param name="y">The y coordinate (in world co-ordinates).</param>
        /// <param name="filter">An optional filter.</param>
        /// <example>
        /// To get GUIs only you can do something like:
        /// <code language="lang-csharp">
        /// var gui = hitTest.GetObjectAt(200, 100, obj => obj is IUIControl);
        /// if (gui == null) await cHero.SayAsync("There's no GUI at (200,100)");
        /// else await cHero.SayAsync($"I see at {gui.DisplayName} at (200,100)!");
        /// </code>
        /// </example>
        IObject GetObjectAt(float x, float y, Predicate<IObject> filter = null);
    }
}