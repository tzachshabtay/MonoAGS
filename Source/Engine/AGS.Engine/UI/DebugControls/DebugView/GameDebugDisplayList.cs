using System.Collections.Generic;
using System.Threading.Tasks;
using AGS.API;

namespace AGS.Engine
{
    public class GameDebugDisplayList : IDebugTab
    {
        private List<IObject> _lastDisplayList;
        private IListboxComponent _listBox;
        private IPanel _listPanel, _scrollingPanel, _parent;
        private IStackLayoutComponent _layout;
        private readonly IRenderLayer _layer;
        private readonly IGame _game;

        public GameDebugDisplayList(IGame game, IRenderLayer layer)
        {
            _game = game;
            _layer = layer;
            game.RenderLoop.OnBeforeRenderingDisplayList.Subscribe(onBeforeRenderingDisplayList);
        }

        public IPanel Panel => _scrollingPanel;

        public void Load(IPanel parent)
        {
            _parent = parent;
            var factory = _game.Factory;
			_scrollingPanel = factory.UI.GetPanel("GameDebugDisplayListScrollingPanel", parent.Width, parent.Height / 2f, 0f, parent.Height / 2f, parent);
			_scrollingPanel.RenderLayer = _layer;
			_scrollingPanel.Anchor = new PointF(0f, 0f);
			_scrollingPanel.Tint = Colors.Transparent;
			_scrollingPanel.Border = AGSBorders.SolidColor(Colors.Green, 2f);
            _scrollingPanel.Visible = false;

            _listPanel = factory.UI.GetPanel("GameDebugDisplayListPanel", 1f, 1f, 0f, _scrollingPanel.Height - 10, _scrollingPanel);
            _listPanel.Tint = Colors.Transparent;
            _listPanel.RenderLayer = _layer;
            _listPanel.Anchor = new PointF(0f, 1f);
            _listPanel.AddComponent<IBoundingBoxWithChildrenComponent>();
            _layout = _listPanel.AddComponent<IStackLayoutComponent>();
            _listBox = _listPanel.AddComponent<IListboxComponent>();
            var yellowBrush = factory.Graphics.Brushes.LoadSolidBrush(Colors.Yellow);
            var whiteBrush = factory.Graphics.Brushes.LoadSolidBrush(Colors.White);
            _listBox.ItemButtonFactory = text =>
            {
                var button = factory.UI.GetButton("GameDebugDisplayListPanel_" + text,
                                                  new ButtonAnimation(null, new AGSTextConfig(whiteBrush, autoFit: AutoFit.TextShouldWrapAndLabelShouldFitHeight), null),
                                                  new ButtonAnimation(null, new AGSTextConfig(yellowBrush, autoFit: AutoFit.TextShouldWrapAndLabelShouldFitHeight), null),
                                                  new ButtonAnimation(null, new AGSTextConfig(yellowBrush, outlineBrush: whiteBrush, outlineWidth: 0.5f, autoFit: AutoFit.TextShouldWrapAndLabelShouldFitHeight), null),
                                                  0f, 0f, width: 500f, height: 50f);
                button.RenderLayer = parent.RenderLayer;
                return button;
            };
            factory.UI.CreateScrollingPanel(_scrollingPanel);
            _scrollingPanel.GetComponent<IScaleComponent>().PropertyChanged += (_, args) =>
			{
                if (args.PropertyName != nameof(IScaleComponent.Height)) return;
                _listPanel.Y = _scrollingPanel.Height - 10f;
			};
        }

        public async Task Show()
        {
            await Task.Run(() => refresh());
            _layout.StartLayout();
            _scrollingPanel.Visible = true;
        }

        public void Hide()
        {
            _scrollingPanel.Visible = false;
            _layout.StopLayout();
            _listBox.Items.Clear();
        }

		public void Resize()
		{
			_scrollingPanel.Image = new EmptyImage(_parent.Width, _scrollingPanel.Image.Height);
		}

        private void refresh()
        {
            _listBox.Items.Clear();
            if (_lastDisplayList == null) return;
            foreach (var obj in _lastDisplayList)
            {
                _listBox.Items.Add(new AGSStringItem { Text = obj.ID });
            }
        }

        private void onBeforeRenderingDisplayList(DisplayListEventArgs args)
        {
            SortDebugger.DebugIfNeeded(args.DisplayList);
            _lastDisplayList = args.DisplayList;
        }

    }
}
