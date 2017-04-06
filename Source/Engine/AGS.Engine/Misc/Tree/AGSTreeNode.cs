using AGS.API;
using System.Collections.Generic;

namespace AGS.Engine
{
	public class AGSTreeNode<TItem> : ITreeNode<TItem> where TItem : class, IInTree<TItem>
	{
		private readonly IConcurrentHashSet<TItem> _children;
		private ITreeNode<TItem> _parent;
        private readonly AGSEventArgs _args = new AGSEventArgs();

		public AGSTreeNode(TItem node = null, IConcurrentHashSet<TItem> children = null)
		{
            OnParentChanged = new AGSEvent<AGSEventArgs>();
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

		public TItem Node { get; set; }

		public TItem Parent
		{
			get 
			{
                var parent = _parent;
				if (parent == null) return null;
				return parent.Node;
			}
		}

		public IEnumerable<TItem> Children { get { return _children; } }

		public void SetParent(ITreeNode<TItem> parent)
		{
			if (_parent == parent) return;
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
            fireParentChanged();
		}

		public void StealParent(ITreeNode<TItem> victim)
		{
			if (victim.Parent == null)
			{
				SetParent(null);
				return;
			}
			SetParent(victim.Parent.TreeNode);
			victim.SetParent(null);
            fireParentChanged();
		}

        public IEvent<AGSEventArgs> OnParentChanged { get; private set; }
			
		public int ChildrenCount
		{
			get
			{
				return _children.Count;
			}
		}

        #endregion

        private void fireParentChanged()
        {
            OnParentChanged.FireEvent(this, _args);
        }
	}
}

