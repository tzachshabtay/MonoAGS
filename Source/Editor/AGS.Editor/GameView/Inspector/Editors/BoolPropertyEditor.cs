using AGS.API;
using AGS.Engine;
using GuiLabs.Undo;

namespace AGS.Editor
{
    public class BoolPropertyEditor : IInspectorPropertyEditor
    {
        private readonly IGameFactory _factory;
        private readonly ActionManager _actions;
        private readonly StateModel _model;
        private IProperty _property;
        private ICheckboxComponent _checkbox;

        public BoolPropertyEditor(IGameFactory factory, ActionManager actions, StateModel model)
        {
            _factory = factory;
            _actions = actions;
            _model = model;
        }

        public static ICheckBox CreateCheckbox(IUIControl label, IGameFactory factory, string id)
        {
            return createCheckbox(label.TreeNode.Parent, factory, id, label.X, label.Y, "", FontIcons.Square, FontIcons.CheckSquare);
        }

        public static ICheckBox CreateRadioButton(IObject parent, IGameFactory factory, string id, float x, float y, string text)
        {
            var checkbox = createCheckbox(parent, factory, id, x, y, text, FontIcons.RadioUnchecked, FontIcons.RadioChecked);
            var config = AGSTextConfig.Clone(checkbox.TextLabel.TextConfig);
            config.Alignment = Alignment.BottomLeft;
            checkbox.TextLabel.TextConfig = config;
            return checkbox;
        }

        private static ICheckBox createCheckbox(IObject parent, IGameFactory factory, string id, 
                                                float x, float y, string text, string @unchecked, string @checked)
        {
            var idleConfig = factory.Fonts.GetTextConfig(font: FontIcons.Font, brush: factory.Graphics.Brushes.LoadSolidBrush(Colors.WhiteSmoke), paddingTop: 0f, paddingLeft: 0f, paddingRight: 0f, paddingBottom: 0f, alignment: Alignment.MiddleCenter);
            var hoverConfig = factory.Fonts.GetTextConfig(font: FontIcons.Font, brush: factory.Graphics.Brushes.LoadSolidBrush(Colors.Yellow), paddingTop: 0f, paddingLeft: 0f, paddingRight: 0f, paddingBottom: 0f, alignment: Alignment.MiddleCenter);
            var idleAnimation = new ButtonAnimation(null, idleConfig, null);
            var hoverAnimation = new ButtonAnimation(null, hoverConfig, null);
            var checkbox = factory.UI.GetCheckBox(id, idleAnimation, hoverAnimation, idleAnimation, hoverAnimation,
                                                 x, y, parent, text, width: 20f, height: 25f);
            var textComponent = checkbox.AddComponent<ITextComponent>();
            textComponent.TextConfig = idleConfig;
            textComponent.Text = @unchecked;
            textComponent.LabelRenderSize = checkbox.BaseSize;
            checkbox.OnCheckChanged.Subscribe(() =>
            {
                textComponent.Text = checkbox.Checked ? @checked : @unchecked;
            });
            return checkbox;
        }

        public void AddEditorUI(string id, ITreeNodeView view, IProperty property)
        {
            _property = property;
			var label = view.TreeItem;
            var checkbox = CreateCheckbox(label, _factory, id);
            _checkbox = checkbox;
            checkbox.Checked = bool.Parse(property.ValueString);
            checkbox.OnCheckChanged.Subscribe(args => 
            {
                if (_actions.ActionIsExecuting) return;
                _actions.RecordAction(new PropertyAction(property, args.Checked, _model));
            });
        }

        public void RefreshUI()
        {
            if (_checkbox == null) return;
            _checkbox.Checked = bool.Parse(_property.ValueString);
        }
    }
}