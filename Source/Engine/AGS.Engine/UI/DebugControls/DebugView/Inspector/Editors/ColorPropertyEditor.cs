using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using AGS.API;

namespace AGS.Engine
{
    public class ColorPropertyEditor : IInspectorPropertyEditor
    {
        private readonly IGameFactory _factory;
        private InspectorProperty _property;
        private ITextBox _text;
        private ILabel _colorLabel;
        private IButton _dropDownButton;
        private Dictionary<string, uint> _namedColors;
        private Dictionary<uint, string> _namedColorsReversed;

        public ColorPropertyEditor(IGameFactory factory)
        {
            _factory = factory;
            _namedColors = new Dictionary<string, uint>();
            _namedColorsReversed = new Dictionary<uint, string>();
        }

        public void AddEditorUI(string id, ITreeNodeView view, InspectorProperty property)
        {
            _property = property;
            var label = view.TreeItem;
            var combobox = _factory.UI.GetComboBox(id, null, null, null, label.TreeNode.Parent, defaultWidth: 200f, defaultHeight: 25f);
            _dropDownButton = combobox.DropDownButton;
            _text = combobox.TextBox;
            _text.TextBackgroundVisible = false;
            var list = new List<IStringItem>();
            foreach (var field in typeof(Colors).GetTypeInfo().DeclaredFields)
            {
                list.Add(new AGSStringItem { Text = field.Name });
                Color color = (Color)field.GetValue(null);
                _namedColors[field.Name] = color.Value;
                _namedColorsReversed[color.Value] = field.Name;
            }
            combobox.DropDownPanelList.Items.AddRange(list);
            combobox.Z = label.Z;

            _colorLabel = _factory.UI.GetLabel($"{id}_ColorLabel", "", 50f, 25f, combobox.Width + 10f, 0f, label.TreeNode.Parent);
            _colorLabel.TextVisible = false;

            RefreshUI();
            _text.TextConfig.AutoFit = AutoFit.TextShouldFitLabel;
            _text.TextConfig.Alignment = Alignment.MiddleLeft;
            _text.TextBackgroundVisible = true;
            _text.Border = AGSBorders.SolidColor(Colors.White, 2f);
            var whiteBrush = _text.TextConfig.Brush;
            var yellowBrush = _factory.Graphics.Brushes.LoadSolidBrush(Colors.Yellow);
            _text.MouseEnter.Subscribe(_ => { _text.TextConfig.Brush = yellowBrush; });
            _text.MouseLeave.Subscribe(_ => { _text.TextConfig.Brush = whiteBrush; });
            _text.OnPressingKey.Subscribe(onTextboxPressingKey);
            combobox.SuggestMode = ComboSuggest.Suggest;
            combobox.DropDownPanelList.OnSelectedItemChanged.Subscribe(args =>
            {
                property.Prop.SetValue(property.Object, Color.FromHexa(_namedColors[args.Item.Text]));
            });
        }

        public void RefreshUI()
        {
            var color = (Color)_property.Prop.GetValue(_property.Object);
            if (_namedColorsReversed.ContainsKey(color.Value))
            {
                _text.Text = _namedColorsReversed[color.Value];
            }
            else
            {
                _text.Text = $"{color.R},{color.G},{color.B},{color.A}";
            }
            _colorLabel.Tint = color;
            _text.Border = AGSBorders.SolidColor(color, 2f);
            var buttonBorder = AGSBorders.Multiple(AGSBorders.SolidColor(color, 1f),
                               _factory.Graphics.Icons.GetArrowIcon(ArrowDirection.Down, color));
            _dropDownButton.IdleAnimation = new ButtonAnimation(buttonBorder, null, Colors.Transparent);
            _dropDownButton.Border = buttonBorder;
        }

        private void onTextboxPressingKey(TextBoxKeyPressingEventArgs args)
        {
            string text = args.IntendedState.Text;
            if (string.IsNullOrEmpty(text)) return;

            //except for selecting pre-set colors from the dropdown, we'll allow multiple formats in the 
            //textbox for specifying the color:
            //1. "{R},{G},{B}" or "{R},{G},{B},{A}": those would be from 0-255
            //2. "#{HEXA}" for a hexa color number

            if (parseHexaColor(text)) return;
            parseRgbColor(text, args);
        }

        private bool parseHexaColor(string text)
        {
            if (!text.StartsWith("#", StringComparison.Ordinal) || text.Length == 1) return false;

            text = text.Substring(1);
            if (text.Length == 6)
            {
                text = "ff" + text;
            }
            if (!uint.TryParse(text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint hexaColor)) return false;
            _property.Prop.SetValue(_property.Object, Color.FromHexa(hexaColor));
            return true;
        }

        private bool parseRgbColor(string text, TextBoxKeyPressingEventArgs args)
        {
            var tokens = text.Split(',');
            if (tokens.Length != 3 && tokens.Length != 4) return false;
            if (!byte.TryParse(tokens[0], out byte r)) return false;
            if (!byte.TryParse(tokens[1], out byte g)) return false;
            if (!byte.TryParse(tokens[2], out byte b)) return false;
            byte a = 255;
            if (tokens.Length == 4)
            {
                if (!byte.TryParse(tokens[3], out a)) return false;
            }

            var color = Color.FromRgba(r, g, b, a);
            var newColor = manipulateRgb(color, tokens, text, args);
            if (newColor != null)
            {
                color = newColor.Value;
                if (tokens.Length == 4) args.IntendedState.Text = $"{color.R},{color.G},{color.B},{color.A}";
                else args.IntendedState.Text = $"{color.R},{color.G},{color.B}";
            }
            _property.Prop.SetValue(_property.Object, color);
            return true;
        }

        private Color? manipulateRgb(Color color, string[] tokens, string text, TextBoxKeyPressingEventArgs args)
        {
            if (args.PressedKey != Key.Down && args.PressedKey != Key.Up) return null;

            int caret = args.IntendedState.CaretPosition;
            int firstComma = text.IndexOf(',');
            if (caret <= firstComma)
            {
                return Color.FromRgba(manipulateColorUnit(color.R, args), color.G, color.B, color.A);
            }

            int secondComma = nthIndexOfComma(text, 0, 2);
            if (caret <= secondComma)
            {
                return Color.FromRgba(color.R, manipulateColorUnit(color.G, args), color.B, color.A);
            }

            if (tokens.Length == 4)
            {
                int thirdComma = nthIndexOfComma(text, 0, 3);
                if (caret > thirdComma)
                {
                    return Color.FromRgba(color.R, color.G, color.B, manipulateColorUnit(color.A, args));
                }
            }
            return Color.FromRgba(color.R, color.G, manipulateColorUnit(color.B, args), color.A);
        }

        private byte manipulateColorUnit(byte unit, TextBoxKeyPressingEventArgs args)
        {
            if (unit <= 0 && args.PressedKey == Key.Down) return unit;
            if (unit >= 255 && args.PressedKey == Key.Up) return unit;
            if (args.PressedKey == Key.Down) return (byte)(unit - 1);
            return (byte)(unit + 1);
        }

        //https://stackoverflow.com/questions/186653/get-the-index-of-the-nth-occurrence-of-a-string
        private static int nthIndexOfComma(string input, int startIndex, int nth)
        {
            if (nth < 1)
                throw new NotSupportedException("Param 'nth' must be greater than 0!");
            if (nth == 1)
                return input.IndexOf(',', startIndex);
            var idx = input.IndexOf(',', startIndex);
            if (idx == -1)
                return -1;
            return nthIndexOfComma(input, idx + 1, nth - 1);
        }
    }
}
