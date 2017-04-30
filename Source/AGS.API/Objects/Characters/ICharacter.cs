namespace AGS.API
{
    /// <summary>
    /// A character is an entity with pre-set components (like talk, walk, and inventory) which is useful
    /// to depict adventure game characters.
    /// </summary>
	public interface ICharacter : IObject, ISayBehavior, IWalkBehavior, IFaceDirectionBehavior, 
		IHasOutfit, IHasInventory, IFollowBehavior
	{
	}
}

