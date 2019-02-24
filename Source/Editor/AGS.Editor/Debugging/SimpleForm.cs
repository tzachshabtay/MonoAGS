using System.Linq;
using System.Threading.Tasks;
using AGS.API;
using AGS.Engine;

namespace AGS.Editor
{
    public interface ISimpleField
    {
        string Name { get; }
        void Add(Position position, IObject parent, IGameFactory factory);
    }

    public class TextboxField : ISimpleField
    {
        private ITextBox _textbox;

        public TextboxField(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public string Value => _textbox.Text;

        public void Add(Position position, IObject parent, IGameFactory factory)
        {
            _textbox = factory.UI.GetTextBox($"{Name}_SimpleTextbox", position.X, position.Y, parent, width: 500f, height: 40f);
            _textbox.Tint = GameViewColors.Textbox;
        }
    }

    public class ComboboxboxField : ISimpleField
    {
        private IComboBox _combobox;
        private string[] _values;

        public ComboboxboxField(string name, params string[] values)
        {
            Name = name;
            _values = values;
        }

        public string Name { get; }

        public string Value => _combobox.DropDownPanelList.SelectedItem?.Text;

        public void Add(Position position, IObject parent, IGameFactory factory)
        {
            _combobox = factory.UI.GetComboBox($"{Name}_SimpleCombobox", parent: parent);
            _combobox.Position = position;
            _combobox.DropDownPanelList.Items.AddRange(_values.Select(v => new AGSStringItem { Text = v}).Cast<IStringItem>().ToList());
        }
    }

    public class CheckboxField : ISimpleField
    {
        private ICheckBox _checkbox;

        public CheckboxField(string name)
        {
            Name = name; 
        }

        public string Name { get; }

        public bool Value => _checkbox.Checked;

        public void Add(Position position, IObject parent, IGameFactory factory)
        {
            _checkbox = factory.UI.GetCheckBox($"{Name}_SimpleCheckbox", (ButtonAnimation)null, null, null, null, position.X, position.Y, parent, width: 40f, height: 40f);
        }
    }

    public class SimpleForm
    {
        private IGame _game;

        public SimpleForm(IGame game, params ISimpleField[] fields)
        {
            _game = game;
            Fields = fields;
        }

        public ISimpleField[] Fields { get; }

        public async Task ShowAsync(string title)
        {
            const float height = 50f;
            const float gap = 10f;
            const float offsetY = 150f;
            var factory = _game.Factory;
            var form = factory.UI.GetForm("SimpleForm", title, 800f, 30f, Fields.Length * (height + gap) + offsetY, 400f, 100f);
            form.Contents.Tint = GameViewColors.Panel;
            form.Header.Tint = GameViewColors.Panel;
            form.Contents.AddComponent<IModalWindowComponent>().GrabFocus();
            float x = 5f;
            float y = form.Height - gap - offsetY;
            const float labelWidth = 200f;
            foreach (var field in Fields)
            {
                factory.UI.GetLabel($"SimpleLabel_{field.Name}", field.Name, labelWidth, height, x, y, form.Contents);
                field.Add((x + labelWidth, y), form.Contents, factory);
                y -= height + gap;
            }
            const float buttonWidth = 50f;
            var okButton = factory.UI.GetButton("SimpleFormOkButton", (IAnimation)null, null, null, form.Width / 2f - buttonWidth / 2f, y, form.Contents, "OK",
                                                width: buttonWidth, height: 30f);
            TaskCompletionSource<object> onOk = new TaskCompletionSource<object>();
            okButton.MouseClicked.Subscribe(() => onOk.TrySetResult(null));
            await onOk.Task;
            form.Header.DestroyWithChildren(_game.State);
        }
    }
}