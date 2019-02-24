using System.Threading.Tasks;

namespace AGS.API
{
    /// <summary>
    /// Allows a character to approach a hotspot. 
    /// When an interaction event is triggered for a hotspot, if this component is available for the player (which it is by default),
    /// then the character will approach the hotspot before the interaction logic code is triggered.
    /// </summary>
    [RequiredComponent(typeof(IFaceDirectionComponent))]
    [RequiredComponent(typeof(IWalkComponent))]
    public interface IApproachComponent : IComponent
    {
        /// <summary>
        /// Gets or sets the approach style.
        /// </summary>
        /// <value>The approach style.</value>
        IApproachStyle ApproachStyle { get; set; }

        /// <summary>
        /// Approachs the target hotspot for the given verb (Look, Interact, etc).
        /// </summary>
        /// <returns>Did the approach complete successfully? (i.e was not interrupted by the player or any other game code).</returns>
        /// <param name="verb">The verb (Look, Interact, etc).</param>
        /// <param name="target">The target hotspot.</param>
        Task<bool> ApproachAsync(string verb, IObject target);
    }
}
