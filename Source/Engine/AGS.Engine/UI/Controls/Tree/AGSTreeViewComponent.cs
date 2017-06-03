using System;
using System.Collections.Generic;
using AGS.API;

namespace AGS.Engine
{
    public class AGSTreeViewComponent : AGSComponent, ITreeViewComponent
    {
        private Node _root;
        private IGameState _state;
        private IInObjectTree _entity;
        private bool _duringUpdate;
        
        public AGSTreeViewComponent(ITreeNodeViewProvider provider, IGameEvents gameEvents, IGameState state)
        {
            HorizontalSpacing = 5f;
            VerticalSpacing = 25f;
            _state = state;
            NodeViewProvider = provider;
            gameEvents.OnRepeatedlyExecute.Subscribe(onRepeatedlyExecute);
        }

        public ITreeStringNode Tree { get; set; }

        public ITreeNodeViewProvider NodeViewProvider { get; set; }

        public float HorizontalSpacing { get; set; }

        public float VerticalSpacing { get; set; }

        public override void Init(IEntity entity)
        {
            base.Init(entity);
            _entity = entity.GetComponent<IInObjectTree>();
        }

        private void onRepeatedlyExecute(object sender, AGSEventArgs args)
        {
            if (_duringUpdate) return;
            _duringUpdate = true;
            var tree = Tree;
            _root = buildTree(_root, tree);
            processTree(_root);
            var root = _root;
            if (root != null) root.ResetOffsets(0f, 0f, HorizontalSpacing, -VerticalSpacing);
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
                currentNode = new Node(actualNode, NodeViewProvider.CreateNode(actualNode), null);
                currentNode.View.ParentPanel.TreeNode.SetParent(_entity.TreeNode);
            }
            int maxChildren = Math.Max(currentNode.Children.Count, actualNode.TreeNode.Children.Count);
            for (int i = 0; i < maxChildren; i++)
            {
                var nodeChild = childOrNull(currentNode.Children, i);
                var actualChild = childOrNull(actualNode.TreeNode.Children, i);
                if (nodeChild == null && actualChild == null) continue;
                if (nodeChild == null)
                {
                    var newNode = new Node(actualChild, NodeViewProvider.CreateNode(actualChild), currentNode);
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
            NodeViewProvider.BeforeDisplayingNode(node.Item, node.View, node.IsCollapsed, node.IsHovered);
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
            public Node(ITreeStringNode item, ITreeNodeView view, Node parentNode)
            {
                Item = item;
                View = view;
                Children = new List<Node>();
                IsCollapsed = true;
                IsNew = true;

                /*var layout = view.ParentPanel.AddComponent<IStackLayoutComponent>();
                if (layout != null) //Adds default vertical layout for the children if the view provider didn't already add a stack layout
                {
                    layout.RelativeSpacing = new PointF(0f, -1f);
                }*/
                if (parentNode != null)
                {
                    view.ParentPanel.TreeNode.SetParent(parentNode.View.VerticalPanel.TreeNode);
                    view.ParentPanel.Visible = !parentNode.IsCollapsed;
                }

                view.TreeItem.MouseEnter.Subscribe(onMouseEnter);
                view.TreeItem.MouseLeave.Subscribe(onMouseLeave);
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

            public bool IsNew { get; set; }
            public bool IsCollapsed { get; set; }
            public bool IsHovered { get; set; }
            public float XOffset { get; private set; }

            public void Dispose()
            {
                View.TreeItem.MouseEnter.Unsubscribe(onMouseEnter);
                View.TreeItem.MouseLeave.Unsubscribe(onMouseLeave);
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
        }
    }
}
