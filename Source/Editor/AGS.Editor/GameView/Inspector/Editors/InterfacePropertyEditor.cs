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
    public class InterfacePropertyEditor : IInspectorPropertyEditor
    {
        private readonly SelectEditor _selectEditor;
        private readonly AGSEditor _editor;

        //todo: cache need to be refreshed/cleared when assembly is replaced in the app domain (i.e the user compiled new code).
        private static Dictionary<Type, List<IImplementation>> _interfaces = new Dictionary<Type, List<IImplementation>>();

        private interface IImplementation
        {
            string Name { get; }
            Task<SelectEditor.ReturnValue> Create();
        }

        private class TypeImplementation : IImplementation
        {
            private readonly Type _type;
            private readonly string _displayName;
            private readonly AGSEditor _editor;

            public TypeImplementation(Type type, AGSEditor editor)
            {
                _editor = editor;
                _type = type;
                var attr = type.GetCustomAttribute<ConcreteImplementationAttribute>();
                _displayName = attr.DisplayName;
            }

            public string Name => _displayName ?? _type.Name;

            public async Task<SelectEditor.ReturnValue> Create()
            {
                var factoryWizard = new FactoryWizard(_editor, null, null, null);
                (object result, MethodModel model, MethodWizardAttribute attr) = await factoryWizard.RunConstructor(_type);
                return new SelectEditor.ReturnValue(result, model == null);
            }
        }

        private class FactoryImplementation : IImplementation
        {
            private readonly string _methodName;
            private readonly object _factory;
            private readonly AGSEditor _editor;

            public FactoryImplementation(HasFactoryAttribute attr, AGSEditor editor)
            {
                _editor = editor;
                switch (attr.FactoryType)
                {
                    case nameof(IBorderFactory):
                        _factory = editor.Game.Factory.Graphics.Borders;
                        break;
                    default:
                        throw new NotSupportedException($"Not supported factory type: {attr.FactoryType}");
                }
                _methodName = attr.MethodName;
            }

            public string Name => _methodName;

            public async Task<SelectEditor.ReturnValue> Create()
            {
                var factoryWizard = new FactoryWizard(_editor, null, null, null);
                (object result, MethodModel model, MethodWizardAttribute attr) = await factoryWizard.RunMethod(_factory, _methodName);
                return new SelectEditor.ReturnValue(result, model == null);
            }
        }

        private class ComboItem : AGSStringItem
        {
            public ComboItem(IImplementation implementation)
            {
                Implementation = implementation;
                Text = implementation?.Name ?? "(null)";
            }

            public IImplementation Implementation { get; }
        }

        public InterfacePropertyEditor(IUIFactory factory, ActionManager actions, StateModel model, AGSEditor editor)
        {
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

        private Task<SelectEditor.ReturnValue> getValue(IStringItem item, IProperty property)
        {
            ComboItem typeItem = (ComboItem)item;
            return typeItem.Implementation?.Create() ?? Task.FromResult(new SelectEditor.ReturnValue(null, false));
        }

        private List<IImplementation> getImplementations(Type type)
        {
            List<IImplementation> implementations = new List<IImplementation>();

            var factories = type.GetCustomAttributes(typeof(HasFactoryAttribute), true);
            implementations.AddRange(factories.Select(f => new FactoryImplementation((HasFactoryAttribute)f, _editor)));

            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && isConcreteImplementation(p))
                .Select(s => new TypeImplementation(s, _editor));
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