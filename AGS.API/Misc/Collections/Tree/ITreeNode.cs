using System.Collections.Generic;

namespace AGS.API
{
    public interface ITreeNode<TItem> where TItem : class, IInTree<TItem>
	{
		TItem Node { get; }
		TItem Parent { get; }
		IEnumerable<TItem> Children { get; }

		void SetParent(ITreeNode<TItem> parent);
		void StealParent(ITreeNode<TItem> victim);

		int ChildrenCount { get; }

		void AddChild(TItem child);
		void RemoveChild(TItem child);
		bool HasChild(TItem child);
	}

	public interface IInTree<TItem> : IComponent where TItem : class, IInTree<TItem>
	{
		ITreeNode<TItem> TreeNode { get; }
	}

	public interface IInObjectTree : IInTree<IObject> {}
}

