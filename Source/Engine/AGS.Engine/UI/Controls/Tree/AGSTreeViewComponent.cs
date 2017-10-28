using System;
using System.Collections.Generic;
using AGS.API;

namespace AGS.Engine
{
    public class AGSTreeViewComponent : AGSComponent, ITreeViewComponent
    {
        private Node _root;
        private IGameState _state;
        private IInObjectTree _treeComponent;
        private ITreeStringNode _tree;
        private IDrawableInfo _drawable;

        public AGSTreeViewComponent(ITreeNodeViewProvider provider, IGameState state)
        {
            HorizontalSpacing = 10f;
            VerticalSpacing = 30f;
            OnNodeSelected = new AGSEvent<NodeEventArgs>();
            OnNodeExpanded = new AGSEvent<NodeEventArgs>();
            OnNodeCollapsed = new AGSEvent<NodeEventArgs>();
            AllowSelection = SelectionType.Single;
            _state = state;
            NodeViewProvider = provider;
        }

        public ITreeStringNode Tree
        {
            get { return _tree; }
            set 
            {
                clearTreeFromUi(_root);
                unsubscribeTree(_tree);
                _tree = value;
                subscribeTree(_tree);
                RebuildTree();
            }
        }

        public ITreeNodeViewProvider NodeViewProvider { get; set; }

        public float HorizontalSpacing { get; set; }

        public float VerticalSpacing { get; set; }

        public SelectionType AllowSelection { get; set; }

        public IEvent<NodeEventArgs> OnNodeSelected { get; private set; }

        public IEvent<NodeEventArgs> OnNodeExpanded { get; private set; }

        public IEvent<NodeEventArgs> OnNodeCollapsed { get; private set; }

        public override void Init(IEntity entity)
        {
            base.Init(entity);
            entity.Bind<IInObjectTree>(c => _treeComponent = c, _ => _treeComponent = null);
            entity.Bind<IDrawableInfo>(c => _drawable = c, _ => _drawable = null);
        }

        public void RebuildTree()
        {
            var tree = Tree;
            _root = buildTree(_root, tree);
            refreshTree();
        }

        public void RefreshLayout()
        {
            refreshTree();
        }

        public void Expand(ITreeStringNode node)
        {
            var nodeView = findNodeView(node);
            if (nodeView != null) nodeView.Expand();
        }

        public void Collapse(ITreeStringNode node)
        {
			var nodeView = findNodeView(node);
            if (nodeView != null) nodeView.Collapse();
        }

        private Node findNodeView(ITreeStringNode node)
        {
            return findNodeView(_root, node);
        }

		private Node findNodeView(Node nodeView, ITreeStringNode node)
		{
            if (nodeView.Item == node) return nodeView;
            foreach (var child in nodeView.Children)
            {
                var result = findNodeView(child, node);
                if (result != null) return result;
            }
            return null;
		}

        private void refreshTree()
        {
            var root = _root;
            if (root != null)
            {
                root.ResetOffsets(0f, 0f, HorizontalSpacing, -VerticalSpacing);
                List<IObject> uiObjectsToAdd = new List<IObject>();
                processTree(root, uiObjectsToAdd);
                if (uiObjectsToAdd.Count > 0)
                {
                    _state.UI.AddRange(uiObjectsToAdd);
                }
            }
        }

        private void subscribeTree(ITreeStringNode node)
        {
            if (node == null) return;
            node.TreeNode.Children.OnListChanged.Subscribe(onNodeListChanged);
            foreach (var child in node.TreeNode.Children)
            {
                subscribeTree(child);
            }
        }

        private void unsubscribeTree(ITreeStringNode node)
        {
            if (node == null) return;
            node.TreeNode.Children.OnListChanged.Unsubscribe(onNodeListChanged);
            foreach (var child in node.TreeNode.Children)
            {
                unsubscribeTree(child);
            }
        }

        private void onNodeListChanged(AGSListChangedEventArgs<ITreeStringNode> args)
        {
            if (args.ChangeType == ListChangeType.Add)
            {
                foreach (var item in args.Items) subscribeTree(item.Item);
            }
            else
            {
                foreach (var item in args.Items) unsubscribeTree(item.Item);
            }
            RebuildTree();
        }

        private void clearTreeFromUi(Node node)
        {
            if (node == null) return;
            foreach (var child in node.Children)
            {
                clearTreeFromUi(child);
            }
            removeFromUI(node);
        }

        private void processTree(Node node, List<IObject> uiObjectsToAdd)
        {
            if (node == null) return;
            addToUI(node, uiObjectsToAdd);
            foreach (var child in node.Children)
            {
                processTree(child, uiObjectsToAdd);
            }
        }

