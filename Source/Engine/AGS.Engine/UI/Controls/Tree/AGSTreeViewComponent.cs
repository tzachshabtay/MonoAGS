using System;
using System.Collections.Generic;
using AGS.API;

namespace AGS.Engine
{
    public class AGSTreeViewComponent : AGSComponent, ITreeViewComponent
    {
        private Node _root;
        private IGameState _state;
        private IInObjectTree _tree;
        private IDrawableInfo _drawable;
        private bool _duringUpdate;
        
        public AGSTreeViewComponent(ITreeNodeViewProvider provider, IGameEvents gameEvents, IGameState state)
        {
            HorizontalSpacing = 4f;
            VerticalSpacing = 8f;
            OnNodeSelected = new AGSEvent<NodeEventArgs>();
            AllowSelection = SelectionType.Single;
            _state = state;
            NodeViewProvider = provider;
            gameEvents.OnRepeatedlyExecute.Subscribe(onRepeatedlyExecute);
        }

        public ITreeStringNode Tree { get; set; }

        public ITreeNodeViewProvider NodeViewProvider { get; set; }

        public float HorizontalSpacing { get; set; }

        public float VerticalSpacing { get; set; }

        public SelectionType AllowSelection { get; set; }

        public IEvent<NodeEventArgs> OnNodeSelected { get; private set; }

        public override void Init(IEntity entity)
        {
            base.Init(entity);
            _tree = entity.GetComponent<IInObjectTree>();
            _drawable = entity.GetComponent<IDrawableInfo>();
        }

        private void onRepeatedlyExecute(object sender, AGSEventArgs args)
        {
            if (_duringUpdate) return;
            _duringUpdate = true;
            try
            {
                var tree = Tree;
                _root = buildTree(_root, tree);
                processTree(_root);
                var root = _root;
                if (root != null) root.ResetOffsets(0f, 0f, HorizontalSpacing, -VerticalSpacing);
            }
            //todo: Currently we rebuild the tree on each tick which can be slow.
            //If it's too slow and the next game tick comes before we finished building, one of the children
            //in the tree might change from another component while we're iterating on it, 
            //which can cause the exception, which we currently ignore and the tree will be rebuilt on
            //the next tick.
            //We don't need to rebuild the tree on each tick, we can listen to events and only build when
            //something actually changes in the tree, so we might be able to remove this ugly catch.
            catch (InvalidOperationException) { } 
            _duringUpdate = false;
        }

        private void processTree(Node node)
        {
            if (node == null) return;
            addToUI(node);
            foreach (var child in node.Children)
            {
                processTree(child);
            }
        }

        private Node buildTree(Node currentNode, ITreeStringNode actualNode)
        {
            if (actualNode == null) return null;

            if (currentNode == null || currentNode.Item != actualNode)
            {
                if (currentNode != null) removeFromUI(currentNode);
                currentNode = new Node(actualNode, NodeViewProvider.CreateNode(actualNode, _drawable.RenderLayer), null, this);
                currentNode.View.ParentPanel.TreeNode.SetParent(_tree.TreeNode);
            }
            int maxChildren = Math.Max(currentNode.Children.Count, actualNode.TreeNode.Children.Count);
            for (int i = 0; i < maxChildren; i++)
            {
                var nodeChild = childOrNull(currentNode.Children, i);
                var actualChild = childOrNull(actualNode.TreeNode.Children, i);
                if (nodeChild == null && actualChild == null) continue;
                if (nodeChild == null)
                {
                    var newNode = new Node(actualChild, NodeViewProvider.CreateNode(actualChild, _drawable.RenderLayer), currentNode, this);
					newNode = buildTree(newNode, actualChild);
                    currentNode.Children.Add(newNode);
                    continue;
                }
                if (nodeChild.Item == actualChild) 
                {
                    nodeChild = buildTree(nodeChild, actualChild);
                    currentNode.Children[i] = nodeChild;
                    continue;   
                }

				removeFromUI(nodeChild);
				currentNode.Children.RemoveAt(i);
				i--;
            }
            return currentNode;
        }

        private TChild childOrNull<TChild>(IList<TChild> list, int i)
        {
            if (list.Count <= i) return default(TChild);
            return list[i];
        }

