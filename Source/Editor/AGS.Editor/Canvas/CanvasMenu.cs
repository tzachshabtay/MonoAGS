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
    public class CanvasMenu
    {
        private readonly AGSEditor _editor;
        private readonly GameToolbar _toolbar;
        private Menu _topMenu;
        private static int _lastId;
        private ICheckboxComponent _guiButton, _parentButton;
        private IObject _potentialParent;
        private IRadioGroup _targetRadioGroup;

        private enum Target 
        {
            Room,
            UI,
            Area
        }

        public CanvasMenu(AGSEditor editor, GameToolbar toolbar)
        {
            _editor = editor;
            _toolbar = toolbar;
        }

        public void Load()
        {
            Action noop = () => {};
            Menu guisMenu = new Menu(_editor.GameResolver, _editor.Editor.Settings, "GuisMenu", 180f,
                                     new MenuItem("Button", showButtonWizard),
                                     new MenuItem("Label", showLabelWizard),
                                     new MenuItem("ComboBox", showComboboxWizard),
                                     new MenuItem("TextBox", showTextboxWizard),
                                     new MenuItem("Inventory Window", showInventoryWindowWizard),
                                     new MenuItem("Checkbox", showCheckboxWizard),
                                     new MenuItem("Listbox", showListboxWizard),
                                     new MenuItem("Panel", showPanelWizard),
                                     new MenuItem("Slider", showSliderWizard));
            Menu presetsMenu = new Menu(_editor.GameResolver, _editor.Editor.Settings,"PresetsMenu", 100f,
                                        new MenuItem("Object", showObjectWizard),
                                        new MenuItem("Character", showCharacterWizard),
                                        new MenuItem("Area", showAreaWizard),
                                        new MenuItem("GUIs", guisMenu));
            _topMenu = new Menu(_editor.GameResolver, _editor.Editor.Settings, "CanvasMenu", 100f, new MenuItem("Create", presetsMenu));
            _topMenu.Load(_editor.Editor.Factory, _editor.Editor.Settings.Defaults);

            _editor.Editor.Input.MouseUp.Subscribe((MouseButtonEventArgs args) => 
            {
                if (args.Button == MouseButton.Right)
                {
                    if (!_toolbar.IsPaused) return;
                    if (!_editor.IsEditorPositionInGameWindow(args.MousePosition.XMainViewport, args.MousePosition.YMainViewport))
                    {
                        return;
                    }
                    _potentialParent = _editor.CanvasHitTest.ObjectAtMousePosition;
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

        private async void showWizard(string name, Target target, object factory, string methodName)
        {
            _topMenu.Visible = false;
            FactoryWizard wizard = new FactoryWizard(null, _editor, panel => addTargetUIForCreate(panel, target), validate, defaults => setDefaults(name, defaults));
            (object result, MethodModel model, MethodWizardAttribute methodAttribute) = await wizard.RunMethod(factory, methodName);
            if (model == null) return;
            List<object> entities = getEntities(factory, result, methodAttribute);
            addNewEntities(entities, model);
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

        private EntityModel addToModel(IEntity entity, MethodModel initializer)
        {
            if (_editor.Project.Model.Entities.TryGetValue(entity.ID, out var existingModel)) return existingModel;
            var entityModel = EntityModel.FromEntity(entity);
            entityModel.Initializer = initializer;

            _editor.Project.Model.Entities.Add(entity.ID, entityModel);
            var tree = entity.GetComponent<IInObjectTreeComponent>();
            if (tree == null) return entityModel;

            if (tree.TreeNode.Parent != null && 
                _editor.Project.Model.Entities.TryGetValue(tree.TreeNode.Parent.ID, out var parent) &&
                !parent.Children.Contains(entity.ID))
            {
                parent.Children.Add(entity.ID);
            }

            foreach (var child in tree.TreeNode.Children)
            {
                addToModel(child, null);
            }

            return entityModel;
        }

        private void addNewEntities(List<object> entities, MethodModel methodModel)
        {
            bool isParent = _potentialParent != null && _targetRadioGroup?.SelectedButton == _parentButton;
            bool isUi = _targetRadioGroup?.SelectedButton == _guiButton || (isParent && _editor.Game.State.UI.Contains(_potentialParent));
            bool isFirst = true;
            foreach (var entityObj in entities)
            {
                IEntity entity = (IEntity)entityObj;
                MethodModel initializer = null;
                if (isFirst)
                {
                    isFirst = false;
                    initializer = methodModel;
                }
                if (isParent)
                {
                    if (entity is IObject obj) _potentialParent.TreeNode.AddChild(obj);
                    else throw new Exception($"Unkown entity created: {entity?.GetType().Name ?? "null"}");
                }
                addToModel(entity, initializer);
                if (isUi)
                {
                    if (entity is IObject obj)
                    {
                        _editor.Game.State.UI.Add(obj);
                        _editor.Project.Model.Guis.Add(entity.ID);
                    }
                    else throw new Exception($"Unkown entity created: {entity?.GetType().Name ?? "null"}");
                }
                else
                {
                    var roomModel = _editor.Project.Model.Rooms.FirstOrDefault(r => r.ID == _editor.Game.State.Room.ID);
                    if (roomModel == null)
                    {
                        Debug.WriteLine($"Room {_editor.Game.State.Room.ID} encountered for the first time!");
                        roomModel = new RoomModel { ID = _editor.Game.State.Room.ID, Entities = new HashSet<string>() };
                        _editor.Project.Model.Rooms.Add(roomModel);
                    }
                    roomModel.Entities.Add(entity.ID);
                    if (entity is IObject obj) _editor.Game.State.Room.Objects.Add(obj);
                    else if (entity is IArea area) _editor.Game.State.Room.Areas.Add(area);
                    else throw new Exception($"Unkown entity created: {entity?.GetType().Name ?? "null"}");
                }
            }
        }

        private void addTargetUIForCreate(IPanel panel, Target target)
        {
            if (target == Target.Area) return;

            var factory = _editor.Editor.Factory;
            var buttonsPanel = factory.UI.GetPanel("MethodWizardTargetPanel", 100f, 45f, MethodWizard.MARGIN_HORIZONTAL, 50f, panel);
            buttonsPanel.Tint = Colors.Transparent;

            var font = _editor.Editor.Settings.Defaults.TextFont;
            var labelConfig = new AGSTextConfig(font: factory.Fonts.LoadFont(font.FontFamily, font.SizeInPoints, FontStyle.Underline));
            factory.UI.GetLabel("AddToLabel", "Add To:", 50f, 20f, 0f, 0f, buttonsPanel, labelConfig);

            const float buttonY = -40f;
            var roomButton = factory.UI.GetCheckBox("AddToRoomRadioButton", (ButtonAnimation)null, null, null, null, 10f, buttonY, buttonsPanel, "Room", width: 20f, height: 25f);
            var guiButton = factory.UI.GetCheckBox("AddToGUIRadioButton", (ButtonAnimation)null, null, null, null, 130f, buttonY, buttonsPanel, "GUI", width: 20f, height: 25f);

            _guiButton = guiButton.GetComponent<ICheckboxComponent>();
            _targetRadioGroup = new AGSRadioGroup();
            roomButton.RadioGroup = _targetRadioGroup;
            guiButton.RadioGroup = _targetRadioGroup;
            if (_potentialParent != null)
            {
                var parentButton = factory.UI.GetCheckBox("AddToParentRadioButton", (ButtonAnimation)null, null, null, null, 250f, buttonY, buttonsPanel, _potentialParent.GetFriendlyName(), width: 20f, height: 25f);
                parentButton.RadioGroup = _targetRadioGroup;
                _parentButton = parentButton.GetComponent<ICheckboxComponent>();
                parentButton.MouseClicked.Subscribe(() => 
                {
                    resetNumberEditor(panel, "x");
                    resetNumberEditor(panel, "y");
                });
            }
            _targetRadioGroup.SelectedButton = target == Target.UI ? _guiButton : roomButton;
        }

        private void resetNumberEditor(IPanel panel, string idSuffix)
        {
            var numberEditor = panel.TreeNode.FindDescendant(obj =>
                      obj.Entity.ID.StartsWith($"{InspectorTreeNodeProvider.EDITOR_PREFIX}{idSuffix}",
                                               StringComparison.InvariantCulture) && obj.Entity.HasComponent<INumberEditorComponent>());
            if (numberEditor != null)
            {
                numberEditor.GetComponent<INumberEditorComponent>().SetUserInitiatedValue(0f);
            }
        }

        private void setDefaults(string name, Dictionary<string, object> overrideDefaults)
        {
            var (x, y) = _editor.ToGameResolution(_topMenu.OriginalPosition.x, _topMenu.OriginalPosition.y, null);
            overrideDefaults["x"] = x;
            overrideDefaults["y"] = y;
            overrideDefaults["id"] = $"{name}{++_lastId}";
        }

        private async Task<bool> validate(Dictionary<string, object> map)
        {
            var id = map["id"];
            if (_editor.Project.Model.Entities.ContainsKey(id.ToString()))
            {
                var settings = new AGSMessageBoxSettings(_editor.Editor);
                settings.RenderLayer = new AGSRenderLayer(settings.RenderLayer.Z - 1000, independentResolution: settings.RenderLayer.IndependentResolution);
                await AGSMessageBox.DisplayAsync($"ID {id} is already taken, please select another ID.", _editor.Editor, settings);
                return false;
            }
            return true;
        }
    }
}