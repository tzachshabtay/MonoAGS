using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AGS.API;
using AGS.Engine;
using GuiLabs.Undo;

namespace AGS.Editor
{
    public class MethodWizard
    {
        private readonly MethodBase _method;
        private readonly IRenderLayer _layer;
        private readonly HashSet<string> _hideProperties;
        private readonly Dictionary<string, object> _overrideDefaults;
        private readonly TaskCompletionSource<Dictionary<string, object>> _taskCompletionSource;
        private readonly Action<IPanel> _addUiExternal;
        private readonly AGSEditor _editor;
        private IForm _form;
        private IModalWindowComponent _modal;
        private InspectorPanel _inspector;
        public const float MARGIN_HORIZONTAL = 30f;
        private const float MARGIN_VERTICAL = 20f;
        private const float WIDTH = 500f;
        private readonly Func<Dictionary<string, object>, Task<bool>> _validate;
        private readonly IForm _parentForm;
        private readonly string _idSuffix;
        private readonly string _title;
        static int _index = 1;

        public MethodWizard(IForm parentForm, string title, MethodBase method, HashSet<string> hideProperties, Dictionary<string, object> overrideDefaults, 
                            Action<IPanel> addUiExternal, AGSEditor editor, Func<Dictionary<string, object>, Task<bool>> validate)
        {
            _parentForm = parentForm;
            _title = title;
            _method = method;
            _idSuffix = $"_{_method.DeclaringType}_{_method.Name}_{_index++}";
            _editor = editor;
            _validate = validate;
            _layer = new AGSRenderLayer(AGSLayers.UI.Z - 1);
            _hideProperties = hideProperties;
            _overrideDefaults = overrideDefaults;
            _addUiExternal = addUiExternal;
            _taskCompletionSource = new TaskCompletionSource<Dictionary<string, object>>();
        }

        public void Load()
        {
            float center = _editor.ToEditorResolution(_editor.Game.Settings.VirtualResolution.Width / 2f, 0f, null).x;
            var factory = _editor.Editor.Factory;
            var title = _parentForm == null ? _title : $"{_parentForm.Header.Text}->{_title}";
            _form = factory.UI.GetForm($"MethodWizardPanel{_idSuffix}", title, 600f, 30f, 400f, -1000f, 100f, addToUi: false);

            var host = new AGSComponentHost(_editor.GameResolver);
            host.Init(_form.Contents, typeof(AGSComponentHost));
            _modal = host.AddComponent<IModalWindowComponent>();
            _modal.GrabFocus();
            var box = _form.Contents.AddComponent<IBoundingBoxWithChildrenComponent>();
            box.IncludeSelf = false;

            _form.Visible = false;
            setupForm(_form.Contents, factory);
            setupForm(_form.Header, factory);

            var inspectorParent = factory.UI.GetPanel($"WizardInspectorParentPanel{_idSuffix}", WIDTH, 300f, MARGIN_HORIZONTAL, 0f, _form.Contents);
            inspectorParent.Tint = Colors.Transparent;
            inspectorParent.Pivot = (0f, 1f);

            _inspector = new InspectorPanel(_editor, _layer, new ActionManager(), $"Wizard{_idSuffix}");
            _inspector.Load(inspectorParent, _form);
            _inspector.Inspector.SortValues = false;

            var methodDescriptor = new MethodTypeDescriptor(_method, _hideProperties, _overrideDefaults);
            _inspector.Show(methodDescriptor);

            _addUiExternal?.Invoke(_form.Contents);
            addButtons();

            var layout = _form.Contents.AddComponent<IStackLayoutComponent>();
            layout.AbsoluteSpacing = -30f;
            layout.LayoutAfterCrop = true;

            box.OnBoundingBoxWithChildrenChanged.Subscribe(() =>
            {
                layout.StartLocation = box.BoundingBoxWithChildren.Height + MARGIN_VERTICAL;
                _form.Contents.BaseSize = (_form.Contents.BaseSize.Width, box.BoundingBoxWithChildren.Height + MARGIN_VERTICAL * 2f);
                _form.Width = box.BoundingBoxWithChildren.Width + MARGIN_HORIZONTAL * 2f;
                _form.X = center - _form.Contents.BaseSize.Width / 2f;
            });

            layout.StartLayout();
        }

        public async Task<Dictionary<string, object>> ShowAsync()
        {
            var dialog = _parentForm;
            if (dialog != null) dialog.Visible = false;
            _form.Visible = true;
            var result = await _taskCompletionSource.Task;
            if (dialog != null) dialog.Visible = true;
            return result;
        }

        private void setupForm(IObject formPart, IGameFactory factory)
        {
            formPart.RenderLayer = _layer;
            formPart.Tint = GameViewColors.Panel;
            formPart.Border = factory.Graphics.Borders.SolidColor(GameViewColors.Border, 3f);
            _editor.Editor.State.UI.Add(formPart);
        }

        private void addButtons()
        {
            var factory = _editor.Editor.Factory;
            var font = _editor.Editor.Settings.Defaults.TextFont;
            var border = factory.Graphics.Borders.SolidColor(GameViewColors.Border, 2f);
            var idleConfig = new AGSTextConfig(GameViewColors.TextBrush, alignment: Alignment.MiddleCenter, font: font);
            var hoveredConfig = new AGSTextConfig(GameViewColors.HoveredTextBrush, alignment: Alignment.MiddleCenter, font: font);
            var idle = new ButtonAnimation(border, idleConfig, GameViewColors.Button);
            var hovered = new ButtonAnimation(border, hoveredConfig, GameViewColors.Button);
            var pushed = new ButtonAnimation(factory.Graphics.Borders.SolidColor(Colors.Black, 2f), idleConfig, GameViewColors.Button);

            var buttonsPanel = factory.UI.GetPanel($"MethodWizardButtonsPanel{_idSuffix}", WIDTH, 20f, MARGIN_HORIZONTAL, 50f, _form.Contents);
            buttonsPanel.Tint = Colors.Transparent;

            var layout = buttonsPanel.AddComponent<IStackLayoutComponent>();
            layout.Direction = LayoutDirection.Horizontal;
            layout.CenterLayout = true;
            layout.RelativeSpacing = 1f;
            layout.AbsoluteSpacing = 40f;
            layout.StartLocation = WIDTH / 2f;

            const float buttonWidth = 80f;
            var okButton = factory.UI.GetButton($"MethodWizardOkButton{_idSuffix}", idle, hovered, pushed, 0f, 0f, buttonsPanel, "OK", width: buttonWidth, height: 20f);
            okButton.MouseClicked.Subscribe(async () =>
            {
                Dictionary<string, object> map = new Dictionary<string, object>();
                foreach (var param in _inspector.Inspector.Properties.SelectMany(p => p.Value))
                {
                    map[param.Name] = param.Value;
                }
                if (_validate != null)
                {
                    if (!await _validate(map)) return;
                }

                _modal?.LoseFocus();
                _form.Header.DestroyWithChildren(_editor.Editor.State);
                _taskCompletionSource.TrySetResult(map);
            });

            var cancelButton = factory.UI.GetButton($"MethodWizardCancelButton{_idSuffix}", idle, hovered, pushed, 0f, 0f, buttonsPanel, "Cancel", width: buttonWidth, height: 20f);
            cancelButton.MouseClicked.Subscribe(() =>
            {
                _modal?.LoseFocus();
                _form.Header.DestroyWithChildren(_editor.Editor.State);
                _taskCompletionSource.TrySetResult(null);
            });

            layout.StartLayout();
        }
    }
}