        private void addToUI(Node node)
        {
            NodeViewProvider.BeforeDisplayingNode(node.Item, node.View, 
                                                  node.IsCollapsed, node.IsHovered, node.IsSelected);
            if (!node.IsNew) return;
            node.IsNew = false;
            _state.UI.Add(node.View.ParentPanel);
            _state.UI.Add(node.View.TreeItem.TreeNode.Parent);
            _state.UI.Add(node.View.TreeItem);
            if (node.View.ExpandButton != null)
            {
                _state.UI.Add(node.View.ExpandButton);
            }
        }

        private void removeFromUI(Node node) 
        {
            var obj = node.Item as IObject;
            if (obj != null)
            {
                _state.UI.Remove(obj);
            }
            node.Dispose();
        }

        private class Node : IDisposable
        {
            private ITreeViewComponent _tree;

            public Node(ITreeStringNode item, ITreeNodeView view, Node parentNode, ITreeViewComponent tree)
            {
                _tree = tree;
                Item = item;
                View = view;
                Parent = parentNode;
                Children = new List<Node>();
                IsCollapsed = true;
                IsNew = true;

                if (parentNode != null)
                {
                    view.ParentPanel.TreeNode.SetParent(parentNode.View.VerticalPanel.TreeNode);
                    view.ParentPanel.Visible = !parentNode.IsCollapsed;
                }

                view.TreeItem.MouseEnter.Subscribe(onMouseEnter);
                view.TreeItem.MouseLeave.Subscribe(onMouseLeave);
                View.TreeItem.MouseClicked.Subscribe(onItemSelected);
                var expandButton = View.ExpandButton;
                if (expandButton != null)
                {
                    expandButton.MouseEnter.Subscribe(onMouseEnter);
                    expandButton.MouseLeave.Subscribe(onMouseLeave);
                }
                getExpandButton().MouseClicked.Subscribe(onMouseClicked);
            }

            public ITreeStringNode Item { get; private set; }
            public ITreeNodeView View { get; private set; }
            public List<Node> Children { get; private set; }
            public Node Parent { get; private set; }

            public bool IsNew { get; set; }
            public bool IsCollapsed { get; set; }
            public bool IsHovered { get; set; }
            public bool IsSelected { get; private set; }
            public float XOffset { get; private set; }

            public void Dispose()
            {
                View.TreeItem.MouseEnter.Unsubscribe(onMouseEnter);
                View.TreeItem.MouseLeave.Unsubscribe(onMouseLeave);
                View.TreeItem.MouseClicked.Unsubscribe(onItemSelected);
                var expandButton = View.ExpandButton;
                if (expandButton != null)
                {
                    expandButton.MouseEnter.Unsubscribe(onMouseEnter);
                    expandButton.MouseLeave.Unsubscribe(onMouseLeave);
                }
                getExpandButton().MouseClicked.Unsubscribe(onMouseClicked);
            }

            public float ResetOffsets(float xOffset, float yOffset, float spacingX, float spacingY)
            {
                xOffset += spacingX;
                View.VerticalPanel.X = xOffset;
                View.ParentPanel.Y = yOffset;
                var childYOffset = spacingY;
                foreach (var child in Children)
                {
                    childYOffset = child.ResetOffsets(xOffset, childYOffset, spacingX, spacingY);
                }
                if (!View.ParentPanel.Visible) return yOffset;
                return yOffset + childYOffset;
            }

            public void ResetSelection()
            {
                IsSelected = false;
                foreach (var child in Children)
                {
                    child.ResetSelection();
                }
            }

            private Node getRoot()
            {
                var root = this;
                while (root.Parent != null) root = root.Parent;
                return root;
            }

            private IUIEvents getExpandButton()
            {
                return (IUIEvents)View.ExpandButton ?? View.TreeItem;
            }

            private void onMouseEnter(object sender, MousePositionEventArgs args)
            {
                IsHovered = true;
            }

            private void onMouseLeave(object sender, MousePositionEventArgs args)
            {
                IsHovered = false;                
            }

            private void onMouseClicked(object sender, MouseButtonEventArgs args)
            {
                IsCollapsed = !IsCollapsed;
                foreach (var child in Children)
                {
                    child.View.ParentPanel.Visible = !IsCollapsed;
                }
            }

            private void onItemSelected(object sender, MouseButtonEventArgs args)
            {
                getRoot().ResetSelection();
                if (_tree.AllowSelection == SelectionType.None) return;
                IsSelected = true;
                _tree.OnNodeSelected.Invoke(this, new NodeEventArgs(Item));
            }
        }
    }
}
