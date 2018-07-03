using System;
using AGS.API;

namespace AGS.Editor
{
    public class CanvasMenu
    {
        private readonly IGameFactory _factory;
        private readonly IInput _input;

        public CanvasMenu(IGameFactory factory, IInput input)
        {
            _factory = factory;
            _input = input;
        }

        public void Load()
        {
            Action noop = () => { };
            Menu guisMenu = new Menu("GuisMenu", 180f,
                                     new MenuItem("Button", noop),
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
            Menu topMenu = new Menu("CanvasMenu", 100f, new MenuItem("Create", presetsMenu));
            topMenu.Load(_factory);

            _input.MouseUp.Subscribe((MouseButtonEventArgs args) => 
            {
                if (args.Button == MouseButton.Right)
                {
                    topMenu.Position = (args.MousePosition.XMainViewport, args.MousePosition.YMainViewport);
                    topMenu.Visible = true;
                }
                else if (args.ClickedEntity == null) 
                {
                    topMenu.Visible = false;
                } 
            });
        }
    }
}