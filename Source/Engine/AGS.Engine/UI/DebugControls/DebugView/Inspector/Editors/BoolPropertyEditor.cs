using AGS.API;

namespace AGS.Engine
{
    public class BoolPropertyEditor : IInspectorPropertyEditor
    {
        private readonly IGameFactory _factory;
        private InspectorProperty _property;
        private ICheckboxComponent _checkbox;

        public BoolPropertyEditor(IGameFactory factory)
        {
            _factory = factory;
        }

        public static ICheckBox CreateCheckbox(IUIControl label, IGameFactory factory, string id)
        {
            var idleColor = Colors.White;
            var hoverColor = Colors.Yellow;
            const float lineWidth = 1f;
            const float padding = 300f;
            var checkIcon = factory.Graphics.Icons.GetXIcon(color: idleColor, padding: padding);
            var checkHoverIcon = factory.Graphics.Icons.GetXIcon(color: hoverColor, padding: padding);
            var notChecked = new ButtonAnimation(AGSBorders.SolidColor(idleColor, lineWidth), null, null);
            var @checked = new ButtonAnimation(AGSBorders.Multiple(AGSBorders.SolidColor(idleColor, lineWidth), checkIcon), null, null);
            var hoverNotChecked = new ButtonAnimation(AGSBorders.SolidColor(hoverColor, lineWidth), null, null);
            var hoverChecked = new ButtonAnimation(AGSBorders.Multiple(AGSBorders.SolidColor(hoverColor, lineWidth), checkHoverIcon), null, null);
            var checkbox = factory.UI.GetCheckBox(id, notChecked, hoverNotChecked, @checked, hoverChecked,
                                                 label.X, label.Y, label.TreeNode.Parent,
                                                 "", width: 20f, height: 20f);
            checkbox.RenderLayer = label.RenderLayer;
            checkbox.Z = label.Z;
            return checkbox;
        }

        public void AddEditorUI(string id, ITreeNodeView view, InspectorProperty property)
        {
            _property = property;
			var label = view.TreeItem;
            var checkbox = CreateCheckbox(label, _factory, id);
            _checkbox = checkbox;
            checkbox.Checked = bool.Parse(property.Value);
            checkbox.OnCheckChanged.Subscribe(args => 
            {
                property.Prop.SetValue(property.Object, args.Checked);
            });
        }

        public void RefreshUI()
        {
            if (_checkbox == null) return;
            _checkbox.Checked = bool.Parse(_property.Value);
        }
    }
}
