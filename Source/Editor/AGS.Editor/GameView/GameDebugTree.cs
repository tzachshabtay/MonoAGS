using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using AGS.API;
using AGS.Engine;
using Autofac;

namespace AGS.Editor
{
    public class GameDebugTree : IDebugTab
    {
        private readonly IRenderLayer _layer;
        private string _panelId;
        private ITreeViewComponent _treeView;
        private readonly AGSEditor _editor;
        private readonly IConcurrentHashSet<string> _addedObjects;
        private readonly List<RoomSubscriber> _roomSubscribers;
        private readonly InspectorPanel _inspector;
        private readonly ConcurrentDictionary<string, ITreeStringNode> _entitiesToNodes;
        private IPanel _treePanel, _scrollingPanel, _contentsPanel, _parent;
        private ITextBox _searchBox;

        private IEnabledComponent _lastSelectedEnabled;
        private IVisibleComponent _lastSelectedMaskVisible;
        private IImageComponent _lastSelectedMaskImage;
        private IEntity _lastSelectedEntity;
        private ITreeStringNode _lastSelectedNode;
        private ITreeStringNode _moreRoomsNode;
        private bool _lastMaskVisible;
        private byte _lastOpacity;
        private bool _lastEnabled;
        private bool _lastClickThrough;

        const float _padding = 42f;
        const float _gutterSize = 15f;

        const string _moreRoomsPrefix = "More Rooms";
        const string _roomPrefix = "Room: ";
        const string _objectsPrefix = "Objects";
        const string _areasPrefix = "Areas";
        const string _uiPrefix = "UI";

        public GameDebugTree(AGSEditor editor, IRenderLayer layer, InspectorPanel inspector)
        {
            _editor = editor;
            _inspector = inspector;
            _entitiesToNodes = new ConcurrentDictionary<string, ITreeStringNode>();
            _addedObjects = new AGSConcurrentHashSet<string>(100, false);
            _roomSubscribers = new List<RoomSubscriber>(20);
            _layer = layer;
        }

        public IPanel Panel => _scrollingPanel;

        public void Load(IPanel parent)
        {
            _parent = parent;
            _panelId = parent.TreeNode.GetRoot().ID;
            var factory = _editor.Editor.Factory;

            _searchBox = factory.UI.GetTextBox("GameDebugTreeSearchBox", 0f, parent.Height, parent, "Search...", width: parent.Width, height: 30f);
            _searchBox.RenderLayer = _layer;
            _searchBox.Border = AGSBorders.SolidColor(GameViewColors.Border, 2f);
            _searchBox.Tint = GameViewColors.Textbox;
            _searchBox.Pivot = new PointF(0f, 1f);
            _searchBox.GetComponent<ITextComponent>().PropertyChanged += onSearchPropertyChanged;

            _scrollingPanel = factory.UI.GetPanel("GameDebugTreeScrollingPanel", parent.Width - _gutterSize, parent.Height - _searchBox.Height - _gutterSize, 0f, 0f, parent);
            _scrollingPanel.RenderLayer = _layer;
            _scrollingPanel.Pivot = new PointF(0f, 0f);
            _scrollingPanel.Tint = Colors.Transparent;
            _scrollingPanel.Border = AGSBorders.SolidColor(GameViewColors.Border, 2f);
            _contentsPanel = factory.UI.CreateScrollingPanel(_scrollingPanel);
            _treePanel = factory.UI.GetPanel("GameDebugTreePanel", 1f, 1f, 0f, _contentsPanel.Height - _padding, _contentsPanel);
            _treePanel.Tint = Colors.Transparent;
            _treePanel.RenderLayer = _layer;
            _treePanel.Pivot = new PointF(0f, 1f);
            _treeView = _treePanel.AddComponent<ITreeViewComponent>();
            _treeView.OnNodeSelected.Subscribe(onTreeNodeSelected);
            parent.GetComponent<IScaleComponent>().PropertyChanged += (_, args) =>
            {
                if (args.PropertyName != nameof(IScaleComponent.Height)) return;
                _contentsPanel.BaseSize = new SizeF(_contentsPanel.Width, parent.Height - _searchBox.Height - _gutterSize);
                _treePanel.Y = _contentsPanel.Height - _padding;
                _searchBox.Y = _parent.Height;
            };
        }

        public Task Show()
        {
            refresh();
            _scrollingPanel.Visible = true;
            _searchBox.Visible = true;
            _editor.Game.State.UI.OnListChanged.Subscribe(onGuiChanged);
            _editor.Game.State.Rooms.OnListChanged.Subscribe(onRoomsChanged);
            _editor.Game.Events.OnRoomChanging.Subscribe(onRoomChanged);
            subscribeRooms(_editor.Game.State.Rooms);
            _treeView.Expand(_treeView.Tree);
            return Task.CompletedTask;
        }