        private Node buildTree(Node currentNode, ITreeStringNode actualNode)
        {
            if (actualNode == null) return null;

            if (currentNode == null || currentNode.Item != actualNode)
            {
                if (currentNode != null) removeFromUI(currentNode);
                var drawable = _drawable;
                currentNode = new Node(actualNode, NodeViewProvider.CreateNode(actualNode, 
                                           drawable == null ? AGSLayers.UI : drawable.RenderLayer), null, this);
                var treeComponent = _treeComponent;
                if (treeComponent != null)
                {
                    currentNode.View.ParentPanel.TreeNode.SetParent(treeComponent.TreeNode);
                }
            }
            int maxChildren = Math.Max(currentNode.Children.Count, actualNode.TreeNode.Children.Count);
            for (int i = 0; i < maxChildren; i++)
            {
                var nodeChild = childOrNull(currentNode.Children, i);
                var actualChild = childOrNull(actualNode.TreeNode.Children, i);
                if (nodeChild == null && actualChild == null) continue;
                if (nodeChild == null)
                {
                    var drawable = _drawable;
                    var newNode = new Node(actualChild, NodeViewProvider.CreateNode(actualChild, 
                                        drawable == null ? AGSLayers.UI : drawable.RenderLayer), currentNode, this);
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

        private void addToUI(Node node, List<IObject> uiObjectsToAdd)
        {
            NodeViewProvider.BeforeDisplayingNode(node.Item, node.View, 
                                                  node.IsCollapsed, node.IsHovered, node.IsSelected);
            if (!node.IsNew) return;
            node.IsNew = false;
            uiObjectsToAdd.Add(node.View.ParentPanel);
            uiObjectsToAdd.Add(node.View.TreeItem.TreeNode.Parent);
            uiObjectsToAdd.Add(node.View.TreeItem);
            if (node.View.ExpandButton != null)
            {
                uiObjectsToAdd.Add(node.View.ExpandButton);
            }
        }

        private void removeFromUI(Node node) 
        {
            node.View.ParentPanel.Visible = false;
            removeFromUI(node.View.ParentPanel);
            removeFromUI(node.View.ExpandButton);
            removeFromUI(node.View.TreeItem);
            removeFromUI(node.View.VerticalPanel);
            removeFromUI(node.View.HorizontalPanel);
            node.View.ParentPanel.TreeNode.SetParent(null);
            node.Dispose();
        }

        private void removeFromUI(IObject obj)
        {
            if (obj == null) return;
            _state.UI.Remove(obj);
        }

        private class Node : IDisposable
        {
            private ITreeViewComponent _tree;
            private bool _isCollapsed;

            public Node(ITreeStringNode item, ITreeNodeView view, Node parentNode, ITreeViewComponent tree)
            {
                _tree = tree;
                Item = item;
                View = view;
                Parent = parentNode;
                Children = new List<Node>();
                _isCollapsed = true;
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
            public bool IsCollapsed 
            { 
                get { return _isCollapsed; } 
                set
                {
                    if (_isCollapsed == value) return;
                    _isCollapsed = value;
                    if (value) _tree.OnNodeCollapsed.Invoke(new NodeEventArgs(Item));
                    else _tree.OnNodeExpanded.Invoke(new NodeEventArgs(Item));
                }
            }
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

            public void Expand()
            {
                IsCollapsed = false;
                refreshCollapseExpand();
            }

            public void Collapse()
            {
                IsCollapsed = true;
                refreshCollapseExpand();
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

            private void onMouseEnter(MousePositionEventArgs args)
            {
                IsHovered = true;
                _tree.NodeViewProvider.BeforeDisplayingNode(Item, View,
                                                  IsCollapsed, IsHovered, IsSelected);
            }

            private void onMouseLeave(MousePositionEventArgs args)
            {
                IsHovered = false;
                _tree.NodeViewProvider.BeforeDisplayingNode(Item, View,
                                                  IsCollapsed, IsHovered, IsSelected);
            }

            private void onMouseClicked(MouseButtonEventArgs args)
            {
                IsCollapsed = !IsCollapsed;
                refreshCollapseExpand();
            }

            private void refreshCollapseExpand()
            {
				foreach (var child in Children)
				{
					child.View.ParentPanel.Visible = !IsCollapsed;
				}
                _tree.RefreshLayout();
            }

            private void onItemSelected(MouseButtonEventArgs args)
            {
                getRoot().ResetSelection();
                if (_tree.AllowSelection == SelectionType.None) return;
                IsSelected = true;
                _tree.OnNodeSelected.Invoke(new NodeEventArgs(Item));
            }
        }
    }
}
