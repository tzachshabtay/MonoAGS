namespace AGS.API
{
    /// <summary>
    /// Adds the ability for an entity to be disabled/enabled.
    /// Only enabled entities can be interacted with by the player.
    /// </summary>
	[RequiredComponent(typeof(IInObjectTree))]
	public interface IEnabledComponent : IComponent
	{
		/// <summary>
		/// Gets or sets a value indicating whether this entity is enabled.
		/// Note that even if you set the entity to be enabled, it might still appear as disabled if
		/// it's part of a tree and its parent (or the parent's parent, or the parent's grandparent, and so on) 
		/// is itself disabled.
		/// However the setting will be remembered (via the <see cref="UnderlyingEnabled"/> property) so if the parent will become enabled the entity will also
		/// become enabled if was set to be enabled before.
		/// </summary>
		/// <example>
		/// <code>
		/// button.TreeNode.SetParent(null); // the button has no parent
		/// button.Enabled = true;
		/// Debug.WriteLine("Is button enabled? {0}, button.Enabled); //This will print "Is button enabled? True".
		/// button.TreeNode.SetParent(panel); // the panel is now the parent of the button.
		/// panel.Enabled = false;
		/// Debug.WriteLine("Is button enabled? {0}, button.Enabled); //This will print "Is button enabled? False" because the parent is disabled.
		/// panel.Enabled = true;
		/// Debug.WriteLine("Is button enabled? {0}, button.Enabled); //This will print "Is button enabled? True" because both the parent and the button are enabled.
		/// </code>
		/// </example>
		/// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
		bool Enabled { get; set; }

        /// <summary>
        /// Gets a value indicating whether this entity underlying property is enabled.
        /// If the entity was set to be enabled but its parent is disabled, the entity will read as disabled.
        /// The "UnderlyingEnabled" property however, will remember that the entity was set to be enabled,
        /// so in case the parent will be enabled, the entity will remember that it is now enabled as well.
        /// </summary>
        /// <seealso cref="Enabled"/>
        /// <value><c>true</c> if underlying enabled; otherwise, <c>false</c>.</value>
		bool UnderlyingEnabled { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this entity is click-through, meaning it can't be clicked.
        /// The difference between setting an entity to be click-through and between disabling it (by setting its
        /// <see cref="Enabled"/> property to be false), is that unlike disabling, the children of the entity
        /// will not be affected.  
        /// </summary>
        /// <value><c>true</c> if click through; otherwise, <c>false</c>.</value>
        bool ClickThrough { get; set; }

		/// <summary>
		/// An event which fires whenever enabled has changed for the entity.
		/// </summary>
		/// <value>The on visible changed.</value>
		IEvent OnEnabledChanged { get; }

		/// <summary>
		/// An event which fires whenever underlying enabled has changed for the entity.
		/// </summary>
		/// <value>The on underlying visible changed.</value>
		IEvent OnUnderlyingEnabledChanged { get; }
	}
}

