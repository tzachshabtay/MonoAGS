﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using AGS.API;
using AGS.Engine;

namespace AGS.Editor
{
    public class InspectorTreeNodeProvider : ITreeNodeViewProvider
    {
        private ITreeNodeViewProvider _provider;
        private float _rowWidth;
        private readonly Dictionary<string, ITreeTableLayout> _layouts;
        private readonly IGameEvents _gameEvents;
        private readonly IBlockingEvent<float> _onResize;
        private readonly IObject _inspectorPanel;
        private readonly Dictionary<ITreeNodeView, ResizeSubscriber> _resizeSubscribers;

        private static int _nextNodeId;

        public const string EDITOR_PREFIX = "InspectorEditor_";

        public InspectorTreeNodeProvider(ITreeNodeViewProvider provider, IGameEvents gameEvents, IObject inspectorPanel)
        {
            _inspectorPanel = inspectorPanel;
            _onResize = new AGSEvent<float>();
            _provider = provider;
            _gameEvents = gameEvents;
            _layouts = new Dictionary<string, ITreeTableLayout>();
            _resizeSubscribers = new Dictionary<ITreeNodeView, ResizeSubscriber>();
        }

        public void BeforeDisplayingNode(ITreeStringNode item, ITreeNodeView nodeView, bool isCollapsed, bool isHovered, bool isSelected)
        {
            _provider.BeforeDisplayingNode(item, nodeView, isCollapsed, isHovered, isSelected);
            var parent = item.TreeNode.Parent;
            if (parent == null) return;
            if (item is IInspectorTreeNode node && !node.IsCategory) return;
            displayCategoryRow(nodeView, isSelected);
        }

        public void Resize(float width)
        {
            _rowWidth = width;
            _onResize.Invoke(width);
        }

        public ITreeNodeView CreateNode(ITreeStringNode item, IRenderLayer layer, IObject parentObj)
        {
            var view = _provider.CreateNode(item, layer, parentObj);
            var parent = item.TreeNode.Parent;
            var node = item as IInspectorTreeNode;
            if (parent != null && (node == null || node.IsCategory))
            {
                setupCategoryRow(view);
            }
            if (parent != null && node != null)
            {
                var layoutId = parent.Properties.Strings.GetValue("LayoutID", Guid.NewGuid().ToString());
                var tableLayout = _layouts.GetOrAdd(layoutId, () => new TreeTableLayout(_gameEvents) { ColumnPadding = 20f });
                view.ParentPanel.OnDisposed(() => { tableLayout.Dispose(); _layouts.Remove(layoutId); });
                view.HorizontalPanel.RemoveComponent<IStackLayoutComponent>();
                var rowLayout = view.HorizontalPanel.AddComponent<ITreeTableRowLayoutComponent>();
                rowLayout.FixedWidthOverrides[0] = 15f; //fixed width for the expand button, as the column padding is too much for it
                rowLayout.Table = tableLayout;
            }
            if (node == null) return view;

			int nodeId = Interlocked.Increment(ref _nextNodeId);
			var itemTextId = (item.Text ?? "") + "_" + nodeId;
            node.Editor.AddEditorUI($"{EDITOR_PREFIX}{itemTextId}", view, node.Property);

            if (node.Property.Object is INotifyPropertyChanged propertyChanged)
            {
                PropertyChangedEventHandler onPropertyChanged = (sender, e) =>
                {
                    if (e.PropertyName != node.Property.Name) return;
                    IInspectorTreeNode inspectorNode = node;
                    do
                    {
                        inspectorNode.Property.Refresh();
                        inspectorNode.Editor.RefreshUI();
                        inspectorNode = inspectorNode.TreeNode.Parent as IInspectorTreeNode;
                    } while (inspectorNode != null);
                };
                propertyChanged.PropertyChanged += onPropertyChanged;
                view.ParentPanel.OnDisposed(() => propertyChanged.PropertyChanged -= onPropertyChanged);
            }
            else if (node.Property is INotifyPropertyChanged propertyValueChanged)
            {
                PropertyChangedEventHandler onPropertyChanged = (sender, e) =>
                {
                    if (e.PropertyName != nameof(IProperty.Value)) return;
                    node.Property.Refresh();
                    node.Editor.RefreshUI();
                };
                propertyValueChanged.PropertyChanged += onPropertyChanged;
                view.ParentPanel.OnDisposed(() => propertyValueChanged.PropertyChanged -= onPropertyChanged);
            }

            return view;
        }

        private void setupCategoryRow(ITreeNodeView view)
        {
            var inspectorJump = _inspectorPanel.AddComponent<IJumpOffsetComponent>();
            var rowJump = view.HorizontalPanel.AddComponent<IJumpOffsetComponent>();
            PropertyChangedEventHandler onPropertyChanged = (sender, args) =>
            {
                if (args.PropertyName != nameof(IJumpOffsetComponent.JumpOffset)) return;
                rowJump.JumpOffset = new PointF(-inspectorJump.JumpOffset.X, 0f);
            };
            inspectorJump.PropertyChanged += onPropertyChanged;
            view.ParentPanel.OnDisposed(() =>
            {
                inspectorJump.PropertyChanged -= onPropertyChanged;
                if (_resizeSubscribers.TryGetValue(view, out var subscriber))
                {
                    subscriber.Unsubscribe(_onResize);
                    _resizeSubscribers.Remove(view);
                }
            });
        }

        private void displayCategoryRow(ITreeNodeView nodeView, bool isSelected)
        {
            nodeView.TreeItem.Tint = Colors.Transparent;
            nodeView.HorizontalPanel.Tint = isSelected ? Colors.DarkSlateBlue : Colors.Gray.WithAlpha(50);
            var subscriber = _resizeSubscribers.GetOrAdd(nodeView, view => new ResizeSubscriber(view));
            subscriber.Subscribe(_onResize);
            subscriber.Resize(_rowWidth);
        }

        private class ResizeSubscriber
        {
            private ITreeNodeView _nodeView;
            private readonly Action<float> _resizeCallback;

            public ResizeSubscriber(ITreeNodeView nodeView)
            {
                _nodeView = nodeView;
                _resizeCallback = Resize;
            }

            public void Subscribe(IBlockingEvent<float> resizeEvent)
            {
                resizeEvent.Unsubscribe(_resizeCallback);
                resizeEvent.Subscribe(_resizeCallback);
            }

            public void Unsubscribe(IBlockingEvent<float> resizeEvent)
            {
                resizeEvent.Unsubscribe(_resizeCallback);
            }

            public void Resize(float rowWidth)
            {
                _nodeView.HorizontalPanel.BaseSize = new SizeF(rowWidth, 30f);
            }
        }
    }
}
