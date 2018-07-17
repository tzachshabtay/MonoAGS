using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AGS.API;
using AGS.Engine;
using Autofac;
using GuiLabs.Undo;

namespace AGS.Editor
{
    public class CanvasMenu
    {
        private readonly AGSEditor _editor;
        private Menu _topMenu;
        private static int _lastId;
        private ICheckboxComponent _roomButton, _guiButton;
        private IRadioGroup _targetRadioGroup;

        public CanvasMenu(AGSEditor editor)
        {
            _editor = editor;
        }

        public void Load()
        {
            Action noop = () => {};
            Menu guisMenu = new Menu("GuisMenu", 180f,
                                     new MenuItem("Button", showButtonWizard),
                                     new MenuItem("Label", noop),
                                     new MenuItem("ComboBox", noop),
                                     new MenuItem("TextBox", noop),
                                     new MenuItem("Inventory Window", noop),
                                     new MenuItem("Checkbox", noop),
                                     new MenuItem("Listbox", noop),
                                     new MenuItem("Panel", noop),
                                     new MenuItem("Slider", noop));
            Menu areasMenu = new Menu("AreasMenu", 150f,
                                      new MenuItem("Walkable Area", noop),
                                      new MenuItem("Walk-Behind", noop),
                                      new MenuItem("Scaling Area", noop),
                                      new MenuItem("Zoom Area", noop),
                                      new MenuItem("Empty Area", noop));
            Menu presetsMenu = new Menu("PresetsMenu", 100f,
                                        new MenuItem("Object", noop), 
                                        new MenuItem("Character", noop),
                                        new MenuItem("GUIs", guisMenu),
                                        new MenuItem("Areas", areasMenu));
            _topMenu = new Menu("CanvasMenu", 100f, new MenuItem("Create", presetsMenu));
            _topMenu.Load(_editor.Editor.Factory, _editor.Editor.Settings.Defaults);

            _editor.Editor.Input.MouseUp.Subscribe((MouseButtonEventArgs args) => 
            {
                if (args.Button == MouseButton.Right)
                {
                    _topMenu.Position = (args.MousePosition.XMainViewport, args.MousePosition.YMainViewport);
                    _topMenu.Visible = true;
                }
                else if (args.ClickedEntity == null) 
                {
                    _topMenu.Visible = false;
                } 
            });
        }

        private void showButtonWizard()
        {
            var method = typeof(IUIFactory).GetMethods().First(m => m.Name == nameof(IUIFactory.GetButton));
            Action<Dictionary<string, object>> setParameters = parameters =>
            {
                parameters["width"] = 25f;
                parameters["height"] = 25f;
                parameters["addToUi"] = false;
                parameters["hovered"] = get("hovered", parameters) ?? getDefaultHovered();
                parameters["idle"] = get("idle", parameters) ?? getDefaultIdle();
                parameters["pushed"] = get("pushed", parameters) ?? getDefaultPushed();
            };
            showWizard("button", true, _editor.Game.Factory.UI, method, setParameters, "parent", "addToUi", "config", "width", "height");
        }

        private ButtonAnimation getDefaultHovered() => new ButtonAnimation(getButtonBorder(), null, GameViewColors.HoveredButton);
        private ButtonAnimation getDefaultIdle() => new ButtonAnimation(getButtonBorder(), GameViewColors.ButtonTextConfig, GameViewColors.Button);
        private ButtonAnimation getDefaultPushed() => new ButtonAnimation(getButtonBorder(), null, GameViewColors.PushedButton);

        private IBorderStyle getButtonBorder()
        {
            return AGSBorders.SolidColor(_editor.GameResolver.Container.Resolve<IGLUtils>(),
                                         _editor.Game.Settings, Colors.Black, 1f);
        }

        private object get(string key, Dictionary<string, object> parameters) => parameters.TryGetValue(key, out var val) ? val : null;

        private async void showWizard(string name, bool isUi, object factory, MethodInfo method, Action<Dictionary<string, object>> setParameters, params string[] propertiesToHide)
        {
            _topMenu.Visible = false;
            HashSet<string> hideProperties = new HashSet<string>(propertiesToHide);
            Dictionary<string, object> overrideDefaults = new Dictionary<string, object>();
            var (x, y) = _editor.ToGameResolution(_topMenu.Position.x, _topMenu.Position.y, null);
            overrideDefaults["x"] = x;
            overrideDefaults["y"] = y;
            overrideDefaults["id"] = $"{name}{++_lastId}";
            var editorProvider = new EditorProvider(_editor.Editor.Factory, new ActionManager(), new StateModel(), _editor.Editor.State, _editor.Editor.Settings);
            var wizard = new MethodWizard(method, hideProperties, overrideDefaults, panel => addTargetForCreateUI(panel, isUi), editorProvider, _editor);
            wizard.Load();
            var parameters = await wizard.ShowAsync();
            if (parameters == null) return;
            setParameters(parameters);
            object entity = runMethod(method, factory, parameters);
            addNewEntity(entity, isUi);
        }

        private void addNewEntity(object entity, bool isUi)
        {
            if (isUi)
            {
                if (entity is IObject obj) _editor.Game.State.UI.Add(obj);
                else throw new Exception($"Unkown entity created: {entity?.GetType().Name ?? "null"}");
            }
            else
            {
                if (entity is IObject obj) _editor.Game.State.Room.Objects.Add(obj);
                else if (entity is IArea area) _editor.Game.State.Room.Areas.Add(area);
                else throw new Exception($"Unkown entity created: {entity?.GetType().Name ?? "null"}");
            }
        }

        private object runMethod(MethodInfo method, object factory, Dictionary<string, object> parameters)
        {
            var methodParams = method.GetParameters();
            object[] values = methodParams.Select(m => parameters.TryGetValue(m.Name, out object val) ?
                                                  val : MethodParam.GetDefaultValue(m.ParameterType)).ToArray();
            return method.Invoke(factory, values);
        }

        private void addTargetForCreateUI(IPanel panel, bool isUi)
        {
            var factory = _editor.Editor.Factory;
            var buttonsPanel = factory.UI.GetPanel("MethodWizardTargetPanel", 300f, 45f, 0f, 50f, panel);
            buttonsPanel.Tint = Colors.Transparent;

            var font = _editor.Editor.Settings.Defaults.TextFont;
            var labelConfig = new AGSTextConfig(font: factory.Fonts.LoadFont(font.FontFamily, font.SizeInPoints, FontStyle.Underline));
            factory.UI.GetLabel("AddToLabel", "Add To:", 50f, 20f, 50f, 0f, buttonsPanel, labelConfig);

            _roomButton = factory.UI.GetCheckBox("AddToRoomRadioButton", (ButtonAnimation)null, null, null, null, 60f, -40f, buttonsPanel, "Room", width: 20f, height: 25f);
            _guiButton = factory.UI.GetCheckBox("AddToGUIRadioButton", (ButtonAnimation)null, null, null, null, 180f, -40f, buttonsPanel, "GUI", width: 20f, height: 25f);
            _targetRadioGroup = new AGSRadioGroup();
            _roomButton.RadioGroup = _targetRadioGroup;
            _guiButton.RadioGroup = _targetRadioGroup;
            _targetRadioGroup.SelectedButton = isUi ? _guiButton : _roomButton;
        }
    }
}