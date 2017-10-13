using System;
using AGS.API;

namespace AGS.Engine
{
    public class BoolPropertyEditor : IInspectorPropertyEditor
    {
        private readonly IUIFactory _factory;
        private readonly IIconFactory _icons;

        public BoolPropertyEditor(IUIFactory factory, IIconFactory icons)
        {
            _factory = factory;
            _icons = icons;
        }

        public void AddEditorUI(string id, ITreeNodeView view, InspectorProperty property)
        {
			var label = view.TreeItem;
            var idleColor = Colors.White;
            var hoverColor = Colors.Yellow;
            const float lineWidth = 1f;
            const float padding = 300f;
            var checkIcon = _icons.GetXIcon(color: idleColor, padding: padding);
            var checkHoverIcon = _icons.GetXIcon(color: hoverColor, padding: padding);
            var notChecked = new ButtonAnimation(AGSBorders.SolidColor(idleColor, lineWidth), null, null);
            var @checked = new ButtonAnimation(AGSBorders.Multiple(AGSBorders.SolidColor(idleColor, lineWidth), checkIcon), null, null);
            var hoverNotChecked = new ButtonAnimation(AGSBorders.SolidColor(hoverColor, lineWidth), null, null);
            var hoverChecked = new ButtonAnimation(AGSBorders.Multiple(AGSBorders.SolidColor(hoverColor, lineWidth), checkHoverIcon), null, null);
            var checkbox = _factory.GetCheckBox(id, notChecked, hoverNotChecked, @checked, hoverChecked,
												 label.X, label.Y, label.TreeNode.Parent,
												 "", width: 20f, height: 20f);
			checkbox.RenderLayer = label.RenderLayer;
			checkbox.Z = label.Z;
            checkbox.Checked = bool.Parse(property.Value);
            checkbox.OnCheckChanged.Subscribe(args => 
            {
                property.Prop.SetValue(property.Object, args.Checked);
            });
        }
    }
}
