using System.Collections.Generic;
using System.Threading.Tasks;
using AGS.API;

namespace AGS.Engine
{
    public class GameDebugDisplayList : IDebugTab
    {
        private List<IObject> _lastDisplayList;
        private IListboxComponent _listBox;
        private IPanel _listPanel;
        private readonly IRenderLayer _layer;
        private readonly IGame _game;

        public GameDebugDisplayList(IGame game, IRenderLayer layer)
        {
            _game = game;
            _layer = layer;
            game.RenderLoop.OnBeforeRenderingDisplayList.Subscribe(onBeforeRenderingDisplayList);
        }

        public void Load(IPanel parent)
        {
            var factory = _game.Factory;
            _listPanel = factory.UI.GetPanel("GameDebugDisplayListPanel", 1f, 1f, 0f, parent.Height, parent);
            _listPanel.Tint = Colors.Transparent;
            _listPanel.RenderLayer = _layer;
            _listPanel.Anchor = new PointF(0f, 1f);
            _listPanel.AddComponent<IStackLayoutComponent>().StartLayout();
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
        }

        public async Task Show()
        {
            await Task.Run(() => refresh());
            _listPanel.Visible = true;
        }

        public void Hide()
        {
            _listPanel.Visible = false;
            _listBox.Items.Clear();
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
