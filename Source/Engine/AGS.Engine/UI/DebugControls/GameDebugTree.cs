using AGS.API;

namespace AGS.Engine
{
    public class GameDebugTree
    {
        private readonly IRenderLayer _layer;
        private const string _panelId = "Game Debug Tree Panel";
        private IPanel _panel;
        private ITreeViewComponent _treeView;
        private readonly IGame _game;
        private readonly IConcurrentHashSet<string> _addedObjects;

        private IAnimationContainer _lastSelectedObject;
        private IBorderStyle _lastObjectBorder;
        private IObject _lastSelectedAreaDebugDraw;
        private bool _lastAreaDebugVisible;

        public GameDebugTree(IGame game)
        {
            _game = game;
            _addedObjects = new AGSConcurrentHashSet<string>(100, false);
            _layer = new AGSRenderLayer(AGSLayers.UI.Z - 1, independentResolution: new Size(1800, 1200));
        }

        public void Load()
        {
            const float headerHeight = 50f;
            const float borderWidth = 3f;
            IGameFactory factory = _game.Factory;
            _panel = factory.UI.GetPanel(_panelId, _layer.IndependentResolution.Value.Width / 4f, _layer.IndependentResolution.Value.Height, 
                                         0f, _layer.IndependentResolution.Value.Height / 2f);
            _panel.Anchor = new PointF(0f, 0.5f);
            _panel.Visible = false;
            _panel.Tint = Colors.Black.WithAlpha(150);
            _panel.Border = AGSBorders.SolidColor(Colors.Green, borderWidth, hasRoundCorners: true);
            _panel.RenderLayer = _layer;

            var headerLabel = factory.UI.GetLabel("GameDebugTreeLabel", "Game Debug", _panel.Width, headerHeight, 0f, _panel.Height - headerHeight,
									  _panel, new AGSTextConfig(alignment: Alignment.MiddleCenter, autoFit: AutoFit.TextShouldFitLabel));
            headerLabel.Tint = Colors.Transparent;
            headerLabel.Border = _panel.Border;
            headerLabel.RenderLayer = _layer;

            var xButton = factory.UI.GetButton("GameDebugTreeCloseButton", (IAnimation)null, null, null, 0f, _panel.Height - headerHeight + 5f, _panel, "X",
            								   new AGSTextConfig(factory.Graphics.Brushes.LoadSolidBrush(Colors.Red),
            													 autoFit: AutoFit.TextShouldFitLabel, alignment: Alignment.MiddleCenter),
            													 width: 40f, height: 40f);
            xButton.Anchor = new PointF();
            xButton.RenderLayer = _layer;
            xButton.Tint = Colors.Transparent;
            xButton.MouseEnter.Subscribe((_, __) => xButton.TextConfig = AGSTextConfig.ChangeColor(xButton.TextConfig, Colors.Yellow, Colors.White, 0.3f));
            xButton.MouseLeave.Subscribe((_, __) => xButton.TextConfig = AGSTextConfig.ChangeColor(xButton.TextConfig, Colors.Red, Colors.Transparent, 0f));
            xButton.MouseClicked.Subscribe((_, __) => Hide());

            var treePanel = factory.UI.GetPanel("GameDebugTreePanel", 1f, 1f, 0f, _panel.Height - headerHeight - 40f, _panel);
            treePanel.Tint = Colors.Transparent;
            treePanel.RenderLayer = _layer;
            _treeView = treePanel.AddComponent<ITreeViewComponent>();
            _treeView.OnNodeSelected.Subscribe(onTreeNodeSelected);
        }

        public bool Visible { get { return _panel.Visible; } }

        public void Show()
        {
            refresh();
            _panel.Visible = true;
        }

        public void Hide()
        {
	        _panel.Visible = false;
        }

        private void onTreeNodeSelected(object sender, NodeEventArgs args)
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
            var animation = obj.GetComponent<IAnimationContainer>();
            _lastSelectedObject = animation;
            IBorderStyle border = null;
            if (animation != null) border = animation.Border;
            _lastObjectBorder = border;
            IBorderStyle hoverBorder = AGSBorders.Gradient(new FourCorners<Color>(Colors.Yellow, Colors.Yellow.WithAlpha(150),
																				  Colors.Yellow.WithAlpha(150), Colors.Yellow), 1, true);
            if (border == null) animation.Border = hoverBorder;
            else animation.Border = AGSBorders.Multiple(border, hoverBorder);
        }

        private void selectArea(ITreeStringNode node)
        { 
            var obj = node.Properties.Entities.GetValue(Fields.Entity);
            var area = obj.GetComponent<IAreaComponent>();
            var debugMask = area.Mask.DebugDraw;
            if (debugMask != null)
            {
                _lastAreaDebugVisible = debugMask.Visible;
                _lastSelectedAreaDebugDraw = debugMask;
                debugMask.Visible = true;
            }
        }

        private void unselect()
        {
            var lastSelectedObject = _lastSelectedObject;
            var lastSelectedArea = _lastSelectedAreaDebugDraw;
            if (lastSelectedObject != null) lastSelectedObject.Border = _lastObjectBorder;
            if (lastSelectedArea != null) lastSelectedArea.Visible = _lastAreaDebugVisible;
            _lastSelectedObject = null;
            _lastObjectBorder = null;
            _lastAreaDebugVisible = false;
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
            var roomNode = addToTree(string.Format("Room: {0}", room.ID), parent);
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
            obj = obj.TreeNode.GetRoot();
            if (obj.ID == _panelId) return null;
            return obj;
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
