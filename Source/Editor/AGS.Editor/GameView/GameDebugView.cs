﻿using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using AGS.API;
using AGS.Engine;
using GuiLabs.Undo;

namespace AGS.Editor
{
    public class GameDebugView
    {
        private readonly IRenderLayer _layer;
        private readonly AGSEditor _editor;
        private readonly GameDebugDisplayList _displayList;
        private readonly InspectorPanel _inspector;
        private readonly IInput _input;
        private readonly ActionManager _actions;
        private readonly GameToolbar _toolbar;
        private readonly Size _resolution = new Size(1800, 1200);
        private const string _panelId = "Game Debug Tree Panel";
        private IPanel _panel;
        private ISplitPanelComponent _splitPanel;
        private IDebugTab _currentTab;
        private IButton _panesButton;

        public GameDebugView(AGSEditor editor, KeyboardBindings keyboardBindings, ActionManager actions, GameToolbar toolbar)
        {
            _toolbar = toolbar;
            _actions = actions;
            _editor = editor;
            _layer = new AGSRenderLayer(AGSLayers.UI.Z - 1, independentResolution: _resolution);
            _inspector = new InspectorPanel(editor, _layer, actions);
            Tree = new GameDebugTree(editor, _layer, _inspector);
            _displayList = new GameDebugDisplayList(editor.Editor, editor.Game, _layer);
            _input = editor.Editor.Input;
            keyboardBindings.OnKeyboardShortcutPressed.Subscribe(onShortcutKeyPressed);
        }

        public bool Visible => _panel.Visible;

        public GameDebugTree Tree { get; }

        public void Load()
        {
            const float headerHeight = 50f;
            const float borderWidth = 3f;
            IGameFactory factory = _editor.Editor.Factory;
            _panel = factory.UI.GetPanel(_panelId, _resolution.Width / 4f, _resolution.Height,
                                                     1f, _resolution.Height / 2f);
            _panel.Pivot = new PointF(0f, 0.5f);
            _panel.Visible = false;
            _panel.Tint = GameViewColors.Panel;
            _panel.Border = factory.Graphics.Borders.SolidColor(GameViewColors.Border, borderWidth, hasRoundCorners: true);
            _panel.RenderLayer = _layer;
            _panel.ClickThrough = false;
            _editor.Editor.State.FocusedUI.CannotLoseFocus.Add(_panelId);

            var headerLabel = factory.UI.GetLabel("GameDebugTreeLabel", "Game View", _panel.Width, headerHeight, 0f, _panel.Height - headerHeight,
                _panel, factory.Fonts.GetTextConfig(alignment: Alignment.MiddleCenter, autoFit: AutoFit.TextShouldFitLabel));
            headerLabel.Tint = Colors.Transparent;
            headerLabel.Border = factory.Graphics.Borders.SolidColor(GameViewColors.Border, borderWidth, hasRoundCorners: true);
            headerLabel.RenderLayer = _layer;

            var xButton = factory.UI.GetButton("GameDebugTreeCloseButton", (IAnimation)null, null, null, 0f, _panel.Height - headerHeight + 5f, _panel, "X",
                                               factory.Fonts.GetTextConfig(factory.Graphics.Brushes.LoadSolidBrush(Colors.Red),
                                                                 autoFit: AutoFit.TextShouldFitLabel, alignment: Alignment.MiddleCenter),
                                                                 width: 40f, height: 40f);
            xButton.Pivot = new PointF();
            xButton.RenderLayer = _layer;
            xButton.Tint = Colors.Transparent;
            xButton.MouseEnter.Subscribe(_ => xButton.TextConfig = AGSTextConfig.ChangeColor(xButton.TextConfig, GameViewColors.HoveredText, GameViewColors.HoveredText, 0.3f));
            xButton.MouseLeave.Subscribe(_ => xButton.TextConfig = AGSTextConfig.ChangeColor(xButton.TextConfig, Colors.Red, Colors.Transparent, 0f));
            xButton.MouseClicked.Subscribe(_ => Hide());

            _panesButton = factory.UI.GetButton("GameDebugViewPanesButton", (IAnimation)null, null, null, _panel.Width, xButton.Y, _panel, "Display List",
                                                   factory.Fonts.GetTextConfig(autoFit: AutoFit.TextShouldFitLabel, alignment: Alignment.MiddleRight),
                                                   width: 120f, height: 40f);
            _panesButton.Pivot = new PointF(1f, 0f);
            _panesButton.RenderLayer = _layer;
            _panesButton.Tint = GameViewColors.Button;
            _panesButton.MouseEnter.Subscribe(_ => _panesButton.TextConfig = AGSTextConfig.ChangeColor(xButton.TextConfig, GameViewColors.HoveredText, GameViewColors.HoveredText, 0.3f));
            _panesButton.MouseLeave.Subscribe(_ => _panesButton.TextConfig = AGSTextConfig.ChangeColor(xButton.TextConfig, GameViewColors.Text, Colors.Transparent, 0f));
            _panesButton.MouseClicked.SubscribeToAsync(onPaneSwitch);

            var parentPanelHeight = _panel.Height - headerHeight;
            var parentPanel = factory.UI.GetPanel("GameDebugParentPanel", _panel.Width, parentPanelHeight, 0f, parentPanelHeight, _panel);
            parentPanel.Pivot = new PointF(0f, 1f);
            parentPanel.Tint = Colors.Transparent;
            parentPanel.RenderLayer = _layer;

            var topPanel = factory.UI.GetPanel("GameDebugTopPanel", _panel.Width, parentPanelHeight / 2f, 0f, parentPanelHeight / 2f, parentPanel);
            topPanel.Pivot = new PointF(0f, 0f);
            topPanel.Tint = Colors.Transparent;
            topPanel.RenderLayer = _layer;

            var bottomPanel = factory.UI.GetPanel("GameDebugBottomPanel", _panel.Width, parentPanelHeight / 2f, 0f, parentPanelHeight / 2f, parentPanel);
            bottomPanel.Pivot = new PointF(0f, 1f);
            bottomPanel.Tint = Colors.Transparent;
            bottomPanel.RenderLayer = _layer;

            Tree.Load(topPanel);
            _displayList.Load(topPanel);
            _inspector.Load(bottomPanel, null);
            _currentTab = Tree;
            _splitPanel = parentPanel.AddComponent<ISplitPanelComponent>();
            _splitPanel.TopPanel = topPanel;
            _splitPanel.BottomPanel = bottomPanel;

            var horizSplit = _panel.AddComponent<ISplitPanelComponent>();
            horizSplit.IsHorizontal = true;
            horizSplit.TopPanel = _panel;

            _panel.GetComponent<IScaleComponent>().PropertyChanged += (_, args) =>
            {
                if (args.PropertyName != nameof(IScaleComponent.Width)) return;
                _panesButton.X = _panel.Width;
                headerLabel.LabelRenderSize = new SizeF(_panel.Width, headerLabel.LabelRenderSize.Height);
                parentPanel.BaseSize = new SizeF(_panel.Width, parentPanel.Height);
                topPanel.BaseSize = new SizeF(_panel.Width, topPanel.Height);
                bottomPanel.BaseSize = new SizeF(_panel.Width, bottomPanel.Height);
                _currentTab.Resize();
                _inspector.Resize();
                resizeGameWindow();
            };

            _editor.Editor.Events.OnRepeatedlyExecute.Subscribe(onRepeatedlyExecute);
            _editor.Editor.Events.OnScreenResize.Subscribe(onScreenResize);
        }

