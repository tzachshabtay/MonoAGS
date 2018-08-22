using System;
using System.Reflection;
using System.Threading.Tasks;
using AGS.API;

namespace AGS.Editor
{
    public class TypeImplementationOption : IImplementationOption
    {
        private readonly Type _type;
        private readonly string _displayName;
        private readonly AGSEditor _editor;
        private readonly IObject _parentDialog;

        public TypeImplementationOption(Type type, AGSEditor editor, IObject parentDialog)
        {
            _parentDialog = parentDialog;
            _editor = editor;
            _type = type;
            var attr = type.GetCustomAttribute<ConcreteImplementationAttribute>();
            _displayName = attr?.DisplayName;
        }

        public string Name => _displayName ?? _type.Name;

        public async Task<SelectEditor.ReturnValue> Create()
        {
            var factoryWizard = new FactoryWizard(_parentDialog, _editor, null, null, null);
            (object result, MethodModel model, MethodWizardAttribute attr) = await factoryWizard.RunConstructor(_type);
            return new SelectEditor.ReturnValue(result, model == null);
        }
    }
}