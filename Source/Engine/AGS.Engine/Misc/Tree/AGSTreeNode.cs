using AGS.API;
using System.Collections.Generic;
using System.Linq;

namespace AGS.Engine
{
	public class AGSTreeNode<TItem> : ITreeNode<TItem> where TItem : class, IInTree<TItem>
	{
        private readonly IAGSBindingList<TItem> _children;
		private ITreeNode<TItem> _parent;

		public AGSTreeNode(TItem node = null, IAGSBindingList<TItem> children = null)
		{
            OnParentChanged = new AGSEvent();
            _children = children ?? new AGSBindingList<TItem>(5);
			Node = node;
		}

		#region ITreeNode implementation

		public void AddChild(TItem child)
		{
            //Adding a child is a two step process (Parent property for the child changes first)
            if (child.TreeNode.Parent == Node)
            {
                if (!HasChild(child)) _children.Add(child);
            }
			else child.TreeNode.SetParent(this);
		}

        public void AddChildren(List<TItem> children)
        {
            foreach (var child in children)
            {
                if (HasChild(child)) continue;
                ((AGSTreeNode<TItem>)child.TreeNode).setParentOneWay(this);
            }
            _children.AddRange(children);
        }

		public void RemoveChild(TItem child)
		{
			//Removing a child is a two step process (Parent property for the child changes first)
			if (child.TreeNode.Parent != Node)
				_children.Remove(child);
			else child.TreeNode.SetParent(null);
		}

        public bool HasChild(TItem child) => _children.Contains(child);

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

        public IAGSBindingList<TItem> Children => _children;

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

        public TItem GetRoot()
        {
            var root = Node;
            while (root.TreeNode.Parent != null) root = root.TreeNode.Parent;
            return root;
        }

        public IBlockingEvent OnParentChanged { get; }

        public int ChildrenCount => _children.Count;

        #endregion

        private void fireParentChanged()
        {
            OnParentChanged.Invoke();
        }

        private void setParentOneWay(ITreeNode<TItem> parent)
        {
            if (_parent == parent) return;
            ITreeNode<TItem> prevParent = _parent;
            _parent = parent;
            if (prevParent != null)
            {
                prevParent.RemoveChild(Node);
            }
            fireParentChanged();
        }
	}
}

