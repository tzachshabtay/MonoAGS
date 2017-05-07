namespace AGS.API
{
    /// <summary>
    /// Allows skinning (providing a common custom theme to) an entity.
    /// </summary>
    public interface ISkin
	{
        /// <summary>
        /// Apply the skin to the specified entity.
        /// </summary>
        /// <param name="entity">Entity.</param>
        void Apply(IEntity entity);
	}
}