        public void Hide()
        {
	        _scrollingPanel.Visible = false;
            _searchBox.Visible = false;
            _editor.Game.State.UI.OnListChanged.Unsubscribe(onGuiChanged);
            _editor.Game.State.Rooms.OnListChanged.Unsubscribe(onRoomsChanged);
            _editor.Game.Events.OnRoomChanging.Unsubscribe(onRoomChanged);
            unsubscribeRooms(_editor.Game.State.Rooms);
            _treeView.Tree = null;
        }

        public void Resize()
        {
            _contentsPanel.BaseSize = new SizeF(_parent.Width - _gutterSize, _contentsPanel.Height);
            _searchBox.LabelRenderSize = new SizeF(_parent.Width, _searchBox.Height);
            _searchBox.Watermark.LabelRenderSize = new SizeF(_parent.Width, _searchBox.Height);
        }

        public void Select(IEntity entity)
        {
            if (!_entitiesToNodes.TryGetValue(entity.ID, out var node)) return;
            var parent = node.TreeNode.Parent;
            while (parent != null)
            {
                _treeView.Expand(parent);
                parent = parent.TreeNode.Parent;
            }
            _treeView.Select(node);
        }

        public void Unselect()
        {
            if (_lastSelectedNode == null) return;
            _lastSelectedNode = null;
            _lastSelectedEntity?.GetComponent<EntityDesigner>()?.Dispose();
            _lastSelectedEntity?.RemoveComponent<EntityDesigner>();
            var lastSelectedMaskVisible = _lastSelectedMaskVisible;
            var lastSelectedMaskImage = _lastSelectedMaskImage;
            var lastEnabled = _lastEnabled;
            if (lastSelectedMaskVisible != null) lastSelectedMaskVisible.Visible = _lastMaskVisible;
            if (lastSelectedMaskImage != null) lastSelectedMaskImage.Opacity = _lastOpacity;
            if (_lastSelectedEnabled != null)
            {
                _lastSelectedEnabled.Enabled = lastEnabled;
                _lastSelectedEnabled.ClickThrough = _lastClickThrough;
            }
            _lastSelectedEnabled = null;
            _lastMaskVisible = false;
            _lastOpacity = 0;
        }

        private void onRoomChanged()
        {
            var children = _treeView.Tree.TreeNode.Children;
            var prevRoom = children.FirstOrDefault(c => c.Text.StartsWith(_roomPrefix, StringComparison.InvariantCulture));
            var room = _editor.Game.State.Room?.ID;
            var newRoom = room == null ? null : _moreRoomsNode.TreeNode.Children.FirstOrDefault(c => c.Text.EndsWith(room, StringComparison.InvariantCulture));
            var expander = new NodesExpander(_treeView);
            prevRoom?.TreeNode.SetParent(_moreRoomsNode.TreeNode);
            newRoom?.TreeNode.SetParent(_treeView.Tree.TreeNode);

            //removing and re-adding "more rooms" so that it would be after the current room in the tree view
            _moreRoomsNode.TreeNode.SetParent(null);
            _moreRoomsNode.TreeNode.SetParent(_treeView.Tree.TreeNode);

            expander.Expand();
        }

        private void subscribeRooms(IEnumerable<IRoom> rooms)
        {
            foreach (var room in rooms)
            {
                var subscriber = new RoomSubscriber(room, _treeView, findRoom, addObjectToTree, addAreaToTree, removeFromTree);
                _roomSubscribers.Add(subscriber);
            }
        }

        private void unsubscribeRooms(IEnumerable<IRoom> rooms)
        {
            foreach (var room in rooms)
            {
                var subscriber = _roomSubscribers.FirstOrDefault(c => c.Room == room);
                if (subscriber != null)
                {
                    subscriber.Unsubscribe();
                    _roomSubscribers.Remove(subscriber);
                }
            }
        }

        private ITreeStringNode findRoom(IRoom room)
        {
            return _moreRoomsNode.TreeNode.Children.FirstOrDefault(c => c.Text.EndsWith(room.ID, StringComparison.InvariantCulture))
                ?? _treeView.Tree.TreeNode.Children.FirstOrDefault(c => c.Text.EndsWith(room.ID, StringComparison.InvariantCulture));
        }

        private void onRoomsChanged(AGSListChangedEventArgs<IRoom> args)
        {
            var expander = new NodesExpander(_treeView);
            if (args.ChangeType == ListChangeType.Add)
            {
                subscribeRooms(args.Items.Select(i => i.Item));
                foreach (var room in args.Items)
                {
                    addRoomToTree(room.Item, _moreRoomsNode);
                }
            }
            else
            {
                unsubscribeRooms(args.Items.Select(i => i.Item));
                foreach (var room in args.Items)
                {
                    var roomNode = findRoom(room.Item);
                    roomNode?.TreeNode.SetParent(null);
                }
            }
            expander.Expand();
        }

