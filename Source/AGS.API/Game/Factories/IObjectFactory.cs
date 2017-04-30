using System.Threading.Tasks;

namespace AGS.API
{
    /// <summary>
    /// A factory for creating objects, characters, and masked hotspots.
    /// </summary>
    public interface IObjectFactory
	{
        /// <summary>
        /// Creates a new object.
        /// </summary>
        /// <returns>The object.</returns>
        /// <param name="id">A unique identifier for the object (this has to be globally unique across all entities).</param>
        /// <param name="sayWhenLook">An optional list of things that the player will say (one after the other) when looking on the object.</param>
        /// <param name="sayWhenInteract">An optional list of things that the player will say (one after the other) when interacting with the object.</param>
		IObject GetObject(string id, string[] sayWhenLook = null, string[] sayWhenInteract = null);

        /// <summary>
        /// Creates a new character
        /// </summary>
        /// <returns>The character.</returns>
        /// <param name="id">A unique identifier for the object (this has to be globally unique across all entities)..</param>
        /// <param name="outfit">An animation outfit for the character (this can be created from the outfit factory).</param>
        /// <param name="sayWhenLook">An optional list of things that the player will say (one after the other) when looking on the character.</param>
        /// <param name="sayWhenInteract">An optional list of things that the player will say (one after the other) when interacting with the character.</param>
		ICharacter GetCharacter(string id, IOutfit outfit, string[] sayWhenLook = null, string[] sayWhenInteract = null);

        /// <summary>
        /// Creates a new character
        /// </summary>
        /// <returns>The character.</returns>
        /// <param name="id">A unique identifier for the object (this has to be globally unique across all entities).</param>
        /// <param name="innerContainer">An animation container for the character</param>
		ICharacter GetCharacter(string id, IOutfit outfit, IAnimationContainer innerContainer);

        /// <summary>
        /// Creates a new hotspot object from a bitmap mask (the hotspot text will be shown when the mouse is hovering the object if a hotspot label is in the game).
        /// </summary>
        /// <returns>The hotspot.</returns>
        /// <param name="maskPath">The path of the resource/file (see cref="IResourceLoader"/> to load the mask from.</param>
        /// <param name="hotspot">The hotspot text.</param>
        /// <param name="sayWhenLook">An optional list of things that the player will say (one after the other) when looking on the object.</param>
        /// <param name="sayWhenInteract">An optional list of things that the player will say (one after the other) when interacting with the object.</param>
        /// <param name="id">A unique identifier for the object (this has to be globally unique across all entities). If an ID is not given, the hotspot text will be used as the id.</param>
		IObject GetHotspot(string maskPath, string hotspot, string[] sayWhenLook = null, string[] sayWhenInteract = null, string id = null);

        /// <summary>
        /// Creates a new hotspot object from a bitmap mask asynchronously (the hotspot text will be shown when the mouse is hovering the object if a hotspot label is in the game).
        /// </summary>
        /// <returns>The hotspot.</returns>
        /// <param name="maskPath">The path of the resource/file (<see cref="IResourceLoader"/> to load the mask from.</param>
        /// <param name="hotspot">The hotspot text.</param>
        /// <param name="sayWhenLook">An optional list of things that the player will say (one after the other) when looking on the object.</param>
        /// <param name="sayWhenInteract">An optional list of things that the player will say (one after the other) when interacting with the object.</param>
        /// <param name="id">A unique identifier for the object (this has to be globally unique across all entities). If an ID is not given, the hotspot text will be used as the id.</par    
        Task<IObject> GetHotspotAsync(string maskPath, string hotspot, string [] sayWhenLook = null, string [] sayWhenInteract = null, string id = null);
	}
}

