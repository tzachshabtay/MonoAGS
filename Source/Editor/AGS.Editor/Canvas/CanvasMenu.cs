using System;
using System.Threading.Tasks;
using AGS.API;
using AGS.Engine;

namespace AGS.Editor
{
    public class CanvasMenu
    {
        private readonly IGameFactory _factory;
        private readonly IInput _input;

        public CanvasMenu(IGameFactory factory, IInput input)
        {
            _factory = factory;
            _input = input;
        }

        public void Load()
        {
            var layer = new AGSRenderLayer(AGSLayers.UI.Z - 1);
            var menu = _factory.UI.GetListBox("CanvasMenu", layer, isVisible: false, withScrollBars: false);
            const float width = 100f;

            menu.ContentsPanel.Tint = GameViewColors.Menu;

            bool isHovered = false;
            var (presetsMenu, numHovered) = getPresetsListbox(layer, () => isHovered);

            var whiteBrush = _factory.Graphics.Brushes.LoadSolidBrush(Colors.White);
            var textConfig = new AGSTextConfig(whiteBrush, autoFit: AutoFit.TextShouldFitLabel);
            var idle = new ButtonAnimation(null, textConfig, Colors.Transparent);
            var hovered = new ButtonAnimation(null, textConfig, GameViewColors.HoveredMenuItem);
            menu.ListboxComponent.ListItemFactory = (string text) =>
            {
                var button = _factory.UI.GetButton($"MenuItem_{text}", idle, hovered, hovered,
                                                   0f, 0f, width: width, height: 25f, text: text);
                button.Pivot = new PointF(0f, 1f);
                button.RenderLayer = layer;

                var subMenuIcon = _factory.UI.GetLabel($"{text}_SubMenuIcon", "", 25f, 25f, 80f, 0f, button, FontIcons.IconConfig);
                subMenuIcon.Text = FontIcons.SubMenu;
                subMenuIcon.RenderLayer = button.RenderLayer;

                button.MouseEnter.Subscribe(() =>
                {
                    isHovered = true;
                    presetsMenu.ContentsPanel.Visible = true;
                });

                button.MouseLeave.Subscribe(async () =>
                {
                    isHovered = false;
                    await Task.Delay(100);
                    if (numHovered() <= 0)
                    {
                        presetsMenu.ContentsPanel.Visible = false;
                    }
                });

                return button;
            };

            menu.ListboxComponent.Items.Add(new AGSStringItem { Text = "Create" });

            menu.ContentsPanel.Pivot = (0f, 1f);

            _input.MouseUp.Subscribe((MouseButtonEventArgs args) => 
            {
                if (args.Button == MouseButton.Right)
                {
                    menu.ContentsPanel.Position = (args.MousePosition.XMainViewport, args.MousePosition.YMainViewport);
                    presetsMenu.ContentsPanel.Position = (args.MousePosition.XMainViewport + width + 3f, Math.Max(100f, args.MousePosition.YMainViewport));
                    menu.ContentsPanel.Visible = true;
                }
                else if (args.ClickedEntity == null) 
                { 
                    menu.ContentsPanel.Visible = false; 
                    presetsMenu.ContentsPanel.Visible = false; 
                } 
            });
        }

        private (IListbox, Func<int>) getPresetsListbox(IRenderLayer layer, Func<bool> isParentHovered)
        {
            var menu = _factory.UI.GetListBox("PresetsMenu", layer, isVisible: false, withScrollBars: false);

            menu.ContentsPanel.Tint = GameViewColors.Menu;

            var whiteBrush = _factory.Graphics.Brushes.LoadSolidBrush(Colors.White);
            var textConfig = new AGSTextConfig(whiteBrush, autoFit: AutoFit.TextShouldFitLabel);
            var idle = new ButtonAnimation(null, textConfig, Colors.Transparent);
            var hovered = new ButtonAnimation(null, textConfig, GameViewColors.HoveredMenuItem);
            int numHovered = 0;
            menu.ListboxComponent.ListItemFactory = (string text) =>
            {
                var button = _factory.UI.GetButton($"MenuItem_{text}", idle, hovered, hovered,
                                                                 0f, 0f, width: 100f, height: 25f, text: text);
                button.Pivot = new PointF(0f, 1f);
                button.RenderLayer = layer;

                button.MouseEnter.Subscribe(() =>
                {
                    numHovered++;
                });

                button.MouseLeave.Subscribe(async () =>
                {
                    numHovered--;
                    await Task.Delay(100);
                    if (numHovered <= 0 && !isParentHovered())
                    {
                        menu.ContentsPanel.Visible = false;
                    }
                });

                return button;
            };

            menu.ListboxComponent.Items.Add(new AGSStringItem { Text = "Object" });
            menu.ListboxComponent.Items.Add(new AGSStringItem { Text = "Character" });
            menu.ListboxComponent.Items.Add(new AGSStringItem { Text = "Button" });
            menu.ListboxComponent.Items.Add(new AGSStringItem { Text = "Label" });

            menu.ContentsPanel.Pivot = (0f, 1f);
            return (menu, () => numHovered);
        }
    }
}