        public Task Show()
        {
            _panel.Visible = true;
            resizeGameWindow();
            return _currentTab.Show();
        }

        public void Hide()
        {
            _panel.Visible = false;
            _currentTab.Hide();
            resizeGameWindow();
        }

        private void resizeGameWindow()
        {
            int height = _editor.Editor.Settings.WindowSize.Height - 100;
            if (_panel.Visible)
            {
                float panelWidth = MathUtils.Lerp(0f, 0f, _resolution.Width, _editor.Editor.Settings.WindowSize.Width, _panel.Width);
                AGSEditor.Platform.SetHostedGameWindow(new Rectangle((int)Math.Round(panelWidth), 0, (int)Math.Round(_editor.Editor.Settings.WindowSize.Width - panelWidth), height));
            }
            else
            {
                AGSEditor.Platform.SetHostedGameWindow(new Rectangle(0, 0, _editor.Editor.Settings.WindowSize.Width, height));
            }
        }

        private void onScreenResize() => resizeGameWindow();

        private Task onPaneSwitch(MouseButtonEventArgs args)
        {
            _currentTab.Hide();
            _currentTab = (_currentTab == Tree) ? (IDebugTab)_displayList : Tree;
            _panesButton.Text = _currentTab == Tree ? "Display List" : "Scene Tree";
            _currentTab.Resize();
            return _currentTab.Show();
        }

