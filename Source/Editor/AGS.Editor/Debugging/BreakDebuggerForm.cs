using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AGS.API;
using AGS.Engine;

namespace AGS.Editor
{
    public static class BreakDebuggerForm
    {
        private static API.IComponent _component;
        private static string _property;

        public static async Task Show(AGSEditor editor)
        {
            if (_component != null)
            {
                _component.PropertyChanged -= onPropertyChanged;
            }
            var gameSelector = new ComboboxboxField("Game", "Game", "Editor");
            var entitySelector = new TextboxField("Entity ID");
            var componentSelector = new TextboxField("Component");
            var propertySelector = new TextboxField("Property Name");
            var simpleForm = new SimpleForm(editor.Editor, gameSelector, entitySelector, componentSelector, propertySelector);
            await simpleForm.ShowAsync("Break Debugger when property changes");
            var game = gameSelector.Value == "Game" ? editor.Game : editor.Editor;
            var entity = game.Find<IEntity>(entitySelector.Value);
            if (entity == null)
            {
                await AGSMessageBox.DisplayAsync($"Did not find entity id {entitySelector.Value}", editor.Editor);
                return;
            }
            var component = entity.FirstOrDefault(c => c.Name == componentSelector.Value);
            if (component == null)
            {
                await AGSMessageBox.DisplayAsync($"Did not find component {componentSelector.Value} for entity id {entitySelector.Value}", editor.Editor);
                return;
            }
            _component = component;
            _property = propertySelector.Value;
            component.PropertyChanged += onPropertyChanged;
        }

        private static void onPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == _property)
            {
                Debugger.Break();
            }
        }
    }
}