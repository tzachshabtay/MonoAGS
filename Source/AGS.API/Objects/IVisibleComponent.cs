namespace AGS.API
{
    /// <summary>
    /// Adds the ability to show/hide an entity.
    /// </summary>
	[RequiredComponent(typeof(IInObjectTree))]
	public interface IVisibleComponent : IComponent
	{
        /// <summary>
        /// Gets or sets a value indicating whether this entity is visible.
        /// Note that even if you set the entity to be visible, it might still appear as not visible if
        /// it's part of a tree and its parent (or the parent's parent, or the parent's grandparent, and so on) 
        /// is itself not visible.
        /// However the setting will be remembered (via the <see cref="UnderlyingVisible"/> property) so if the parent will become visible the entity will also
        /// become visible if was set to be visible before.
        /// </summary>
        /// <example>
        /// <code>
        /// button.TreeNode.SetParent(null); // the button has no parent
        /// button.Visible = true;
        /// Debug.WriteLine("Is button visible? {0}, button.Visible); //This will print "Is button visible? True".
        /// button.TreeNode.SetParent(panel); // the panel is now the parent of the button.
        /// panel.Visible = false;
        /// Debug.WriteLine("Is button visible? {0}, button.Visible); //This will print "Is button visible? False" because the parent is not visible.
        /// panel.Visible = true;
        /// Debug.WriteLine("Is button visible? {0}, button.Visible); //This will print "Is button visible? True" because both the parent and the button are visible.
        /// </code>
        /// </example>
        /// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
		bool Visible { get; set; }

        /// <summary>
        /// Gets a value indicating whether this entity underlying property is visible.
        /// If the entity was set to be visible but its parent is not visible, the entity will read as not visible.
        /// The "UnderlyingVisible" property however, will remember that the entity was set to be visible,
        /// so in case the parent will be visible, the entity will remember that it is now visible as well.
        /// </summary>
        /// <seealso cref="Visible"/>
        /// <value><c>true</c> if underlying visible; otherwise, <c>false</c>.</value>
		bool UnderlyingVisible { get; }

        /// <summary>
        /// An event which fires whenever underlying visible has changed for the entity.
        /// </summary>
        /// <value>The on underlying visible changed.</value>
        IEvent OnUnderlyingVisibleChanged { get; }
	}
}

