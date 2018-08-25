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
            switch (attr.FactoryType)
            {
                case nameof(IBorderFactory):
                    _factory = editor.Game.Factory.Graphics.Borders;
                    break;
                case nameof(IBrushLoader):
                    _factory = editor.Game.Factory.Graphics.Brushes;
                    break;
                case nameof(IFontLoader):
                    _factory = editor.Game.Factory.Fonts;
                    break;
                default:
                    throw new NotSupportedException($"Not supported factory type: {attr.FactoryType}");
            }
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