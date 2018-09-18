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
        private readonly IInput _input;
        private readonly IGameEvents _gameEvents;
        private IButton _dropDownButton;
        private IListboxComponent _dropDownPanelList;
        private IVisibleComponent _dropDownPanelVisible;
        private IScrollingComponent _scrolling;
        private IListbox _dropDownPanel;
        private ComboSuggest _suggestMode;
        private ITextBox _textbox;
        private int _currentSuggestion = -1;
        private Color? _lastSuggestionTint;
        private bool _currentlyNavigatingSuggestions;
        private bool _currentlyUsingTextbox;

        public AGSComboBoxComponent(IGameEvents gameEvents, IInput input)
        {
            _input = input;
            _gameEvents = gameEvents;
            MarkComboboxSuggestion = markSuggestion;
            gameEvents.OnRepeatedlyExecute.Subscribe(refreshDropDownLayout);
            input.MouseDown.Subscribe(onMouseDown);
        }

        public override void Dispose()
        {
            base.Dispose();
            _input?.MouseDown.Unsubscribe(onMouseDown);
            _gameEvents?.OnRepeatedlyExecute.Unsubscribe(refreshDropDownLayout);
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

        public IListboxComponent DropDownPanelList => _dropDownPanel.ListboxComponent;

        public ComboSuggest SuggestMode 
        { 
            get => _suggestMode; 
            set
            {
                _suggestMode = value;
                updateTextbox();
            }
        }

        public IListbox DropDownPanel 
        {
            get => _dropDownPanel;
            set 
            {
                _dropDownPanel = value;
                var panel = value.ContentsPanel;
                var scrollingContainer = value.ScrollingPanel ?? panel;
                var visibleComponent = scrollingContainer.GetComponent<IVisibleComponent>();
                var listBoxComponent = panel.GetComponent<IListboxComponent>();
                var scrollingImageComponent = scrollingContainer.GetComponent<IImageComponent>();
                var imageComponent = panel.GetComponent<IImageComponent>();
                _scrolling = panel.GetComponent<IScrollingComponent>();
                _dropDownPanelVisible = visibleComponent;

                _dropDownPanelList?.OnSelectedItemChanged.Unsubscribe(onSelectedItemChanged);
                _dropDownPanelList = listBoxComponent;
                listBoxComponent?.OnSelectedItemChanged.Subscribe(onSelectedItemChanged);
                if (scrollingImageComponent != null) scrollingImageComponent.Pivot = new PointF(0f, 1f);
                if (visibleComponent != null) visibleComponent.Visible = false;
            }
        }

        public Action<IButton, IButton> MarkComboboxSuggestion { get; set; }

        private void onMouseDown(MouseButtonEventArgs args)
        {
            if (!(_dropDownPanelVisible?.Visible ?? false)) return;
            var clicked = args.ClickedEntity;
            if (clicked != null)
            {
                if (clicked == _dropDownButton || clicked == _textbox || 
                    clicked == _scrolling?.VerticalScrollBar.Graphics || clicked == _scrolling?.VerticalScrollBar.HandleGraphics ||
                    clicked == _scrolling?.HorizontalScrollBar.Graphics || clicked == _scrolling?.HorizontalScrollBar.HandleGraphics ||
                    clicked == _dropDownPanel || _dropDownPanelList.ListItemUIControls.Any(c => c == clicked))
                {
                    return;
                }
            }
            setPanelVisible(false);
        }

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
            var buttons = dropDownPanel.ListItemUIControls.Cast<IButton>().ToList();
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
                scrollToSuggestion();
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
            newSuggestion = step > 0 ? 0 : buttons.Count - 1;
            while (newSuggestion >= 0 && newSuggestion < buttons.Count && newSuggestion != _currentSuggestion)
            {
                if (newSuggestion >= 0 && newSuggestion < buttons.Count && buttons[newSuggestion].Visible)
                {
                    markSuggestion(buttons, newSuggestion);
                    return;
                }
                newSuggestion += step;
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
                _currentSuggestion = newSuggestion;
                scrollToSuggestion();
            }
        }

        private async void scrollToSuggestion()
        {
            if (!_currentlyUsingTextbox || _currentSuggestion < 0) return;
            await Task.Delay(500); //waiting for buttons on drop-down to change visibility based on the filter -> todo: find a better way
            List<IButton> buttons = _dropDownPanelList.ListItemUIControls.Cast<IButton>().ToList();
            if (_currentSuggestion >= buttons.Count) return;
            var button = buttons[_currentSuggestion];
            var crop = button.GetComponent<ICropSelfComponent>();
            if (crop == null) return;

            if (crop.CropArea.Height >= button.Height) return;

            var visibleButtons = buttons.Where(b => b.Visible).ToList();
            if (visibleButtons.Count == 0) return;
            int index = visibleButtons.IndexOf(button);
            if (index < 0) return;

            var totalHeight = button.Height * visibleButtons.Count;
            var heightToReachButton = button.Height * index;

            var sliderValue = MathUtils.Lerp(0f, _scrolling.VerticalScrollBar.MinValue,
                                             totalHeight, _scrolling.VerticalScrollBar.MaxValue, heightToReachButton);

            _scrolling.VerticalScrollBar.Value = sliderValue;                
        }

        private void onSelectedItemChanged(ListboxItemArgs args)
        {
            var textBox = TextBox;
            if (textBox != null) textBox.Text = args.Item == null ? "" : args.Item.Text;
            if (_dropDownPanelVisible != null) _dropDownPanelVisible.Visible = false;
            if (_currentlyUsingTextbox)
            {
                var buttons = _dropDownPanelList.ListItemUIControls.Cast<IButton>().ToList();
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
            setPanelVisible(!(_dropDownPanelVisible?.Visible ?? false));
        }

        private void setPanelVisible(bool visible)
        {
            if (_dropDownPanelVisible != null)
            {
                _dropDownPanelVisible.Visible = visible;
            }
            if (_currentlyUsingTextbox)
            {
                var buttons = _dropDownPanelList.ListItemUIControls.Cast<IButton>().ToList();
                onTextboxTypingCompleted(buttons);
            }
        }
    }
}
