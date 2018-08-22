using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AGS.API;
using AGS.Engine;
using GuiLabs.Undo;

namespace AGS.Editor
{
    public class EnumPropertyEditor : IInspectorPropertyEditor
    {
        private readonly SelectEditor _editor;

        public EnumPropertyEditor(IUIFactory factory, ActionManager actions, StateModel model)
        {
            _editor = new SelectEditor(factory, actions, model, getOptions, getValue);
        }

        public void AddEditorUI(string id, ITreeNodeView view, IProperty property)
        {
            _editor.AddEditorUI(id, view, property);
        }

        public void RefreshUI() => _editor.RefreshUI();

        private List<IStringItem> getOptions(IProperty property)
        {
            var list = new List<IStringItem>();
            Type enumType = property.PropertyType;
            foreach (var option in Enum.GetValues(enumType))
            {
                list.Add(new AGSStringItem { Text = option.ToString() });
            }
            return list;
        }

        private Task<SelectEditor.ReturnValue> getValue(IStringItem item, IProperty property, Action closeCombobox)
        {
            Type enumType = property.PropertyType;
            return Task.FromResult(new SelectEditor.ReturnValue(Enum.Parse(enumType, item.Text), false));
        }
    }
}
