using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AGS.API;
using AGS.Engine;
using GuiLabs.Undo;

namespace AGS.Editor
{
    public class ColorPropertyEditor : IEditorSupportsNulls
    {
        private readonly IGameFactory _factory;
        private readonly ActionManager _actions;
        private readonly StateModel _model;
        private IProperty _property;
        private ITextBox _text;
        private ILabel _colorLabel;
        private IButton _dropDownButton;
        private IComboBox _combobox;

        private static List<IStringItem> _colorList = new List<IStringItem>(NamedColorsMap.NamedColors.Keys.Select(c => new AGSStringItem { Text = c }));

        public ColorPropertyEditor(IGameFactory factory, ActionManager actions, StateModel model)
        {
            _factory = factory;
            _actions = actions;
            _model = model;
        }

        public void AddEditorUI(string id, ITreeNodeView view, IProperty property)
        {
            _property = property;
            var label = view.TreeItem;
            var panel = _factory.UI.GetPanel(id, 0f, 0f, 0f, 0f, label.TreeNode.Parent);
            _combobox = SelectEditor.GetCombobox($"{id}_Combobox", _factory, panel);
            _dropDownButton = _combobox.DropDownButton;
            _text = _combobox.TextBox;
            _combobox.DropDownPanelList.MinWidth = 200f;
            _combobox.DropDownPanelList.Items.AddRange(_colorList);
            _combobox.Z = label.Z;

            _colorLabel = _factory.UI.GetLabel($"{id}_ColorLabel", "", 50f, 25f, 250f, 0f, panel);
            _colorLabel.TextVisible = false;

            var layout = view.HorizontalPanel.GetComponent<ITreeTableRowLayoutComponent>();
            if (layout != null)
            {
                layout.RestrictionList.RestrictionList.AddRange(new List<string> { panel.ID });
            }

            RefreshUI();
            _text.OnPressingKey.Subscribe(onTextboxPressingKey);
            _combobox.SuggestMode = ComboSuggest.Suggest;
            _combobox.DropDownPanelList.OnSelectedItemChanged.Subscribe(args =>
            {
                setColor(Color.FromHexa(NamedColorsMap.NamedColors[args.Item.Text]));
            });
        }

        public void RefreshUI()
        {
            if (_property == null) return;
            OnNullChanged(false);
        }

        public void OnNullChanged(bool isNull)
        {
            if (isNull)
            {
                _text.Text = "";
                _colorLabel.Tint = Colors.Transparent;
                _combobox.Visible = false;
                return;
            }
            var color = Colors.Red;
            if (_property.Value is Color c)
                color = c;
            else if (_property.Value is uint u)
            {
                color = Color.FromHexa(u);
                _property.Value = color;
            }

            Action onBoxChanged = null;
            onBoxChanged = () =>
            {
                _colorLabel.X = _text.WorldBoundingBox.Width + 40f;
                _text.OnBoundingBoxesChanged.Unsubscribe(onBoxChanged);
            };
            _text.OnBoundingBoxesChanged.Subscribe(onBoxChanged);
            _text.Text = color.Value == 0 ? "" : color.ToString();
            _colorLabel.Tint = color;
            _combobox.Visible = true;
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
            setColor(Color.FromHexa(hexaColor));
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
            setColor(color);
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

        private void setColor(Color color)
        {
            if (_actions.ActionIsExecuting) return;
            _actions.RecordAction(new PropertyAction(_property, color, _model));
        }
    }
}