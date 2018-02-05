using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AGS.API;

namespace AGS.Engine
{
    public class AGSComboBoxComponent : AGSComponent, IComboBoxComponent
    {
        private IButton _dropDownButton;
        private IListboxComponent _dropDownPanelList;
        private IVisibleComponent _dropDownPanelVisible;
        private IScrollingComponent _scrolling;
        private IEntity _dropDownPanel;
        private ComboSuggest _suggestMode;
        private ITextBox _textbox;
        private int _currentSuggestion = -1;
        private Color? _lastSuggestionTint;
        private bool _currentlyNavigatingSuggestions;
        private bool _currentlyUsingTextbox;

        public AGSComboBoxComponent(IGameEvents gameEvents)
        {
            MarkComboboxSuggestion = markSuggestion;
            gameEvents.OnRepeatedlyExecute.Subscribe(refreshDropDownLayout);
        }

        public IButton DropDownButton
        {
            get => _dropDownButton;
            set
            {
                if (_dropDownButton == value) return;
                _dropDownButton?.MouseClicked.Unsubscribe(onDropDownClicked);
                _dropDownButton = value;
                _dropDownButton?.MouseClicked.Subscribe(onDropDownClicked);
            }
        }

        public ITextBox TextBox 
        {
            get => _textbox; 
            set
            {
                var currentTextbox = _textbox;
                if (currentTextbox != null) currentTextbox.OnPressingKey.Unsubscribe(onTextboxKeyPressed);
                _textbox = value;
                if (value != null) value.OnPressingKey.Subscribe(onTextboxKeyPressed);
                updateTextbox();
            }
        }

        public IListboxComponent DropDownPanelList => _dropDownPanelList;

        public ComboSuggest SuggestMode 
        { 
            get => _suggestMode; 
            set
            {
                _suggestMode = value;
                updateTextbox();
            }
        }

        public IEntity DropDownPanel 
        {
            get => _dropDownPanel;
            set 
            {
                _dropDownPanel = value;
                var scrollingContainer = value.GetComponent<IInObjectTreeComponent>()?.TreeNode.Parent ?? value;
                var visibleComponent = scrollingContainer.GetComponent<IVisibleComponent>();
                var listBoxComponent = value.GetComponent<IListboxComponent>();
                var scrollingImageComponent = scrollingContainer.GetComponent<IImageComponent>();
                var imageComponent = value.GetComponent<IImageComponent>();
                var translateComponent = value.GetComponent<ITranslateComponent>();
                _scrolling = value.GetComponent<IScrollingComponent>();
                _dropDownPanelVisible = visibleComponent;

                _dropDownPanelList?.OnSelectedItemChanged.Unsubscribe(onSelectedItemChanged);
                _dropDownPanelList = listBoxComponent;
                listBoxComponent?.OnSelectedItemChanged.Subscribe(onSelectedItemChanged);
                if (scrollingImageComponent != null) scrollingImageComponent.Pivot = new PointF(0f, 1f);
                if (translateComponent != null) translateComponent.Y = -1f;
                if (visibleComponent != null) visibleComponent.Visible = false;
            }
        }

        public Action<IButton, IButton> MarkComboboxSuggestion { get; set; }

        private void updateTextbox()
        {
            var textBox = TextBox;
            if (textBox != null) textBox.Enabled = (_suggestMode != ComboSuggest.None);
        }

        private void onTextboxKeyPressed(TextBoxKeyPressingEventArgs args)
        {
            if (_suggestMode == ComboSuggest.None || !DropDownButton.Enabled)
            {
                args.ShouldCancel = true;
                return;
            }
            var dropDownPanel = _dropDownPanelList;
            var panelVisible = _dropDownPanelVisible;
            if (dropDownPanel == null || panelVisible == null) return;

            _currentlyUsingTextbox = true;
            panelVisible.Visible = true;
            var buttons = dropDownPanel.ItemButtons.ToList();
            _dropDownPanelList.SearchFilter = args.IntendedState.Text;
            switch (args.PressedKey)
            {
                case Key.Enter:
                    if (_suggestMode == ComboSuggest.Enforce || _currentlyNavigatingSuggestions)
                    {
                        var matchingItem = dropDownPanel.Items.FirstOrDefault(c => c.Text.ToLowerInvariant() != args.IntendedState.Text.ToLowerInvariant());
                        if (matchingItem == null || _currentlyNavigatingSuggestions)
                        {
                            selectSuggestion(buttons, args);
                        }
                        else
                        {
                            dropDownPanel.SelectedIndex = dropDownPanel.Items.IndexOf(matchingItem);
                        }
                    }
                    panelVisible.Visible = false;
                    return;
                case Key.Tab:
                    selectSuggestion(buttons, args);
                    panelVisible.Visible = false;
                    return;
                case Key.Up:
                    _currentlyNavigatingSuggestions = true;
                    if (_currentSuggestion > 0) searchAndMarkSuggestion(buttons, -1);
                    break;
                case Key.Down:
                    _currentlyNavigatingSuggestions = true;
                    if (_currentSuggestion < buttons.Count - 1) searchAndMarkSuggestion(buttons, 1);
                    break;
                default:
                    _currentlyNavigatingSuggestions = false;
                    break;
            }
            if (_currentSuggestion < 0 || _currentSuggestion >= buttons.Count || !buttons[_currentSuggestion].Visible)
            {
                markFirstVisibleSuggestion(buttons);
            }
            else
            {
                scrollToSuggestion(buttons, buttons[_currentSuggestion], _currentSuggestion);
            }
        }