        private async void onShortcutKeyPressed(string action)
        {
            if (action == KeyboardBindings.BreakDebugger)
            {
                await BreakDebuggerForm.Show(_editor);
                return;
            }
            if (action == KeyboardBindings.FindObject)
            {
                await FindObjectForm.Show(_editor);
                return; 
            }
            if (!_panel?.Visible ?? false) return;

            if (action == KeyboardBindings.Undo)
            {
                _inspector?.Inspector?.Undo();
            }
            else if (action == KeyboardBindings.Redo)
            {
                _inspector?.Inspector?.Redo();
            }
            else if (action == KeyboardBindings.Save)
            {
                string baseFolder = Path.GetDirectoryName(_editor.Project.AGSProjectPath);
                CSharpCodeGeneartor codeGeneartor = new CSharpCodeGeneartor(_editor.Project.Model);
                _editor.Project.Model.GenerateCode(baseFolder, codeGeneartor);
                _editor.Project.Model.Save(AGSEditor.Platform, _editor.Editor, baseFolder);
            }
        }

        private void moveEntity(IEntity entity, float xOffset, float yOffset)
        {
            var translate = entity.GetComponent<ITranslateComponent>();
            if (translate == null) return;
            if (_input.IsKeyDown(Key.AltLeft) || _input.IsKeyDown(Key.AltRight))
            {
                xOffset /= 10f;
                yOffset /= 10f;
            }
            if (!MathUtils.FloatEquals(xOffset, 0f))
            {
                PropertyInfo prop = translate.GetType().GetProperty(nameof(ITranslateComponent.X));
                PropertyAction action = new PropertyAction(new InspectorProperty(translate, null, nameof(ITranslateComponent.X), prop), translate.X + xOffset, _editor.Project.Model);
                _actions.RecordAction(action);
            }
            if (!MathUtils.FloatEquals(yOffset, 0f))
            {
                PropertyInfo prop = translate.GetType().GetProperty(nameof(ITranslateComponent.Y));
                PropertyAction action = new PropertyAction(new InspectorProperty(translate, null, nameof(ITranslateComponent.Y), prop), translate.Y + yOffset, _editor.Project.Model);
                _actions.RecordAction(action);
            }
        }

        private void rotateEntity(IEntity entity, float angleOffset)
        {
            var rotate = entity.GetComponent<IRotateComponent>();
            if (rotate == null) return;
            if (_input.IsKeyDown(Key.AltLeft) || _input.IsKeyDown(Key.AltRight))
            {
                angleOffset /= 10f;
            }
            if (MathUtils.FloatEquals(angleOffset, 0f)) return;
            PropertyInfo prop = rotate.GetType().GetProperty(nameof(IRotateComponent.Angle));
            PropertyAction action = new PropertyAction(new InspectorProperty(rotate, null, nameof(IRotateComponent.Angle), prop), rotate.Angle + angleOffset, _editor.Project.Model);
            _actions.RecordAction(action);
        }

        private void scaleEntity(IEntity entity, float scaleOffset)
        {
            var scale = entity.GetComponent<IScaleComponent>();
            if (scale == null) return;
            if (_input.IsKeyDown(Key.AltLeft) || _input.IsKeyDown(Key.AltRight))
            {
                scaleOffset /= 10f;
            }
            PropertyInfo prop = scale.GetType().GetProperty(nameof(IScaleComponent.Scale));
            PropertyAction action = new PropertyAction(new InspectorProperty(scale, null, nameof(IScaleComponent.Scale), prop), new PointF(scale.ScaleX + scaleOffset, scale.ScaleY + scaleOffset), _editor.Project.Model);
            _actions.RecordAction(action);
        }

        private void onRepeatedlyExecute(IRepeatedlyExecuteEventArgs args)
        {
            respondToKeyboard();
        }

        private void respondToKeyboard()
        {
            if (_editor.Editor.State.FocusedUI.HasKeyboardFocus != null || !_toolbar.IsPaused) return;
            var entity = _inspector.Inspector?.SelectedObject as IEntity;
            if (entity == null) return;

            if (_input.IsKeyDown(Key.Down)) moveEntity(entity, 0f, -1f);
            else if (_input.IsKeyDown(Key.Up)) moveEntity(entity, 0f, 1f);

            if (_input.IsKeyDown(Key.Left)) moveEntity(entity, -1f, 0f);
            else if (_input.IsKeyDown(Key.Right)) moveEntity(entity, 1f, 0f);

            if (_input.IsKeyDown(Key.BracketLeft)) rotateEntity(entity, -1f);
            else if (_input.IsKeyDown(Key.BracketRight)) rotateEntity(entity, 1f);

            if (_input.IsKeyDown(Key.Plus)) scaleEntity(entity, 0.1f);
            else if (_input.IsKeyDown(Key.Minus)) scaleEntity(entity, -0.1f);
        }
    }
}
