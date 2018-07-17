using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using AGS.API;
using AGS.Engine;

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
        private readonly List<MethodParam> _params;
        private readonly AGSEditor _editor;
        private TreeTableLayout _layout;
        private IPanel _parent;
        private IModalWindowComponent _modal;

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
            _params = new List<MethodParam>();
        }

        public void Load()
        {
            float center = _editor.ToEditorResolution(_editor.Game.Settings.VirtualResolution.Width / 2f, 0f, null).x;
            _parent = _editor.Editor.Factory.UI.GetPanel($"MethodWizardPanel_{_method.Name}", 600f, 400f, -1000f, 100f, addToUi: false);
            _parent.RenderLayer = _layer;
            _parent.Tint = GameViewColors.Panel;
            _parent.Border = AGSBorders.SolidColor(GameViewColors.Border, 3f);
            var host = new AGSComponentHost(_editor.GameResolver);
            host.Init(_parent, typeof(AGSComponentHost));
            _modal = host.AddComponent<IModalWindowComponent>();
            _modal.GrabFocus();
            var box = _parent.AddComponent<IBoundingBoxWithChildrenComponent>();
            box.IncludeSelf = false;

            _parent.Visible = false;
            _editor.Editor.State.UI.Add(_parent);

            _layout = new TreeTableLayout(_editor.Editor.Events);
            _layout.ColumnPadding = 15f;

            var parameters = _method.GetParameters();
            foreach (var parameter in parameters)
            {
                if (_hideProperties.Contains(parameter.Name)) continue;
                var editor = _editorProvider.GetEditor(parameter.ParameterType, null);
                _overrideDefaults.TryGetValue(parameter.Name, out var overrideDefault);
                var param = new MethodParam(parameter, null, overrideDefault);
                _params.Add(param);
                editor.AddEditorUI($"MethodParam_{parameter.Name}", getView(param), param);
            }

            _addUiExternal?.Invoke(_parent);
            addButtons();

            var layout = _parent.AddComponent<IStackLayoutComponent>();
            layout.AbsoluteSpacing = -10f;

            box.OnBoundingBoxWithChildrenChanged.Subscribe(() =>
            {
                layout.StartLocation = box.BoundingBoxWithChildren.Height - 20f;
                _parent.BaseSize = (box.BoundingBoxWithChildren.Width  + 10f, box.BoundingBoxWithChildren.Height + 10f);
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
            foreach (var param in _params)
            {
                map[param.Name] = param.GetValue();
            }
            return map;
        }

        private ITreeNodeView getView(MethodParam param)
        {
            const float labelWidth = 20f;
            var factory = _editor.Editor.Factory;
            var horizontalPanel = factory.UI.GetPanel($"MethodParamPanel_{param.Name}", 50f, 30f, 50f, 0f, _parent);
            var font = _editor.Editor.Settings.Defaults.TextFont;
            var label = factory.UI.GetLabel($"MethodParamLabel_{param.Name}", param.Name, 0f, 0f, labelWidth, 1f, horizontalPanel,
                                            new AGSTextConfig(paddingTop: 0f, paddingBottom: 0f, autoFit: AutoFit.LabelShouldFitText, font: font));
            label.RenderLayer = _layer;
            label.Tint = Colors.Transparent;
            label.TextBackgroundVisible = false;
            label.Enabled = true;

            horizontalPanel.RenderLayer = _layer;
            horizontalPanel.Tint = Colors.Transparent;
            horizontalPanel.AddComponent<IBoundingBoxWithChildrenComponent>();

            var layout = horizontalPanel.AddComponent<ITreeTableRowLayoutComponent>();
            layout.Table = _layout;

            return new AGSTreeNodeView(label, null, null, null, horizontalPanel);
        }

        private void addButtons()
        {
            var factory = _editor.Editor.Factory;
            var font = _editor.Editor.Settings.Defaults.TextFont;
            var border = AGSBorders.SolidColor(GameViewColors.Border, 2f);
            var idleConfig = new AGSTextConfig(GameViewColors.TextBrush, alignment: Alignment.MiddleCenter, font: font);
            var hoveredConfig = new AGSTextConfig(GameViewColors.HoveredTextBrush, alignment: Alignment.MiddleCenter, font: font);
            var idle = new ButtonAnimation(border, idleConfig, GameViewColors.Button);
            var hovered = new ButtonAnimation(border, hoveredConfig, GameViewColors.Button);
            var pushed = new ButtonAnimation(AGSBorders.SolidColor(Colors.Black, 2f), idleConfig, GameViewColors.Button);

            var buttonsPanel = factory.UI.GetPanel("MethodWizardButtonsPanel", 300f, 20f, 0f, 50f, _parent);
            buttonsPanel.Tint = Colors.Transparent;

            var okButton = factory.UI.GetButton("MethodWizardOkButton", idle, hovered, pushed, 50f, 0f, buttonsPanel, "OK", width: 80f, height: 20f);
            okButton.MouseClicked.Subscribe(() =>
            {
                _modal?.LoseFocus();
                _parent.DestroyWithChildren(_editor.Editor.State);
                _taskCompletionSource.TrySetResult(true);
            });

            var cancelButton = factory.UI.GetButton("MethodWizardCancelButton", idle, hovered, pushed, 170f, 0f, buttonsPanel, "Cancel", width: 80f, height: 20f);
            cancelButton.MouseClicked.Subscribe(() =>
            {
                _modal?.LoseFocus();
                _parent.DestroyWithChildren(_editor.Editor.State);
                _taskCompletionSource.TrySetResult(false);
            });
        }
    }
}