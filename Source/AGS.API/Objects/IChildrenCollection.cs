using System.Collections.Generic;

namespace AGS.API
{
    /// <summary>
    /// A collection of children (to be used in a tree like structure).
    /// </summary>
    public interface IChildrenCollection : IEnumerable<IObject>
	{
        /// <summary>
        /// The number of children.
        /// </summary>
        /// <value>The count.</value>
		int Count { get; }

        /// <summary>
        /// Adds a child.
        /// </summary>
        /// <param name="child">Child.</param>
		void AddChild(IObject child);

        /// <summary>
        /// Removes the child.
        /// </summary>
        /// <param name="child">Child.</param>
		void RemoveChild(IObject child);

        /// <summary>
        /// Checks whether the specified object is a child in this collection.
        /// </summary>
        /// <returns><c>true</c>, if this is one of the existing children, <c>false</c> otherwise.</returns>
        /// <param name="child">Child.</param>
		bool HasChild(IObject child);
	}
}