        private void onGuiChanged(AGSHashSetChangedEventArgs<IObject> args)
        {
            var uiNode = _treeView.Tree.TreeNode.Children.First(c => c.Text == _uiPrefix);
            var expander = new NodesExpander(_treeView);
            if (args.ChangeType == ListChangeType.Add)
            {
                foreach (var item in args.Items)
                {
                    addObjectToTree(item, uiNode);
                }
            }
            else
            {
                foreach (var item in args.Items)
                {
                    removeFromTree(item.ID, uiNode);
                }
            }
            expander.Expand();
        }

        private void onSearchPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(ITextComponent.Text)) return;
            _treeView.SearchFilter = _searchBox.Text;
        }

        private void onTreeNodeSelected(NodeEventArgs args)
        {
            if (args.Node == _lastSelectedNode) return;
            Unselect();
            _lastSelectedNode = args.Node;
            string nodeType = args.Node.Properties.Strings.GetValue(Fields.Type);
            if (nodeType == null) return;
            switch (nodeType)
            {
                case NodeType.Object:
                    selectObject(args.Node);
                    break;
                case NodeType.Area:
                    selectArea(args.Node);
                    break;
            }
        }

        private void selectObject(ITreeStringNode node)
        {
            var obj = node.Properties.Entities.GetValue(Fields.Entity);
            _inspector.Inspector.Show(obj);
            _lastSelectedEntity = obj;
            var host = new AGSComponentHost(_editor.EditorResolver);
            host.Init(obj);
            TypedParameter uiEventsAggParam = new TypedParameter(typeof(UIEventsAggregator), _editor.UIEventsAggregator);
            var uiEvents = _editor.EditorResolver.Container.Resolve<EditorUIEvents>(uiEventsAggParam);
            obj.AddComponent<EditorUIEvents>(uiEvents);
            host.AddComponent<EntityDesigner>();

            var visibleComponent = obj.GetComponent<IVisibleComponent>();
            var image = obj.GetComponent<IImageComponent>();
            var borderComponent = obj.GetComponent<IBorderComponent>();
            var enabledComponent = obj.GetComponent<IEnabledComponent>();
            if (enabledComponent != null)
            {
                _lastSelectedEnabled = enabledComponent;
                _lastEnabled = enabledComponent.Enabled;
                _lastClickThrough = enabledComponent.ClickThrough;
                enabledComponent.Enabled = true;
                enabledComponent.ClickThrough = false;
            }
            if (image != null)
            {
                if (image.Opacity == 0)
                {
                    _lastOpacity = image.Opacity;
                    _lastSelectedMaskImage = image;
                    image.Opacity = 100;
                }
            }
            if (visibleComponent != null)
            {
                _lastMaskVisible = visibleComponent.Visible;
                _lastSelectedMaskVisible = visibleComponent;
                visibleComponent.Visible = true;
            }
        }

        private void selectArea(ITreeStringNode node)
        {
            var obj = node.Properties.Entities.GetValue(Fields.Entity);
            _inspector.Inspector.Show(obj);
            var area = obj.GetComponent<IAreaComponent>();
            var debugMask = area.Mask.DebugDraw;
            if (debugMask != null)
            {
                _lastMaskVisible = debugMask.Visible;
                _lastSelectedMaskVisible = debugMask;
                debugMask.Visible = true;
            }
        }

        private void refresh()
        {
            _addedObjects.Clear();
            var root = addToTree("Game", null);
            var ui = addToTree(_uiPrefix, root);

            var state = _editor.Game.State;
            foreach (var obj in state.UI)
            {
                addObjectToTree(getRoot(obj), ui);
            }
            addRoomToTree(state.Room, root);

            _moreRoomsNode = addToTree(_moreRoomsPrefix, root);
            foreach (var room in state.Rooms)
            {
                if (room == state.Room) continue;
                addRoomToTree(room, _moreRoomsNode);
            }

            _treeView.Tree = root;
        }

        private void addRoomToTree(IRoom room, ITreeStringNode parent)
        {
            if (room == null) return;
            var roomNode = addToTree($"{_roomPrefix}{room.ID}", parent);
            var objects = addToTree(_objectsPrefix, roomNode);
            var areas = addToTree(_areasPrefix, roomNode);

            addObjectToTree(getRoot(room.Background), objects);
            foreach (var obj in room.Objects)
            {
                addObjectToTree(getRoot(obj), objects);
            }
            foreach (var area in room.Areas)
            {
                addAreaToTree(area, areas);
            }
        }

        private void addAreaToTree(IArea area, ITreeStringNode parent)
        {
            var node = addToTree(area.ID, parent);
            node.Properties.Strings.SetValue(Fields.Type, NodeType.Area);
            node.Properties.Entities.SetValue(Fields.Entity, area);
            _entitiesToNodes[area.ID] = node;
        }

        private IObject getRoot(IObject obj)
        {
            if (obj == null) return null;
            var root = obj.TreeNode.GetRoot();
            if (root.ID == _panelId) return null;
            return root;
        }

        private void addObjectToTree(IObject obj, ITreeStringNode parent)
        {
            if (obj == null || !_addedObjects.Add(obj.ID)) return;
            var node = addToTree(obj.ID, parent);
            node.Properties.Strings.SetValue(Fields.Type, NodeType.Object);
            node.Properties.Entities.SetValue(Fields.Entity, obj);
            _entitiesToNodes[obj.ID] = node;
            foreach (var child in obj.TreeNode.Children)
            {
                addObjectToTree(child, node);
            }
        }

        private ITreeStringNode addToTree(string text, ITreeStringNode parent)
        {
            var node = new AGSTreeStringNode(text, _editor.Editor.Settings.Defaults.TextFont);
            if (parent != null) node.TreeNode.SetParent(parent.TreeNode);
            return node;
        }

        private void removeFromTree(string id, ITreeStringNode parent)
        {
            _addedObjects.Remove(id);
            var node = parent.TreeNode.Children.FirstOrDefault(c => c.Text == id);
            node?.TreeNode.SetParent(null);
        }

        private static class Fields
        {
            public const string Type = "Type";
            public const string Entity = "Entity";
        }

        private static class NodeType
        {
            public const string Object = "Object";
            public const string Area = "Area";
        }

        private class NodesExpander
        {
            private readonly List<ITreeStringNode> _shouldExpand;
            private readonly ITreeViewComponent _treeView;

            public NodesExpander(ITreeViewComponent treeView)
            {
                _treeView = treeView;
                _shouldExpand = new List<ITreeStringNode>();
                treeView.Tree.TreeNode.RunOnTree(0, (node, _) => { if (!(treeView.IsCollapsed(node) ?? true)) _shouldExpand.Add(node); });
            }

            public void Expand()
            {
                foreach (var node in _shouldExpand)
                {
                    _treeView.Expand(node);
                }
            }
        }

        private class RoomSubscriber
        {
            private readonly Func<IRoom, ITreeStringNode> _findRoom;
            private readonly Action<IObject, ITreeStringNode> _addObjToTree;
            private readonly Action<IArea, ITreeStringNode> _addAreaToTree;
            private readonly Action<string, ITreeStringNode> _removeFromTree;
            private readonly ITreeViewComponent _treeView;

            public RoomSubscriber(IRoom room, ITreeViewComponent treeView, Func<IRoom, ITreeStringNode> findRoom, 
                                  Action<IObject, ITreeStringNode> addObjToTree, Action<IArea, ITreeStringNode> addAreaToTree,
                                  Action<string, ITreeStringNode> removeFromTree)
            {
                Room = room;
                _treeView = treeView;
                _findRoom = findRoom;
                _addObjToTree = addObjToTree;
                _removeFromTree = removeFromTree;
                _addAreaToTree = addAreaToTree;
                room.Areas.OnListChanged.Subscribe(onRoomAreasChanged);
                room.Objects.OnListChanged.Subscribe(onRoomObjectsChanged);
            }

            public IRoom Room { get; }

            public void Unsubscribe()
            {
                Room.Areas.OnListChanged.Unsubscribe(onRoomAreasChanged);
                Room.Objects.OnListChanged.Unsubscribe(onRoomObjectsChanged);
            }

            private void onRoomObjectsChanged(AGSHashSetChangedEventArgs<IObject> args)
            {
                var roomNode = _findRoom(Room);
                var expander = new NodesExpander(_treeView);
                var objects = roomNode.TreeNode.Children.First(c => c.Text == _objectsPrefix);
                if (args.ChangeType == ListChangeType.Add)
                {
                    foreach (var obj in args.Items)
                    {
                        _addObjToTree(obj, objects);
                    }
                }
                else
                {
                    foreach (var obj in args.Items)
                    {
                        _removeFromTree(obj.ID, objects);
                    }
                }
                expander.Expand();
            }

            private void onRoomAreasChanged(AGSListChangedEventArgs<IArea> args)
            {
                var roomNode = _findRoom(Room);
                var expander = new NodesExpander(_treeView);
                var areas = roomNode.TreeNode.Children.First(c => c.Text == _areasPrefix);
                if (args.ChangeType == ListChangeType.Add)
                {
                    foreach (var area in args.Items)
                    {
                        _addAreaToTree(area.Item, areas);
                    }
                }
                else
                {
                    foreach (var area in args.Items)
                    {
                        _removeFromTree(area.Item.ID, areas);
                    }
                }
                expander.Expand();
            }
        }
    }
}