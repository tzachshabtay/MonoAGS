using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using AGS.API;

namespace AGS.Engine
{
    public class AGSTreeViewComponent : AGSComponent, ITreeViewComponent
    {
        private Node _root;
        private IGameState _state;
        private IInObjectTreeComponent _treeComponent;
        private ITreeStringNode _tree;
        private IDrawableInfoComponent _drawable;
        private string _searchFilter;
        private TaskCompletionSource<object> _currentSearchToken;
        private bool _skipRenderingRoot;

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
            get => _tree;
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

        public string SearchFilter 
        {
            get => _searchFilter;
            set 
            { 
                value = value?.ToLowerInvariant() ?? "";
                _searchFilter = value;
                applySearchOnIdle();
            }
        }

        public bool SkipRenderingRoot
        {
            get => _skipRenderingRoot;
            set 
            {
                _skipRenderingRoot = value;
                RefreshLayout();
            }
        }

        public IBlockingEvent<NodeEventArgs> OnNodeSelected { get; }

        public IBlockingEvent<NodeEventArgs> OnNodeExpanded { get; }

        public IBlockingEvent<NodeEventArgs> OnNodeCollapsed { get; }

        public override void Init(IEntity entity)
        {
            base.Init(entity);
            entity.Bind<IInObjectTreeComponent>(c => _treeComponent = c, _ => _treeComponent = null);
            entity.Bind<IDrawableInfoComponent>(c => _drawable = c, _ => _drawable = null);
        }

        public override void Dispose()
        {
            base.Dispose();
            var tree = Tree;
            if (tree != null) unsubscribeTree(tree);
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
            nodeView?.Expand();
        }

        public void Collapse(ITreeStringNode node)
        {
			var nodeView = findNodeView(node);
            nodeView?.Collapse();
        }

        public bool? IsCollapsed(ITreeStringNode node)
        {
            var nodeView = findNodeView(node);
            if (nodeView == null) return null;
            return nodeView.IsCollapsed;
        }

        public void Select(ITreeStringNode node)
        {
            var nodeView = findNodeView(node);
            nodeView?.Select();
        }

        private async void applySearchOnIdle()
        {
            _currentSearchToken?.TrySetResult(null);
            var token = new TaskCompletionSource<object>();
            _currentSearchToken = token;
            var timeout = Task.Delay(800);
            var task = await Task.WhenAny(token.Task, timeout);
            if (task == timeout) //800 milliseconds has passed and we haven't got a new search text, user must have finished typing, let's do the performance killing stuff
            {
                _root?.UpdateSearchFilter(SearchFilter);
            }
        }

        private Node findNodeView(ITreeStringNode node) => findNodeView(_root, node);

        private Node findNodeView(Node nodeView, ITreeStringNode node)
		{
            try
            {
                if (nodeView.Item == node)
                    return nodeView;
                foreach (var child in nodeView.Children)
                {
                    var result = findNodeView(child, node);
                    if (result != null)
                        return result;
                }
                return null;
            }
            catch
            {
                return null;
            }
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
                currentNode = new Node(actualNode, () => _drawable, () => _treeComponent, null, this);
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
                    var newNode = new Node(actualChild, () => _drawable, () => null, currentNode, this);
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
            if (list.Count <= i) return default;
            return list[i];
        }

        private void addToUI(Node node, List<IObject> uiObjectsToAdd)
        {
            node.RefreshDisplay();
            if (!node.IsNew) return;
            var view = node.View;
            if (view == null) return;
            node.IsNew = false;
            uiObjectsToAdd.Add(view.ParentPanel);
            uiObjectsToAdd.Add(view.TreeItem.TreeNode.Parent);
            uiObjectsToAdd.Add(view.TreeItem);
            if (view.ExpandButton != null)
            {
                uiObjectsToAdd.Add(view.ExpandButton);
            }
        }

        private void removeFromUI(Node node) 
        {
            var view = node.View;
            if (view != null)
            {
                view.ParentPanel.Visible = false;
                removeFromUI(view.ParentPanel);
                removeFromUI(view.ExpandButton);
                removeFromUI(view.TreeItem);
                removeFromUI(view.VerticalPanel);
                removeFromUI(view.HorizontalPanel);
                view.ParentPanel.TreeNode.SetParent(null);
            }
            node.Dispose();
        }

