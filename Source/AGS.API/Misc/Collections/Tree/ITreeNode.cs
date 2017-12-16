using System.Collections.Generic;

namespace AGS.API
{
    /// <summary>
    /// A node in a tree.
    /// A tree is a structure in which every node can have one parent (or no parent if it's the root)
    /// and several children (or no children, which will make it a leaf in the tree).
    /// </summary>
    public interface ITreeNode<TItem> where TItem : class, IInTree<TItem>
	{
        /// <summary>
        /// Gets the item contained in the node.
        /// </summary>
        /// <value>The node.</value>
		TItem Node { get; }

        /// <summary>
        /// Gets the item contained in the parent.
        /// </summary>
        /// <value>The parent.</value>
		TItem Parent { get; }

        /// <summary>
        /// Gets the items contained in the children.
        /// </summary>
        /// <value>The children.</value>
        IAGSBindingList<TItem> Children { get; }

        /// <summary>
        /// An event which fires whenever the parent for a node changes.
        /// </summary>
        /// <value>The event.</value>
        IBlockingEvent OnParentChanged { get; }

        /// <summary>
        /// Sets a new parent for the node.
        /// </summary>
        /// <param name="parent">Parent.</param>
		void SetParent(ITreeNode<TItem> parent);

        /// <summary>
        /// "Steals" the parent from the specified node: this will make this node have the specified node's parent
        /// as its parent, and the specified node will remain with no parent.
        /// </summary>
        /// <param name="victim">Victim.</param>
		void StealParent(ITreeNode<TItem> victim);

        /// <summary>
        /// Returns the root of the tree (the node which has no parent).
        /// </summary>
        /// <returns>The root.</returns>
        TItem GetRoot();

        /// <summary>
        /// Gets the number of children this node has.
        /// </summary>
        /// <value>The children count.</value>
		int ChildrenCount { get; }

        /// <summary>
        /// Adds a child to the node.
        /// </summary>
        /// <param name="child">Child.</param>
		void AddChild(TItem child);

        /// <summary>
        /// Adds the list of children to the node.
        /// </summary>
        /// <param name="children">Children.</param>
        void AddChildren(List<TItem> children);

        /// <summary>
        /// Removes the specified child from the node.
        /// </summary>
        /// <param name="child">Child.</param>
		void RemoveChild(TItem child);

        /// <summary>
        /// Is the specified item a child of this node?
        /// </summary>
        /// <returns><c>true</c>, if it is a child of the node, <c>false</c> otherwise.</returns>
        /// <param name="child">Child.</param>
		bool HasChild(TItem child);
	}

    /// <summary>
    /// A component which adds the ability for an entity to be a part of an entity's tree.
    /// </summary>
    /// <seealso cref="IComponent"/>
	public interface IInTree<TItem> : IComponent where TItem : class, IInTree<TItem>
	{
        /// <summary>
        /// Gets the tree node, which allows to compose various entity together in a tree-like hierarchy.
        /// </summary>
        /// <value>The tree node.</value>
		ITreeNode<TItem> TreeNode { get; }
	}

    /// <summary>
    /// A component which adds the ability for an entity to be a part of an object's tree.
    /// </summary>
    /// <seealso cref="IComponent"/>
	public interface IInObjectTreeComponent : IInTree<IObject> {}
}

