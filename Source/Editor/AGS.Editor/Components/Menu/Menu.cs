using System;
using System.Linq;
using AGS.API;
using AGS.Engine;

namespace AGS.Editor
{
    public class Menu
    {
        private MenuItem[] _menuItems;
        private readonly string _id;
        private IListbox _menu;
        private readonly float _width;
        private const float _height = 25f;

        public Menu(string id, float width, params MenuItem[] menuItems)
        {
            _width = width;
            _menuItems = menuItems;
            foreach (var item in menuItems) item.ParentMenu = this;
            _id = id;
        }

        public bool IsHovered => _menuItems.Any(m => m.IsHovered);

        public MenuItem ParentMenuItem { get; set; }

        public bool Visible 
        { 
            get => _menu.ContentsPanel.Visible; 
            set
            {
                _menu.ContentsPanel.Visible = value;
                if (!value)
                {
                    foreach (var item in _menuItems)
                    {
                        if (item.SubMenu != null) item.SubMenu.Visible = false;
                    }
                }
            }
        }

        public (float x, float y) Position
        {
            set
            {
                float minY = _height * _menuItems.Length;
                _menu.ContentsPanel.Position = (value.x, Math.Max(minY, value.y));
                (float x, float y) = value;
                for (int index = 0; index < _menuItems.Length; index++)
                {
                    var item = _menuItems[index];
                    if (item.SubMenu != null) item.SubMenu.Position = (x + _width + 3f, y - index * _height);
                }
            }
        }

        public void Load(IGameFactory factory)
        {
            var layer = new AGSRenderLayer(AGSLayers.UI.Z - 1);
            _menu = factory.UI.GetListBox(_id, layer, isVisible: false, withScrollBars: false);

            _menu.ContentsPanel.Tint = GameViewColors.Menu;

            var whiteBrush = factory.Graphics.Brushes.LoadSolidBrush(Colors.White);
            var textConfig = new AGSTextConfig(whiteBrush, autoFit: AutoFit.TextShouldFitLabel);
            var idle = new ButtonAnimation(null, textConfig, Colors.Transparent);
            var hovered = new ButtonAnimation(null, textConfig, GameViewColors.HoveredMenuItem);

            _menu.ListboxComponent.ListItemFactory = (string text) =>
            {
                var menuItem = _menuItems.First(m => m.Text == text);
                return menuItem.GetControl(factory, layer, idle, hovered, _width, _height);
            };

            _menu.ListboxComponent.Items.AddRange(_menuItems.Cast<IStringItem>().ToList());
            _menu.ContentsPanel.Pivot = (0f, 1f);

            foreach (var item in _menuItems)
            {
                if (item.SubMenu != null) item.SubMenu.Load(factory);
            }
        }
    }
}