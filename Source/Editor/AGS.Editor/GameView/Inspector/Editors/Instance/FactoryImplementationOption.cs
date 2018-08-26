using System;
using System.Threading.Tasks;
using AGS.API;

namespace AGS.Editor
{
    public class FactoryImplementationOption : IImplementationOption
    {
        private readonly object _factory;
        private readonly AGSEditor _editor;

        public FactoryImplementationOption(HasFactoryAttribute attr, AGSEditor editor)
        {
            _editor = editor;
            _factory = FactoryProvider.GetFactory(attr.FactoryType, editor.Game);
            Name = attr.MethodName;
        }

        public string Name { get; }

        public async Task<SelectEditor.ReturnValue> Create(IForm parentForm, string title)
        {
            var factoryWizard = new FactoryWizard(parentForm, title, _editor, null, null, null);
            (object result, MethodModel model, MethodWizardAttribute attr) = await factoryWizard.RunMethod(_factory, Name);
            return new SelectEditor.ReturnValue(result, model == null);
        }
    }
}