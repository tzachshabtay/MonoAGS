using System;
using System.Linq;
using AGS.API;
using AGS.Engine;

namespace AGS.Editor
{
    public class Menu
    {
        private readonly Resolver _gameResolver;
        private MenuItem[] _menuItems;
        private readonly string _id;
        private IListbox _menu;
        private readonly float _width;
        private readonly IGameSettings _settings;
        private const float _height = 25f;
        private (float x, float y) _position;
        private IModalWindowComponent _modal;

        public Menu(Resolver gameResolver, IGameSettings settings, string id, float width, params MenuItem[] menuItems)
        {
            _gameResolver = gameResolver;
            _settings = settings;
            _width = width;
            _menuItems = menuItems;
            foreach (var item in menuItems) item.ParentMenu = this;
            _id = id;
        }

        public bool IsHovered => _menuItems.Any(m => m.IsHovered);

        public MenuItem ParentMenuItem { get; set; }

        public MenuItem[] Children => _menuItems;

        public bool Visible 
        { 
            get => _menu.ContentsPanel.Visible; 
            set
            {
                _menu.ContentsPanel.Visible = value;
                if (value)
                {
                    _modal?.GrabFocus();
                }
                else
                {
                    _modal?.LoseFocus();
                    foreach (var item in _menuItems)
                    {
                        if (item.SubMenu != null) item.SubMenu.Visible = false;
                    }
                }

            }
        }

        public (float x, float y) OriginalPosition { get; private set; }

        public (float x, float y) Position
        {
            get => _position;
            set
            {
                OriginalPosition = value;
                float minY = _height * _menuItems.Length;
                float maxX = (_menu.ContentsPanel.RenderLayer?.IndependentResolution?.Width ?? _settings.VirtualResolution.Width) - _width;
                float x = value.x;
                if (value.x > maxX)
                {
                    x = ParentMenuItem == null ? maxX : ParentMenuItem.ParentMenu.Position.x - _width - 3f;
                }
                float y = Math.Max(minY, value.y);
                _position = (x, y);
                _menu.ContentsPanel.Position = _position;
                for (int index = 0; index < _menuItems.Length; index++)
                {
                    var item = _menuItems[index];
                    if (item.SubMenu != null) item.SubMenu.Position = (x + _width + 3f, y - index * _height);
                }
            }
        }

        public void Load(IGameFactory factory, IDefaultsSettings settings)
        {
            var layer = new AGSRenderLayer(AGSLayers.UI.Z - 1);
            _menu = factory.UI.GetListBox(_id, layer, isVisible: false, withScrollBars: false);
            if (ParentMenuItem == null)
            {
                var host = new AGSComponentHost(_gameResolver);
                host.Init(_menu.ContentsPanel, typeof(AGSComponentHost));
                _modal = host.AddComponent<IModalWindowComponent>();
            }

            _menu.ContentsPanel.Tint = GameViewColors.Menu;

            var whiteBrush = factory.Graphics.Brushes.LoadSolidBrush(Colors.White);
            var textConfig = factory.Fonts.GetTextConfig(whiteBrush, autoFit: AutoFit.TextShouldFitLabel);
            var idle = new ButtonAnimation(null, textConfig, Colors.Transparent);
            var hovered = new ButtonAnimation(null, textConfig, GameViewColors.HoveredMenuItem);

            _menu.ListboxComponent.ListItemFactory = text =>
            {
                var menuItem = _menuItems.First(m => m.Text == text);
                return menuItem.GetControl(factory, layer, idle, hovered, _width, _height);
            };

            _menu.ListboxComponent.Padding = new SizeF();
            _menu.ListboxComponent.Items.AddRange(_menuItems.Cast<IStringItem>().ToList());
            _menu.ContentsPanel.Pivot = (0f, 1f);

            foreach (var item in _menuItems)
            {
                if (item.SubMenu != null) item.SubMenu.Load(factory, settings);
            }
        }
    }
}
