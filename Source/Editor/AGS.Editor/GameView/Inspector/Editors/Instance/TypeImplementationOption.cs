using System;
using System.Reflection;
using System.Threading.Tasks;
using AGS.API;
using Humanizer;

namespace AGS.Editor
{
    public class TypeImplementationOption : IImplementationOption
    {
        private readonly Type _type;
        private readonly AGSEditor _editor;

        public TypeImplementationOption(Type type, Type targetType, AGSEditor editor)
        {
            _editor = editor;
            if (type.IsGenericTypeDefinition)
            {
                _type = type.MakeGenericType(targetType.GetGenericArguments());
            }
            else _type = type;
            var attr = type.GetCustomAttribute<ConcreteImplementationAttribute>();
            Name = attr?.DisplayName ?? _type.Name.Humanize();
        }

        public string Name { get; }

        public async Task<SelectEditor.ReturnValue> Create(IForm parentForm, string title)
        {
            var factoryWizard = new FactoryWizard(parentForm, title, _editor, null, null, null);
            (object result, MethodModel model, MethodWizardAttribute attr) = await factoryWizard.RunConstructor(_type);
            return new SelectEditor.ReturnValue(new ValueModel { Value = result, Initializer = model }, model == null);
        }
    }
}