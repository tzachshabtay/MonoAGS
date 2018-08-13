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
        private readonly MethodInfo _method;
        private readonly EditorProvider _editorProvider;
        private readonly IRenderLayer _layer;
        private readonly HashSet<string> _hideProperties;
        private readonly Dictionary<string, object> _overrideDefaults;
        private readonly TaskCompletionSource<bool> _taskCompletionSource;
        private readonly Action<IPanel> _addUiExternal;
        private readonly AGSEditor _editor;
        private IPanel _parent;
        private IModalWindowComponent _modal;
        private InspectorPanel _inspector;
        public const float MARGIN_HORIZONTAL = 30f;
        private const float MARGIN_VERTICAL = 20f;
        private const float WIDTH = 500f;

        public MethodWizard(MethodInfo method, HashSet<string> hideProperties, Dictionary<string, object> overrideDefaults, 
                            Action<IPanel> addUiExternal, EditorProvider editorProvider, AGSEditor editor)
        {
            _method = method;
            _editor = editor;
            _editorProvider = editorProvider;
            _layer = new AGSRenderLayer(AGSLayers.UI.Z - 1);
            _hideProperties = hideProperties;
            _overrideDefaults = overrideDefaults;
            _addUiExternal = addUiExternal;
            _taskCompletionSource = new TaskCompletionSource<bool>();
        }

        public void Load()
        {
            float center = _editor.ToEditorResolution(_editor.Game.Settings.VirtualResolution.Width / 2f, 0f, null).x;
            var factory = _editor.Editor.Factory;
            _parent = factory.UI.GetPanel($"MethodWizardPanel_{_method.Name}", 600f, 400f, -1000f, 100f, addToUi: false);
            _parent.RenderLayer = _layer;
            _parent.Tint = GameViewColors.Panel;
            _parent.Border = factory.Graphics.Borders.SolidColor(GameViewColors.Border, 3f);
            var host = new AGSComponentHost(_editor.GameResolver);
            host.Init(_parent, typeof(AGSComponentHost));
            _modal = host.AddComponent<IModalWindowComponent>();
            _modal.GrabFocus();
            var box = _parent.AddComponent<IBoundingBoxWithChildrenComponent>();
            box.IncludeSelf = false;

            _parent.Visible = false;
            _editor.Editor.State.UI.Add(_parent);

            var inspectorParent = factory.UI.GetPanel("WizardInspectorParentPanel", WIDTH, 300f, MARGIN_HORIZONTAL, 0f, _parent);
            inspectorParent.Tint = Colors.Transparent;
            inspectorParent.Pivot = (0f, 1f);

            _inspector = new InspectorPanel(_editor, _layer, new ActionManager(), "Wizard");
            _inspector.Load(inspectorParent);
            _inspector.Inspector.SortValues = false;

            var methodDescriptor = new MethodTypeDescriptor(_method, _hideProperties, _overrideDefaults);
            _inspector.Show(methodDescriptor);

            _addUiExternal?.Invoke(_parent);
            addButtons();

            var layout = _parent.AddComponent<IStackLayoutComponent>();
            layout.AbsoluteSpacing = -30f;
            layout.LayoutAfterCrop = true;

            box.OnBoundingBoxWithChildrenChanged.Subscribe(() =>
            {
                layout.StartLocation = box.BoundingBoxWithChildren.Height + MARGIN_VERTICAL;
                _parent.BaseSize = (box.BoundingBoxWithChildren.Width + MARGIN_HORIZONTAL * 2f, box.BoundingBoxWithChildren.Height + MARGIN_VERTICAL * 2f);
                _parent.X = center - _parent.BaseSize.Width / 2f;
            });

            layout.StartLayout();
        }

        public async Task<Dictionary<string, object>> ShowAsync()
        {
            _parent.Visible = true;
            if (!await _taskCompletionSource.Task)
                return null;
            Dictionary<string, object> map = new Dictionary<string, object>();
            foreach (var param in _inspector.Inspector.Properties.SelectMany(p => p.Value))
            {
                map[param.Name] = param.GetValue();
            }
            return map;
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

            var buttonsPanel = factory.UI.GetPanel("MethodWizardButtonsPanel", WIDTH, 20f, MARGIN_HORIZONTAL, 50f, _parent);
            buttonsPanel.Tint = Colors.Transparent;

            var layout = buttonsPanel.AddComponent<IStackLayoutComponent>();
            layout.Direction = LayoutDirection.Horizontal;
            layout.CenterLayout = true;
            layout.RelativeSpacing = 1f;
            layout.AbsoluteSpacing = 40f;
            layout.StartLocation = WIDTH / 2f;

            const float buttonWidth = 80f;
            var okButton = factory.UI.GetButton("MethodWizardOkButton", idle, hovered, pushed, 0f, 0f, buttonsPanel, "OK", width: buttonWidth, height: 20f);
            okButton.MouseClicked.Subscribe(() =>
            {
                _modal?.LoseFocus();
                _parent.DestroyWithChildren(_editor.Editor.State);
                _taskCompletionSource.TrySetResult(true);
            });

            var cancelButton = factory.UI.GetButton("MethodWizardCancelButton", idle, hovered, pushed, 0f, 0f, buttonsPanel, "Cancel", width: buttonWidth, height: 20f);
            cancelButton.MouseClicked.Subscribe(() =>
            {
                _modal?.LoseFocus();
                _parent.DestroyWithChildren(_editor.Editor.State);
                _taskCompletionSource.TrySetResult(false);
            });

            layout.StartLayout();
        }
    }
}