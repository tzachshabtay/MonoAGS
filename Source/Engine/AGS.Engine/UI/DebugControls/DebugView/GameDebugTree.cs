using System;
using System.ComponentModel;
using System.Threading.Tasks;
using AGS.API;

namespace AGS.Engine
{
    public class GameDebugTree : IDebugTab
    {
        private readonly IRenderLayer _layer;
        private string _panelId;
        private ITreeViewComponent _treeView;
        private readonly IGame _game;
        private readonly IConcurrentHashSet<string> _addedObjects;
        private readonly InspectorPanel _inspector;
        private IPanel _treePanel, _scrollingPanel, _parent;
        private ITextBox _searchBox;

        private IAnimationComponent _lastSelectedObject;
        private IVisibleComponent _lastSelectedMaskVisible;
        private IImageComponent _lastSelectedMaskImage;
        private IBorderStyle _lastObjectBorder;
        private bool _lastMaskVisible;
        private byte _lastOpacity;

        public GameDebugTree(IGame game, IRenderLayer layer, InspectorPanel inspector)
        {
            _game = game;
            _inspector = inspector;
            _addedObjects = new AGSConcurrentHashSet<string>(100, false);
            _layer = layer;
        }

        public IPanel Panel => _scrollingPanel;

        public void Load(IPanel parent)
        {
            _parent = parent;
            _panelId = parent.TreeNode.GetRoot().ID;
            var factory = _game.Factory;

            _searchBox = factory.UI.GetTextBox("GameDebugTreeSearchBox", 0f, parent.Height, parent, "Search...", width: parent.Width, height: 30f);
            _searchBox.RenderLayer = _layer;
            _searchBox.Border = AGSBorders.SolidColor(Colors.Green, 2f);
            _searchBox.Tint = Colors.Transparent;
            _searchBox.Pivot = new PointF(0f, 1f);
            _searchBox.GetComponent<ITextComponent>().PropertyChanged += onSearchPropertyChanged;

            _scrollingPanel = factory.UI.GetPanel("GameDebugTreeScrollingPanel", parent.Width, parent.Height - _searchBox.Height, 0f, 0f, parent);
            _scrollingPanel.RenderLayer = _layer;
            _scrollingPanel.Pivot = new PointF(0f, 0f);
            _scrollingPanel.Tint = Colors.Transparent;
            _scrollingPanel.Border = AGSBorders.SolidColor(Colors.Green, 2f);
            const float lineHeight = 42f;
            _treePanel = factory.UI.GetPanel("GameDebugTreePanel", 1f, 1f, 0f, _scrollingPanel.Height - lineHeight, _scrollingPanel);
            _treePanel.Tint = Colors.Transparent;
            _treePanel.RenderLayer = _layer;
            _treeView = _treePanel.AddComponent<ITreeViewComponent>();
            _treeView.OnNodeSelected.Subscribe(onTreeNodeSelected);
            factory.UI.CreateScrollingPanel(_scrollingPanel);
            parent.GetComponent<IScaleComponent>().PropertyChanged += (_, args) => 
            {
                if (args.PropertyName != nameof(IScaleComponent.Height)) return;
                _scrollingPanel.Image = new EmptyImage(_scrollingPanel.Width, parent.Height - _searchBox.Height);
                _treePanel.Y = _scrollingPanel.Height - lineHeight;
                _searchBox.Y = _parent.Height;
            };
        }

        public async Task Show()
        {
            await Task.Run(() => refresh());
            _scrollingPanel.Visible = true;
            _searchBox.Visible = true;
        }

        public void Hide()
        {
	        _scrollingPanel.Visible = false;
            _searchBox.Visible = false;
            _treeView.Tree = null;
        }

        public void Resize()
        {
            _scrollingPanel.Image = new EmptyImage(_parent.Width, _scrollingPanel.Image.Height);
            _searchBox.LabelRenderSize = new SizeF(_parent.Width, _searchBox.Height);
        }

        private void onSearchPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(ITextComponent.Text)) return;
            _treeView.SearchFilter = _searchBox.Text;
        }

        private void onTreeNodeSelected(NodeEventArgs args)
        {
            unselect();
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
            var animation = obj.GetComponent<IAnimationComponent>();
            var visibleComponent = obj.GetComponent<IVisibleComponent>();
            var image = obj.GetComponent<IImageComponent>();
            if (animation != null)
            {
                _lastSelectedObject = animation;
                IBorderStyle border = null;          
                border = animation.Border;
                _lastObjectBorder = border;
                IBorderStyle hoverBorder = AGSBorders.Gradient(new FourCorners<Color>(Colors.Yellow, Colors.Yellow.WithAlpha(150),
                                                                                      Colors.Yellow.WithAlpha(150), Colors.Yellow), 1, true);
                if (border == null) animation.Border = hoverBorder;
                else animation.Border = AGSBorders.Multiple(border, hoverBorder);
            }
            if (visibleComponent != null)
            {
                _lastMaskVisible = visibleComponent.Visible;
                _lastSelectedMaskVisible = visibleComponent;
                visibleComponent.Visible = true;
            }
            if (image != null && image.Opacity == 0)
            {
                _lastOpacity = image.Opacity;
                _lastSelectedMaskImage = image;
                image.Opacity = 100;
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

        private void unselect()
        {
            var lastSelectedObject = _lastSelectedObject;
            var lastSelectedMaskVisible = _lastSelectedMaskVisible;
            var lastSelectedMaskImage = _lastSelectedMaskImage;
            if (lastSelectedObject != null) lastSelectedObject.Border = _lastObjectBorder;
            if (lastSelectedMaskVisible != null) lastSelectedMaskVisible.Visible = _lastMaskVisible;
            if (lastSelectedMaskImage != null) lastSelectedMaskImage.Opacity = _lastOpacity;
            _lastSelectedObject = null;
            _lastObjectBorder = null;
            _lastMaskVisible = false;
            _lastOpacity = 0;
        }

        private void refresh()
        {
            _addedObjects.Clear();
            var root = addToTree("Game", null);
            var ui = addToTree("UI", root);

            foreach (var obj in _game.State.UI)
            {
                addObjectToTree(getRoot(obj), ui);
            }
            addRoomToTree(_game.State.Room, root);

            var rooms = addToTree("More Rooms", root);
            foreach (var room in _game.State.Rooms)
            {
                if (room == _game.State.Room) continue;
                addRoomToTree(room, rooms);
            }

            _treeView.Tree = root;
        }

        private void addRoomToTree(IRoom room, ITreeStringNode parent)
        {
            var roomNode = addToTree($"Room: {room.ID}", parent);
            var objects = addToTree("Objects", roomNode);
            var areas = addToTree("Areas", roomNode);

            addObjectToTree(getRoot(room.Background), objects);
            foreach (var obj in room.Objects)
            {
                addObjectToTree(getRoot(obj), objects);
            }
            foreach (var area in room.Areas)
            {
                var node = addToTree(area.ID, areas);
                node.Properties.Strings.SetValue(Fields.Type, NodeType.Area);
                node.Properties.Entities.SetValue(Fields.Entity, area);
            }
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
            foreach (var child in obj.TreeNode.Children)
            {
                addObjectToTree(child, node);
            }
        }

        private ITreeStringNode addToTree(string text, ITreeStringNode parent)
        {
            var node = new AGSTreeStringNode { Text = text };
            if (parent != null) node.TreeNode.SetParent(parent.TreeNode);
            return node;
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
    }
}
