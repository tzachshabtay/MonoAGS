using System;

namespace API
{
	public interface ITreeNode<TItem> where TItem : class, IInTree<TItem>
	{
		TItem Node { get; }
		TItem Parent { get; }

		void SetParent(ITreeNode<TItem> parent);

		int ChildrenCount { get; }

		void AddChild(TItem child);
		void RemoveChild(TItem child);
		bool HasChild(TItem child);
	}

	public interface IInTree<TItem> where TItem : class, IInTree<TItem>
	{
		ITreeNode<TItem> TreeNode { get; }
	}
}

