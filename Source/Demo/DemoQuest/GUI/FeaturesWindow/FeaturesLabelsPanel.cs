using System;
using AGS.API;
using AGS.Engine;

namespace DemoGame
{
    public class FeaturesLabelsPanel : IFeaturesPanel
    {
        private ILabel _label;
        private IButton _featuresAutoFitDropDownButton;
        private ITextBox _featuresAutoFitTextbox;
        private IComboBox _featuresAutoFitCombobox;
        private IGame _game;
        private IObject _parent;

        public FeaturesLabelsPanel(IGame game, IObject parent)
        {
            _game = game;
            _parent = parent;
            var factory = game.Factory;
            _label = factory.UI.GetLabel("FeaturesLabel", "The quick brown fox jumps over the lazy dog.",
                                         200f, 50f, 25f, _parent.Height - 25f, parent, 
                                         new AGSTextConfig(autoFit: AutoFit.TextShouldWrapAndLabelShouldFitHeight), false);
            _label.RenderLayer = parent.RenderLayer;
            _label.Anchor = new PointF(0f, 1f);
            _label.Tint = Colors.DarkOliveGreen;
            _label.Border = AGSBorders.SolidColor(Colors.LightSeaGreen, 3f);

            float autoFitX = _label.X;
            float autoFitY = _label.Y - _label.Height - 100f;

            _featuresAutoFitTextbox = factory.UI.GetTextBox("FeaturesAutoFitTextbox", autoFitX, autoFitY, parent, 
                                                            nameof(AutoFit.TextShouldWrapAndLabelShouldFitHeight),
                                                            new AGSTextConfig(alignment: Alignment.MiddleCenter), 
                                                            false, 500f, 40f);
            _featuresAutoFitTextbox.Border = AGSBorders.SolidColor(Colors.WhiteSmoke, 3f);
            _featuresAutoFitTextbox.Tint = Colors.Transparent;
            _featuresAutoFitTextbox.RenderLayer = parent.RenderLayer;

            var whiteArrow = AGSBorders.Multiple(AGSBorders.SolidColor(Colors.WhiteSmoke, 3f), 
                                                 factory.Graphics.Icons.GetArrowIcon(ArrowDirection.Down, Colors.WhiteSmoke));
            var yellowArrow = AGSBorders.Multiple(AGSBorders.SolidColor(Colors.Yellow, 3f), 
                                                  factory.Graphics.Icons.GetArrowIcon(ArrowDirection.Down, Colors.Yellow));
            _featuresAutoFitDropDownButton = factory.UI.GetButton("FeaturesAutoFitDropDownButton", 
                                                                  new ButtonAnimation(whiteArrow, null, Colors.Transparent), 
                                                                  new ButtonAnimation(yellowArrow, null, Colors.Transparent), 
                                                                  new ButtonAnimation(yellowArrow, null, Colors.White.WithAlpha(100)),
                                                                  autoFitX, autoFitY, parent, "", null, 
                                                                  false, 30f, 40f);
            _featuresAutoFitDropDownButton.Border = whiteArrow;
            _featuresAutoFitDropDownButton.RenderLayer = parent.RenderLayer;
            _featuresAutoFitDropDownButton.Z = _featuresAutoFitTextbox.Z - 1;

            var yellowBrush = factory.Graphics.Brushes.LoadSolidBrush(Colors.Yellow);
            var whiteBrush = factory.Graphics.Brushes.LoadSolidBrush(Colors.White);
            Func<IButton> itemButtonFactory = () =>
            {
                var button = factory.UI.GetButton("FeaturesAutoFitItem" + Guid.NewGuid(), 
                                                  new ButtonAnimation(null, new AGSTextConfig(whiteBrush, autoFit: AutoFit.TextShouldWrapAndLabelShouldFitHeight), null),
                                                  new ButtonAnimation(null, new AGSTextConfig(yellowBrush, autoFit: AutoFit.TextShouldWrapAndLabelShouldFitHeight), null), 
                                                  new ButtonAnimation(null, new AGSTextConfig(yellowBrush, outlineBrush: whiteBrush, outlineWidth: 0.5f, autoFit: AutoFit.TextShouldWrapAndLabelShouldFitHeight), null), 
                                                  0f, 0f, width: 500f, height: 50f);
                button.RenderLayer = parent.RenderLayer;
                return button;
            };

            _featuresAutoFitCombobox = factory.UI.GetComboBox("FeaturesAutoFitCombo", _featuresAutoFitDropDownButton, _featuresAutoFitTextbox,
                                   itemButtonFactory, parent, false);
            _featuresAutoFitCombobox.X = autoFitX;
            _featuresAutoFitCombobox.Y = autoFitY;
            foreach (var autoFitOption in Enum.GetValues(typeof(AutoFit)))
            {
                _featuresAutoFitCombobox.Items.Add(autoFitOption);
            }
            _featuresAutoFitCombobox.DropDownPanel.Border = AGSBorders.SolidColor(Colors.Green, 3f);
            _featuresAutoFitCombobox.DropDownPanel.Tint = Colors.DarkGreen;

            _featuresAutoFitCombobox.OnSelectedItemChanged.Subscribe((_, args) => _label.TextConfig.AutoFit = (AutoFit)args.Item);
        }

        public void Show() 
        {
            _game.State.UI.Add(_label);
            _game.State.UI.Add(_featuresAutoFitTextbox);
            _game.State.UI.Add(_featuresAutoFitDropDownButton);
            _game.State.UI.Add(_featuresAutoFitCombobox);
        }

        public void Close() 
        {
            _game.State.UI.Remove(_label);
            _game.State.UI.Remove(_featuresAutoFitTextbox);
            _game.State.UI.Remove(_featuresAutoFitDropDownButton);
            _game.State.UI.Remove(_featuresAutoFitCombobox);
        }
    }
}
