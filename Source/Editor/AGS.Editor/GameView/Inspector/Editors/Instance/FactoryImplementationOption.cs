using System;
using System.Threading.Tasks;
using AGS.API;
using Humanizer;

namespace AGS.Editor
{
    public class FactoryImplementationOption : IImplementationOption
    {
        private readonly object _factory;
        private readonly AGSEditor _editor;
        private readonly string _methodName;

        public FactoryImplementationOption(HasFactoryAttribute attr, AGSEditor editor)
        {
            _editor = editor;
            _factory = FactoryProvider.GetFactory(attr.FactoryType, editor.Game);
            _methodName = attr.MethodName;
            Name = attr.DisplayName ?? attr.MethodName.Humanize();
        }

        public string Name { get; }

        public async Task<SelectEditor.ReturnValue> Create(IForm parentForm, string title)
        {
            var factoryWizard = new FactoryWizard(parentForm, title, _editor, null, null, null);
            (object result, MethodModel model, MethodWizardAttribute attr) = await factoryWizard.RunMethod(_factory, _methodName);
            return new SelectEditor.ReturnValue(new ValueModel(result, model), model == null);
        }
    }
}