using System;
using System.Threading.Tasks;
using AGS.API;

namespace AGS.Editor
{
    public class FactoryImplementationOption : IImplementationOption
    {
        private readonly string _methodName;
        private readonly object _factory;
        private readonly AGSEditor _editor;
        private readonly IObject _parentDialog;

        public FactoryImplementationOption(HasFactoryAttribute attr, AGSEditor editor, IObject parentDialog)
        {
            _parentDialog = parentDialog;
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
            var factoryWizard = new FactoryWizard(_parentDialog, _editor, null, null, null);
            (object result, MethodModel model, MethodWizardAttribute attr) = await factoryWizard.RunMethod(_factory, _methodName);
            return new SelectEditor.ReturnValue(result, model == null);
        }
    }
}