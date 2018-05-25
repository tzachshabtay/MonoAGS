namespace AGS.API
{
    /// <summary>
    /// Gives an entity the ability to have an outfit (and be able to change outfits).
    /// </summary>
	public interface IOutfitComponent : IComponent
	{
        /// <summary>
        /// Gets or sets the outfit for the entity.
        /// </summary>
        /// <example>
        /// <code language="lang-csharp">
        /// await cHero.SayAsync("Let's try on this cool hat."); //Hero speaks with the normal outfit- without the hat
        /// var normalOutfit = cHero.Outfit; //Saving the current outfit so we can wear it back later
        /// cHero.Outfit = outfitWithHat;
        /// await cHero.SayAsync("Hmm, it looks nice, but not sure if it's comfortable."); //Character's animation now uses the hat for speaking.
        /// await cHero.WalkAsync(walkPoint); //Character's walk animation now includes the hat.
        /// cHero.Outfit = normalOutfit;
        /// await cHero.SayAsync("Yeah, it's not comfortable, I took it down."); //Character's animation is now again without the hat
        /// </code>
        /// </example>
        /// <seealso cref="IOutfit"/>
        /// <value>The outfit.</value>
		IOutfit Outfit { get; set; }
	}
}