        private void removeFromUI(IObject obj)
        {
            obj.DestroyWithChildren(_state);
        }

        private class Node : IDisposable
        {
            private ITreeViewComponent _tree;
            private bool _isCollapsed, _isExpandButtonHovered, _isNodeHovered;
            private Func<IDrawableInfoComponent> _drawable;
            private Func<IInObjectTreeComponent> _treeObj;

            public enum SearchFilterMode
            {
                Visible,
                VisibleBecauseOfChild,
                VisibleBecauseOfParent,
                NotVisible
            }

            public Node(ITreeStringNode item, Func<IDrawableInfoComponent> drawable, Func<IInObjectTreeComponent> treeObj, Node parentNode, ITreeViewComponent tree)
            {
                _tree = tree;
                _drawable = drawable;
                _treeObj = treeObj;
                Item = item;
                Parent = parentNode;
                Children = new List<Node>();
                _isCollapsed = true;
                IsNew = true;
                if (parentNode == null)
                {
                    initView();
                }
            }

            public ITreeStringNode Item { get; }
            public ITreeNodeView View { get; private set; }
            public List<Node> Children { get; }
            public Node Parent { get; }

            public bool IsNew { get; set; }
            public bool IsCollapsed
            {
                get => _isCollapsed;
                set
                {
                    _isCollapsed = value;
                    if (value) _tree.OnNodeCollapsed.Invoke(new NodeEventArgs(Item));
                    else
                    {
                        foreach (var child in Children)
                        {
                            child.initView();
                        }
                        _tree.OnNodeExpanded.Invoke(new NodeEventArgs(Item));
                    }
                }
            }
            public bool IsHovered { get => _isNodeHovered || _isExpandButtonHovered; }
            public bool IsSelected { get; private set; }
            public float XOffset { get; private set; }
            public SearchFilterMode FilterMode { get; private set; }

            public void Dispose()
            {
                var view = View;
                if (view == null) return;
                view.TreeItem.MouseEnter.Unsubscribe(onMouseEnterNode);
                view.TreeItem.MouseLeave.Unsubscribe(onMouseLeaveNode);
                view.TreeItem.MouseClicked.Unsubscribe(onItemSelected);
                var expandButton = view.ExpandButton;
                if (expandButton != null)
                {
                    expandButton.MouseEnter.Unsubscribe(onMouseEnterExpandButton);
                    expandButton.MouseLeave.Unsubscribe(onMouseLeaveExpandButton);
                }
                var button = getExpandButton();
                button?.MouseClicked.Unsubscribe(onMouseClicked);
            }

            public void UpdateSearchFilter(string filter)
            {
                UpdateSearchFilter(filter, SearchFilterMode.NotVisible);
                if (!string.IsNullOrEmpty(filter))
                {
                    setCollapsedAfterSearch();
                }
                updateVisibilityWithChildren();
                _tree.RefreshLayout();
            }

            public SearchFilterMode UpdateSearchFilter(string filter, SearchFilterMode parentMode)
            {
                var filterMode = SearchFilterMode.NotVisible;
                if (passesFilter(filter))
                {
                    filterMode = SearchFilterMode.Visible;
                }
                else if (parentMode == SearchFilterMode.VisibleBecauseOfParent || parentMode == SearchFilterMode.Visible)
                {
                    filterMode = SearchFilterMode.VisibleBecauseOfParent;
                }
                foreach (var child in Children)
                {
                    var childMode = child.UpdateSearchFilter(filter, filterMode);
                    if (childMode != SearchFilterMode.NotVisible && filterMode != SearchFilterMode.Visible)
                    {
                        filterMode = SearchFilterMode.VisibleBecauseOfChild;
                    }
                }
                FilterMode = filterMode;
                return FilterMode;
            }

            private void setCollapsedAfterSearch()
            {
                IsCollapsed = FilterMode != SearchFilterMode.VisibleBecauseOfChild;
                foreach (var child in Children)
                {
                    child.setCollapsedAfterSearch();
                }
            }

            public float ResetOffsets(float xOffset, float yOffset, float spacingX, float spacingY)
            {
                bool skipRoot = _tree.SkipRenderingRoot && Parent == null;
                xOffset += skipRoot ? 0f : spacingX;
                var view = View;
                if (view != null)
                {
                    view.VerticalPanel.X = xOffset;
                    view.ParentPanel.Y = yOffset;
                }
                var childYOffset = skipRoot ? 0f : spacingY;
                foreach (var child in Children)
                {
                    childYOffset = child.ResetOffsets(xOffset, childYOffset, spacingX, spacingY);
                }

                if (view == null || !view.ParentPanel.Visible) return yOffset;
                return yOffset + childYOffset;
            }

