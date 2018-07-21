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
        private ICheckboxComponent _guiButton;
        private IRadioGroup _targetRadioGroup;

        private enum Target 
        {
            Room,
            UI,
            Area
        }

        public CanvasMenu(AGSEditor editor)
        {
            _editor = editor;
        }

        public void Load()
        {
            Action noop = () => {};
            Menu guisMenu = new Menu(_editor.GameResolver, "GuisMenu", 180f,
                                     new MenuItem("Button", showButtonWizard),
                                     new MenuItem("Label", showLabelWizard),
                                     new MenuItem("ComboBox", showComboboxWizard),
                                     new MenuItem("TextBox", showTextboxWizard),
                                     new MenuItem("Inventory Window", showInventoryWindowWizard),
                                     new MenuItem("Checkbox", showCheckboxWizard),
                                     new MenuItem("Listbox", showListboxWizard),
                                     new MenuItem("Panel", showPanelWizard),
                                     new MenuItem("Slider", showSliderWizard));
            Menu presetsMenu = new Menu(_editor.GameResolver, "PresetsMenu", 100f,
                                        new MenuItem("Object", showObjectWizard),
                                        new MenuItem("Character", showCharacterWizard),
                                        new MenuItem("Area", showAreaWizard),
                                        new MenuItem("GUIs", guisMenu));
            _topMenu = new Menu(_editor.GameResolver, "CanvasMenu", 100f, new MenuItem("Create", presetsMenu));
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

        private void showButtonWizard() => showWizard("button", Target.UI, _editor.Game.Factory.UI, nameof(IUIFactory.GetButton));
        private void showLabelWizard() => showWizard("label", Target.UI, _editor.Game.Factory.UI, nameof(IUIFactory.GetLabel));
        private void showCheckboxWizard() => showWizard("checkbox", Target.UI, _editor.Game.Factory.UI, nameof(IUIFactory.GetCheckBox));
        private void showComboboxWizard() => showWizard("combobox", Target.UI, _editor.Game.Factory.UI, nameof(IUIFactory.GetComboBox));
        private void showPanelWizard() => showWizard("panel", Target.UI, _editor.Game.Factory.UI, nameof(IUIFactory.GetPanel));
        private void showSliderWizard() => showWizard("slider", Target.UI, _editor.Game.Factory.UI, nameof(IUIFactory.GetSlider));
        private void showTextboxWizard() => showWizard("textbox", Target.UI, _editor.Game.Factory.UI, nameof(IUIFactory.GetTextBox));
        private void showInventoryWindowWizard() => showWizard("invWindow", Target.UI, _editor.Game.Factory.Inventory, nameof(IInventoryFactory.GetInventoryWindow));
        private void showListboxWizard() => showWizard("listbox", Target.UI, _editor.Game.Factory.UI, nameof(IUIFactory.GetListBox));
        private void showObjectWizard() => showWizard("object", Target.Room, _editor.Game.Factory.Object, nameof(IObjectFactory.GetAdventureObject));
        private void showCharacterWizard() => showWizard("character", Target.Room, _editor.Game.Factory.Object, nameof(IObjectFactory.GetCharacter));
        private void showAreaWizard() => showWizard("area", Target.Area, _editor.Game.Factory.Room, nameof(IRoomFactory.GetArea));

        private object get(string key, Dictionary<string, object> parameters) => parameters.TryGetValue(key, out var val) ? val : null;

        private async void showWizard(string name, Target target, object factory, string methodName)
        {
            var (method, methodAttribute) = getMethod(factory, methodName);
            HashSet<string> hideProperties = new HashSet<string>();
            Dictionary<string, object> overrideDefaults = new Dictionary<string, object>();
            foreach (var param in method.GetParameters())
            {
                var attr = param.GetCustomAttribute<MethodParamAttribute>();
                if (attr == null) continue;
                if (!attr.Browsable) hideProperties.Add(param.Name);
                if (attr.DefaultProvider != null)
                {
                    var resolver = _editor.GameResolver;
                    var provider = factory.GetType().GetMethod(attr.DefaultProvider);
                    if (provider == null)
                    {
                        throw new NullReferenceException($"Failed to find method with name: {attr.DefaultProvider ?? "null"}");
                    }
                    overrideDefaults[param.Name] = provider.Invoke(null, new[] { resolver });
                }
                else if (attr.Default != null) overrideDefaults[param.Name] = attr.Default;
            }

            _topMenu.Visible = false;

            var (x, y) = _editor.ToGameResolution(_topMenu.Position.x, _topMenu.Position.y, null);
            overrideDefaults["x"] = x;
            overrideDefaults["y"] = y;
            overrideDefaults["id"] = $"{name}{++_lastId}";
            var editorProvider = new EditorProvider(_editor.Editor.Factory, new ActionManager(), new StateModel(), _editor.Editor.State, _editor.Editor.Settings);
            var wizard = new MethodWizard(method, hideProperties, overrideDefaults, panel => addTargetForCreateUI(panel, target), editorProvider, _editor);
            wizard.Load();
            var parameters = await wizard.ShowAsync();
            if (parameters == null) return;
            foreach (var param in overrideDefaults.Keys)
            {
                parameters[param] = get(param, parameters) ?? overrideDefaults[param];
            }
            object result = runMethod(method, factory, parameters);
            List<object> entities = getEntities(factory, result, methodAttribute);
            addNewEntities(entities);
        }

        private (MethodInfo, MethodWizardAttribute) getMethod(object factory, string methodName)
        {
            foreach (var method in factory.GetType().GetMethods())
            {
                if (method.Name != methodName) continue;
                var attr = method.GetCustomAttribute<MethodWizardAttribute>();
                if (attr == null) continue;
                return (method, attr);
            }
            throw new InvalidOperationException($"Failed to find method name {methodName} in {factory.GetType()}");
        }

        private List<object> getEntities(object factory, object result, MethodWizardAttribute attr)
        {
            if (attr.EntitiesProvider == null) return new List<object>{result};

            var provider = factory.GetType().GetMethod(attr.EntitiesProvider);
            if (provider == null)
            {
                throw new NullReferenceException($"Failed to find entity provider method with name: {attr.EntitiesProvider ?? "null"}");
            }
            return (List<object>)provider.Invoke(null, new[] { result });
        }

        private void addNewEntities(List<object> entities)
        {
            bool isUi = _targetRadioGroup?.SelectedButton == _guiButton;
            foreach (var entity in entities)
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
        }

        private object runMethod(MethodInfo method, object factory, Dictionary<string, object> parameters)
        {
            var methodParams = method.GetParameters();
            object[] values = methodParams.Select(m => parameters.TryGetValue(m.Name, out object val) ?
                                                  val : MethodParam.GetDefaultValue(m.ParameterType)).ToArray();
            return method.Invoke(factory, values);
        }

        private void addTargetForCreateUI(IPanel panel, Target target)
        {
            if (target == Target.Area) return;

            var factory = _editor.Editor.Factory;
            var buttonsPanel = factory.UI.GetPanel("MethodWizardTargetPanel", 300f, 45f, 0f, 50f, panel);
            buttonsPanel.Tint = Colors.Transparent;

            var font = _editor.Editor.Settings.Defaults.TextFont;
            var labelConfig = new AGSTextConfig(font: factory.Fonts.LoadFont(font.FontFamily, font.SizeInPoints, FontStyle.Underline));
            factory.UI.GetLabel("AddToLabel", "Add To:", 50f, 20f, 50f, 0f, buttonsPanel, labelConfig);

            var roomButton = factory.UI.GetCheckBox("AddToRoomRadioButton", (ButtonAnimation)null, null, null, null, 60f, -40f, buttonsPanel, "Room", width: 20f, height: 25f);
            var guiButton = factory.UI.GetCheckBox("AddToGUIRadioButton", (ButtonAnimation)null, null, null, null, 180f, -40f, buttonsPanel, "GUI", width: 20f, height: 25f);
            _guiButton = guiButton.GetComponent<ICheckboxComponent>();
            _targetRadioGroup = new AGSRadioGroup();
            roomButton.RadioGroup = _targetRadioGroup;
            guiButton.RadioGroup = _targetRadioGroup;
            _targetRadioGroup.SelectedButton = target == Target.UI ? _guiButton : roomButton;
        }
    }
}