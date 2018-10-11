using System;
using AGS.API;
using GuiLabs.Undo;

namespace AGS.Editor
{
    public class NullablePropertyEditor : IInspectorPropertyEditor
    {
        private ICheckBox _nullBox;
        private readonly IGameFactory _factory;
        private readonly ActionManager _actions;
        private readonly StateModel _model;
        private readonly IEditorSupportsNulls _editor;
        private IProperty _property;

        public NullablePropertyEditor(IGameFactory factory, ActionManager actions, 
                                      StateModel model, IEditorSupportsNulls editor)
        {
            _factory = factory;
            _actions = actions;
            _model = model;
            _editor = editor;
        }

        public void AddEditorUI(string id, ITreeNodeView view, IProperty property)
        {
            _property = property;
            _nullBox = BoolPropertyEditor.CreateCheckbox(view.TreeItem, _factory, id + "_NullBox");
            refreshChecked();

            _nullBox.OnCheckChanged.Subscribe(args =>
            {
                object val = args.Checked ? Activator.CreateInstance(Nullable.GetUnderlyingType(property.PropertyType)) : null;

                if (args.UserInitiated) _actions.RecordAction(new PropertyAction(property, val, _model));
                else property.Value = new ValueModel(val);

                _editor.OnNullChanged(!_nullBox.Checked);
            });

            _editor.AddEditorUI(id, view, property);
            _editor.OnNullChanged(!_nullBox.Checked);
        }

        public void RefreshUI()
        {
            if (_property == null) return;
            refreshChecked();
        }

        private void refreshChecked()
        {
            bool newChecked = (_property.ValueString != InspectorProperty.NullValue);
            if (_nullBox.Checked == newChecked) 
            {
                _editor.RefreshUI();
                return;
            }
            _nullBox.Checked = newChecked;
        }
    }
}