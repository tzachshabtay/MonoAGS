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
            var checkbox = factory.UI.GetCheckBox(id, (ButtonAnimation)null, null, null, null,
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
