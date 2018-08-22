using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AGS.API;
using AGS.Engine;
using GuiLabs.Undo;

namespace AGS.Editor
{
    public class InstancePropertyEditor : IInspectorPropertyEditor
    {
        private readonly SelectEditor _selectEditor;
        private readonly AGSEditor _editor;
        private readonly IObject _parentDialog;

        //todo: cache need to be refreshed/cleared when assembly is replaced in the app domain (i.e the user compiled new code).
        private static Dictionary<Type, List<IImplementationOption>> _interfaces = new Dictionary<Type, List<IImplementationOption>>();

        private class ComboItem : AGSStringItem
        {
            public ComboItem(IImplementationOption implementation)
            {
                Implementation = implementation;
                Text = implementation?.Name ?? "(null)";
            }

            public IImplementationOption Implementation { get; }
        }

        public InstancePropertyEditor(IUIFactory factory, ActionManager actions, StateModel model, AGSEditor editor, IObject parentDialog)
        {
            _parentDialog = parentDialog;
            _editor = editor;
            _selectEditor = new SelectEditor(factory, actions, model, getOptions, getValue);
        }

        public void AddEditorUI(string id, ITreeNodeView view, IProperty property)
        {
            _selectEditor.AddEditorUI(id, view, property);
        }

        public void RefreshUI()
        {
            _selectEditor.RefreshUI();
        }

        private List<IStringItem> getOptions(IProperty property)
        {
            var list = new List<IStringItem> { new ComboItem(null) };
            var implementations = _interfaces.GetOrAdd(property.PropertyType, getImplementations);
            foreach (var option in implementations)
            {
                list.Add(new ComboItem(option));
            }
            return list;
        }

        private Task<SelectEditor.ReturnValue> getValue(IStringItem item, IProperty property, Action closeCombobox)
        {
            closeCombobox();
            ComboItem typeItem = (ComboItem)item;
            return typeItem.Implementation?.Create() ?? Task.FromResult(new SelectEditor.ReturnValue(null, false));
        }

        private List<IImplementationOption> getImplementations(Type type)
        {
            List<IImplementationOption> implementations = new List<IImplementationOption>();

            var factories = type.GetCustomAttributes(typeof(HasFactoryAttribute), true);
            implementations.AddRange(factories.Select(f => new FactoryImplementationOption((HasFactoryAttribute)f, _editor, _parentDialog)));

            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && isConcreteImplementation(p))
                .Select(s => new TypeImplementationOption(s, _editor, _parentDialog));
            implementations.AddRange(types);

            return implementations;
        }

        private bool isConcreteImplementation(Type type)
        {
            if (type.IsInterface) return false;
            if (type.IsAbstract) return false;
            var attr = type.GetCustomAttribute<ConcreteImplementationAttribute>();
            if (attr != null && !attr.Browsable) return false;
            return true;
        }
    }
}