            public void ResetSelection()
            {
                if (IsSelected)
                {
                    IsSelected = false;
                    RefreshDisplay();
                }
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

            public void RefreshDisplay()
            {
                var view = View;
                if (view == null) return;
                _tree.NodeViewProvider.BeforeDisplayingNode(Item, view,
                                                  IsCollapsed, IsHovered, IsSelected);
            }

            public void Select()
            {
                getRoot().ResetSelection();
                if (_tree.AllowSelection == SelectionType.None) return;
                IsSelected = true;
                _tree.OnNodeSelected.Invoke(new NodeEventArgs(Item));
                RefreshDisplay();
            }

            private void updateVisibilityWithChildren()
            {
                updateVisibility();
                foreach (var child in Children)
                {
                    child.updateVisibilityWithChildren();
                }
            }

            private bool passesFilter(string filter)
            {
                if (string.IsNullOrEmpty(filter)) return true;
                var customSearch = Item as ICustomSearchItem;
                if (customSearch != null) return customSearch.Contains(filter);
                string text = Item?.Text?.ToLowerInvariant() ?? "";
                return (text.Contains(filter));
            }

            private void initView()
            {
                if (View != null) return;
                var drawable = _drawable();
                var view = _tree.NodeViewProvider.CreateNode(Item,
                                     drawable == null ? AGSLayers.UI : drawable.RenderLayer);
                var parentNode = Parent;
                if (parentNode != null)
                {
                    parentNode.initView();
                    view.ParentPanel.TreeNode.SetParent(parentNode.View.VerticalPanel.TreeNode);
                    view.ParentPanel.Visible = !parentNode.IsCollapsed;
                }

                view.TreeItem.MouseEnter.Subscribe(onMouseEnterNode);
                view.TreeItem.MouseLeave.Subscribe(onMouseLeaveNode);
                view.TreeItem.MouseClicked.Subscribe(onItemSelected);
                var expandButton = view.ExpandButton;
                if (expandButton != null)
                {
                    expandButton.MouseEnter.Subscribe(onMouseEnterExpandButton);
                    expandButton.MouseLeave.Subscribe(onMouseLeaveExpandButton);
                }
                View = view;
                getExpandButton().MouseClicked.Subscribe(onMouseClicked);

                var treeObj = _treeObj();
                if (treeObj != null)
                {
                    view.ParentPanel.TreeNode.SetParent(treeObj.TreeNode);
                }
                updateVisibility();
            }

            private Node getRoot()
            {
                var root = this;
                while (root.Parent != null) root = root.Parent;
                return root;
            }

            private IUIEvents getExpandButton()
            {
                var view = View;
                if (view == null) return null;
                return (IUIEvents)view.ExpandButton ?? view.TreeItem;
            }

            private void onMouseEnterExpandButton(MousePositionEventArgs args)
            {
                _isExpandButtonHovered = true;
                RefreshDisplay();
            }

            private void onMouseLeaveExpandButton(MousePositionEventArgs args)
            {
                _isExpandButtonHovered = false;
                _tree.NodeViewProvider.BeforeDisplayingNode(Item, View,
                                                  IsCollapsed, IsHovered, IsSelected);
            }

            private void onMouseEnterNode(MousePositionEventArgs args)
            {
                _isNodeHovered = true;
                RefreshDisplay();
            }

            private void onMouseLeaveNode(MousePositionEventArgs args)
            {
                _isNodeHovered = false;
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
                    child.updateVisibility();
				}
                _tree.RefreshLayout();
            }

            private void updateVisibility()
            {
                var view = View;
                if (view == null) return;
                if (_tree.SkipRenderingRoot && Parent == null)
                {
                    view.HorizontalPanel.Visible = false;
                    return;
                }
                view.ParentPanel.Visible = !(Parent?.IsCollapsed ?? false) && FilterMode != SearchFilterMode.NotVisible;
            }

            private void onItemSelected(MouseButtonEventArgs args)
            {
                Select();
            }
        }
    }
}
