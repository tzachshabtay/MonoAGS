using System;
using System.Threading.Tasks;
using AGS.API;
using AGS.Engine;

namespace DemoGame
{
    public class FeaturesLabelsPanel : IFeaturesPanel
    {
        private ILabel _label;
        private IComboBox _featuresAutoFitCombobox;
        private IGame _game;
        private const string LABEL_TEXT = "The quick brown fox jumps over the lazy dog.";

        public FeaturesLabelsPanel(IGame game, IObject parent)
        {
            _game = game;
            var factory = game.Factory;
            _label = factory.UI.GetLabel("FeaturesLabel", LABEL_TEXT,
                                         200f, 50f, 25f, parent.Height - 25f, parent, 
                                         new AGSTextConfig(autoFit: AutoFit.TextShouldWrapAndLabelShouldFitHeight), false);
            _label.RenderLayer = parent.RenderLayer;
            _label.Pivot = (0f, 1f);
            _label.Tint = Colors.DarkOliveGreen;
            _label.Border = AGSBorders.SolidColor(Colors.LightSeaGreen, 3f);
            _label.MouseEnter.Subscribe(_ => _label.Tint = Colors.DarkGoldenrod);
            _label.MouseLeave.Subscribe(_ => _label.Tint = Colors.DarkOliveGreen);
            _label.Enabled = true;

            float autoFitX = _label.X;
            float autoFitY = _label.Y - _label.Height - 100f;

            _featuresAutoFitCombobox = factory.UI.GetComboBox("FeaturesAutoFitCombo", null, null, null, parent, false);
            _featuresAutoFitCombobox.TextBox.Text = nameof(AutoFit.TextShouldWrapAndLabelShouldFitHeight);
            _featuresAutoFitCombobox.Position = (autoFitX, autoFitY);
            var autoFitList = _featuresAutoFitCombobox.DropDownPanelList;
            foreach (var autoFitOption in Enum.GetValues(typeof(AutoFit)))
            {
                autoFitList.Items.Add(new AGSStringItem { Text = autoFitOption.ToString() });
            }

            autoFitList.OnSelectedItemChanged.Subscribe(args => _label.TextConfig.AutoFit = (AutoFit)Enum.Parse(typeof(AutoFit), args.Item.Text));
            animateText();
        }

        public void Show() 
        {
            _game.State.UI.Add(_label);
            _game.State.UI.Add(_featuresAutoFitCombobox.TextBox);
            _game.State.UI.Add(_featuresAutoFitCombobox.DropDownButton);
            _game.State.UI.Add(_featuresAutoFitCombobox);
        }

        public Task Close() 
        {
            _game.State.UI.Remove(_label);
            _game.State.UI.Remove(_featuresAutoFitCombobox.TextBox);
            _game.State.UI.Remove(_featuresAutoFitCombobox.DropDownButton);
            _game.State.UI.Remove(_featuresAutoFitCombobox);
            return Task.CompletedTask;
        }

        private async void animateText()
        {
            var textLen = _label.Text.Length + 1;
            if (textLen > LABEL_TEXT.Length) textLen = 0;
            _label.Text = LABEL_TEXT.Substring(0, textLen);
            await Task.Delay(200);
            animateText();
        }
    }
}
