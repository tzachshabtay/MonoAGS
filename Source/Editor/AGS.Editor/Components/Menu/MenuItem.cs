using System;
using AGS.API;
using AGS.Engine;

namespace AGS.Editor
{
    public class MenuItem : AGSStringItem
    {
        public MenuItem(string text, Action onClick)
        {
            Text = text;
            OnClick = onClick;
        }

        public MenuItem(string text, Menu subMenu)
        {
            Text = text;
            SubMenu = subMenu;
            subMenu.ParentMenuItem = this;
        }

        public Menu ParentMenu { get; set; }
        public Action OnClick { get; }
        public Menu SubMenu { get; }

        public bool IsHovered { get; private set; }

        public IUIControl GetControl(IGameFactory factory, IRenderLayer layer, ButtonAnimation idle, ButtonAnimation hovered, 
                                     float width, float height)
        {
            var button = factory.UI.GetButton($"MenuItem_{Text}", idle, hovered, hovered,
                                              0f, 0f, width: width, height: height, text: Text);
            button.Pivot = new PointF(0f, 1f);
            button.RenderLayer = layer;

            if (SubMenu != null)
            {
                var subMenuIcon = factory.UI.GetLabel($"{Text}_SubMenuIcon", "", 25f, 25f, 80f, 0f, button, FontIcons.IconConfig);
                subMenuIcon.Text = FontIcons.SubMenu;
                subMenuIcon.RenderLayer = button.RenderLayer;
            }

            button.MouseEnter.Subscribe(() =>
            {
                IsHovered = true;

                hide(ParentMenu);
                if (SubMenu != null) SubMenu.Visible = true;
            });

            button.MouseLeave.Subscribe(() =>
            {
                IsHovered = false;
            });

            if (OnClick != null) button.MouseClicked.Subscribe(OnClick);

            return button;
        }

        private void hide(Menu menu)
        {
            foreach (var item in menu.Children)
            {
                if (item.SubMenu != null)
                    item.SubMenu.Visible = false;
            }
        }
    }
}