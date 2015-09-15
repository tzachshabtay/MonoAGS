using System;
using AGS.API;
using AGS.Engine;

namespace AGS.Engine
{
	public class AGSTreeNode<TItem> : ITreeNode<TItem> where TItem : class, IInTree<TItem>
	{
		IConcurrentHashSet<TItem> _children;
		ITreeNode<TItem> _parent;

		public AGSTreeNode(TItem node, IConcurrentHashSet<TItem> children = null)
		{
			_children = children ?? new AGSConcurrentHashSet<TItem>();
			Node = node;
		}

		#region ITreeNode implementation

		public void AddChild(TItem child)
		{
			//Adding a child is a two step process (Parent property for the child changes first)
			if (child.TreeNode.Parent == Node)
				_children.Add(child);
			else child.TreeNode.SetParent(this);
		}

		public void RemoveChild(TItem child)
		{
			//Removing a child is a two step process (Parent property for the child changes first)
			if (child.TreeNode.Parent != Node)
				_children.Remove(child);
			else child.TreeNode.SetParent(null);
		}

		public bool HasChild(TItem child)
		{
			return _children.Contains(child);
		}

		public TItem Node { get; private set; }

		public TItem Parent
		{
			get 
			{
				if (_parent == null) return null;
				return _parent.Node;
			}
		}

		public void SetParent(ITreeNode<TItem> parent)
		{
			ITreeNode<TItem> prevParent = _parent;
			_parent = parent;
			if (prevParent != null)
			{
				prevParent.RemoveChild(Node);
			}
			if (_parent != null)
			{
				_parent.AddChild(Node);
			}
		}
			
		public int ChildrenCount
		{
			get
			{
				return _children.Count;
			}
		}

		#endregion
	}
}

