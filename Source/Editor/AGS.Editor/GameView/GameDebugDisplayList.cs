using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using AGS.API;

namespace AGS.Engine
{
    public class GameDebugDisplayList : IDebugTab
    {
        private List<IObject> _lastDisplayList;
        private IListboxComponent _listBox;
        private IPanel _listPanel, _scrollingPanel, _contentsPanel, _parent;
        private ITextBox _searchBox;
        private IStackLayoutComponent _layout;
        private readonly IRenderLayer _layer;
        private readonly IGame _game;

        const float _gutterSize = 15f;
        const float _padding = 42f;

        public GameDebugDisplayList(IGame game, IRenderLayer layer)
        {
            _game = game;
            _layer = layer;
            game.RenderPipeline.OnBeforeProcessingDisplayList.Subscribe(onBeforeProcessingDisplayList);
        }

        public IPanel Panel => _scrollingPanel;

        public void Load(IPanel parent)
        {
            _parent = parent;
            var factory = _game.Factory;

            _searchBox = factory.UI.GetTextBox("GameDebugDisplayListSearchBox", 0f, parent.Height, parent, "Search...", width: parent.Width, height: 30f);
            _searchBox.RenderLayer = _layer;
            _searchBox.Border = AGSBorders.SolidColor(GameViewColors.Border, 2f);
            _searchBox.Tint = GameViewColors.Textbox;
            _searchBox.Pivot = new PointF(0f, 1f);
            _searchBox.Visible = false;
            _searchBox.GetComponent<ITextComponent>().PropertyChanged += onSearchPropertyChanged;

            _scrollingPanel = factory.UI.GetPanel("GameDebugDisplayListScrollingPanel", parent.Width - _gutterSize, parent.Height - _searchBox.Height - _gutterSize, 0f, 0f, parent);
			_scrollingPanel.RenderLayer = _layer;
			_scrollingPanel.Pivot = new PointF(0f, 0f);
			_scrollingPanel.Tint = Colors.Transparent;
            _scrollingPanel.Border = AGSBorders.SolidColor(GameViewColors.Border, 2f);
            _scrollingPanel.Visible = false;
            _contentsPanel = factory.UI.CreateScrollingPanel(_scrollingPanel);

            _listPanel = factory.UI.GetPanel("GameDebugDisplayListPanel", 1f, 1f, 0f, _contentsPanel.Height - _padding, _contentsPanel);
            _listPanel.Tint = Colors.Transparent;
            _listPanel.RenderLayer = _layer;
            _listPanel.Pivot = new PointF(0f, 1f);
            _listPanel.AddComponent<IBoundingBoxWithChildrenComponent>();
            _layout = _listPanel.AddComponent<IStackLayoutComponent>();
            _listBox = _listPanel.AddComponent<IListboxComponent>();
            var hoverBrush = factory.Graphics.Brushes.LoadSolidBrush(GameViewColors.HoveredText);
            var textBrush = factory.Graphics.Brushes.LoadSolidBrush(GameViewColors.Text);
            _listBox.ItemButtonFactory = text =>
            {
                var button = factory.UI.GetButton("GameDebugDisplayListPanel_" + text,
                                                  new ButtonAnimation(null, new AGSTextConfig(textBrush, autoFit: AutoFit.LabelShouldFitText), null),
                                                  new ButtonAnimation(null, new AGSTextConfig(hoverBrush, autoFit: AutoFit.LabelShouldFitText), null),
                                                  new ButtonAnimation(null, new AGSTextConfig(hoverBrush, outlineBrush: textBrush, outlineWidth: 0.5f, autoFit: AutoFit.LabelShouldFitText), null),
                                                  0f, 0f, width: 500f, height: 50f);
                button.RenderLayer = parent.RenderLayer;
                return button;
            };
            parent.GetComponent<IScaleComponent>().PropertyChanged += (_, args) =>
			{
                if (args.PropertyName != nameof(IScaleComponent.Height)) return;
                _contentsPanel.BaseSize = new SizeF(_contentsPanel.Width, parent.Height - _searchBox.Height - _gutterSize);
                _listPanel.Y = _contentsPanel.Height - _padding;
                _searchBox.Y = _parent.Height;
			};
        }

        public async Task Show()
        {
            await Task.Run(() => refresh());
            _layout.StartLayout();
            _scrollingPanel.Visible = true;
            _searchBox.Visible = true;
        }

        public void Hide()
        {
            _scrollingPanel.Visible = false;
            _searchBox.Visible = false;
            _layout.StopLayout();
            _listBox.Items.Clear();
        }

		public void Resize()
		{
            _contentsPanel.BaseSize = new SizeF(_parent.Width - _gutterSize, _contentsPanel.Height);
            _searchBox.LabelRenderSize = new SizeF(_parent.Width, _searchBox.Height);
            _searchBox.Watermark.LabelRenderSize = new SizeF(_parent.Width, _searchBox.Height);
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

        private void onBeforeProcessingDisplayList(DisplayListEventArgs args)
        {
            SortDebugger.DebugIfNeeded(args.DisplayList);
            _lastDisplayList = args.DisplayList;
        }

        private void onSearchPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(ITextComponent.Text)) return;
            _listBox.SearchFilter = _searchBox.Text;
        }
    }
}
