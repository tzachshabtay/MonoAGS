using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AGS.API;
using AGS.Engine;

namespace AGS.Editor
{
    public static class FindObjectForm
    {
        public static async Task Show(AGSEditor editor)
        {
            var gameSelector = new ComboboxboxField("Game", "Game", "Editor");
            var entitySelector = new TextboxField("Text to search");
            var propertyTypeSelector = new ComboboxboxField("Search By", nameof(IEntity.ID), nameof(ITextComponent.Text));
            var testTypeSelector = new ComboboxboxField("Test By", "Includes", "Exact Match");
            var caseSensitive = new CheckboxField("Case Sensitive");
            var simpleForm = new SimpleForm(editor.Editor, gameSelector, entitySelector, propertyTypeSelector, testTypeSelector, caseSensitive);
            await simpleForm.ShowAsync("Find Object");
            if (entitySelector.Value == null)
            {
                return;
            }
            Debug.WriteLine($"Searching for {entitySelector.Value} ({propertyTypeSelector.Value}, {testTypeSelector.Value}, Case Sensitive={caseSensitive.Value}) in {gameSelector.Value}");
            var game = gameSelector.Value == "Game" ? editor.Game : editor.Editor;
            var matches = game.State.All<IObject>().Where(e =>
            {
                var property = e.GetType().GetProperty(propertyTypeSelector.Value);
                if (property == null) return false;
                string value = property.GetValue(e)?.ToString();
                if (value == null) return false;
                string matchWith = entitySelector.Value;
                if (!caseSensitive.Value)
                {
                    value = value.ToLowerInvariant();
                    matchWith = matchWith.ToLowerInvariant(); 
                }
                if (testTypeSelector.Value == "Exact Match") return value == matchWith;
                return value.Contains(matchWith);
            });
            foreach (var match in matches)
            {
                Debug.WriteLine($"Found match: {(match, match.Position, match.WorldXY)}");
            }
            await AGSMessageBox.DisplayAsync($"Done. Results written to console.", editor.Editor);
        }
    }
}