        private void selectSuggestion(List<IButton> buttons, TextBoxKeyPressingEventArgs args)
        {
            if (_currentSuggestion < 0 || _currentSuggestion >= buttons.Count || !buttons[_currentSuggestion].Visible)
            {
                if (_suggestMode == ComboSuggest.Enforce)
                {
                    args.IntendedState.Text = _dropDownPanelList.SelectedItem?.Text ?? "";
                    onTextboxTypingCompleted(buttons);
                }
            }
            else
            {
                var selection = buttons[_currentSuggestion];
                args.IntendedState.Text = selection.Text;
                _dropDownPanelList.SelectedIndex = _dropDownPanelList.Items.IndexOf(_dropDownPanelList.Items.First(i => i.Text == selection.Text));
            }
        }

        private void onTextboxTypingCompleted(List<IButton> buttons)
        {
            markSuggestion(buttons, -1);
            _currentlyUsingTextbox = false;
            _dropDownPanelList.SearchFilter = "";
        }

        private void markFirstVisibleSuggestion(List<IButton> buttons)
        {
            for (int index = 0; index < buttons.Count; index++)
            {
                if (!buttons[index].Visible) continue;
                markSuggestion(buttons, index);
                return;
            }
        }

        private void searchAndMarkSuggestion(List<IButton> buttons, int step)
        {
            if (buttons.Count == 0) return;
            var newSuggestion = _currentSuggestion;
            while (newSuggestion >= 0 && newSuggestion < buttons.Count)
            {
                newSuggestion += step;
                if (newSuggestion >= 0 && newSuggestion < buttons.Count && buttons[newSuggestion].Visible)
                {
                    markSuggestion(buttons, newSuggestion);
                    return;
                }
            } 
        }

        private void markSuggestion(IButton oldButton, IButton newButton)
        {
            var lastTint = _lastSuggestionTint;
            if (oldButton != null && lastTint != null)
            {
                oldButton.Tint = lastTint.Value;
            }
            if (newButton != null)
            {
                _lastSuggestionTint = newButton.Tint;
                newButton.Tint = Colors.Orange;
            }
        }

        private void markSuggestion(List<IButton> buttons, int newSuggestion)
        {
            if (buttons.Count == 0) return;
            IButton oldButton = null;
            IButton newButton = null;
            if (_currentSuggestion >= 0 && _currentSuggestion < buttons.Count)
            {
                oldButton = buttons[_currentSuggestion];
            }
            if (newSuggestion >= 0 && newSuggestion < buttons.Count)
            {
                var button = buttons[newSuggestion];
                if (button.Visible) newButton = button;
            }
            MarkComboboxSuggestion?.Invoke(oldButton, newButton);
            if (newButton != null)
            {
                scrollToSuggestion(buttons, newButton, newSuggestion);
                _currentSuggestion = newSuggestion;
            }
        }

        private void scrollToSuggestion(List<IButton> buttons, IButton button, int index)
        {
            var crop = button.GetComponent<ICropSelfComponent>();
            if (crop == null) return;

            if (crop.CropArea.Height >= button.Height) return;
                
            var firstVisible = findFirstVisibleSuggestion(buttons);
            if (firstVisible < 0)
            {
                _scrolling.VerticalScrollBar.Value = _scrolling.VerticalScrollBar.MinValue;
                return;
            }

            int step = firstVisible > index ? -1 : 1;
            int retries = 1000;

            while (crop.CropArea.Height < button.Height && retries > 0 &&
                   (step < 0 || _scrolling.VerticalScrollBar.Value < _scrolling.VerticalScrollBar.MaxValue) &&
                   (step > 0 || _scrolling.VerticalScrollBar.Value > _scrolling.VerticalScrollBar.MinValue))
            {
                _scrolling.VerticalScrollBar.Value += step;
                retries--;
            }
        }

        private int findFirstVisibleSuggestion(List<IButton> buttons)
        {
            for (int index = 0; index < buttons.Count; index++)
            {
                var button = buttons[index];
                if (!button.Visible) continue;
                var crop = button.GetComponent<ICropSelfComponent>();
                if (crop != null && MathUtils.FloatEquals(crop.CropArea.Height, button.Height)) return index;
            }
            return -1;
        }

        private void onSelectedItemChanged(ListboxItemArgs args)
        {
            var textBox = TextBox;
            if (textBox != null) textBox.Text = args.Item == null ? "" : args.Item.Text;
            if (_dropDownPanelVisible != null) _dropDownPanelVisible.Visible = false;
            if (_currentlyUsingTextbox)
            {
                var buttons = _dropDownPanelList.ItemButtons.ToList();
                onTextboxTypingCompleted(buttons);
            }
        }

        private void refreshDropDownLayout()
        {
            var textbox = TextBox;
            var dropDownButton = DropDownButton;
            if (dropDownButton == null) return;
            if (textbox == null)
            {
                dropDownButton.X = 0;
            }
            else 
            {
                float dropBorderLeft = dropDownButton.Border?.WidthLeft ?? 0f;
                dropDownButton.X = textbox.X + (textbox.Width < 0 ? textbox.TextWidth : textbox.Width) + dropBorderLeft;
            }
        }

        private void onDropDownClicked(MouseButtonEventArgs args)
        {
            if (_dropDownPanelVisible != null) _dropDownPanelVisible.Visible = !_dropDownPanelVisible.Visible;
            if (_currentlyUsingTextbox)
            {
                var buttons = _dropDownPanelList.ItemButtons.ToList();
                onTextboxTypingCompleted(buttons);
            }
        }
    }
}
