using AGS.API;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AGS.Engine
{
    public class AGSComboBoxComponent : AGSComponent, IComboBoxComponent
    {
        private AGSBindingList<object> _items;
        private List<IButton> _itemButtons;
        private IInObjectTree _tree;
        private ITextBox _textBox;
        private IButton _dropDownButton;
        private IUIFactory _uiFactory;
        private int _selectedIndex;

        public AGSComboBoxComponent(IUIFactory factory, IGameEvents gameEvents)
        {
            _uiFactory = factory;
            _itemButtons = new List<IButton>();
            _items = new AGSBindingList<object>(10);
            _items.OnListChanged.Subscribe(onListChanged);
            OnSelectedItemChanged = new AGSEvent<ComboboxItemArgs>();
            gameEvents.OnRepeatedlyExecute.Subscribe((_, __) => refreshDropDownLayout());
        }

        public IButton DropDownButton
        {
            get { return _dropDownButton; }
            set
            {
                if (_dropDownButton == value) return;
                var oldDropDownButton = _dropDownButton;
                var newDropDownButton = value;
                if (oldDropDownButton != null) oldDropDownButton.MouseClicked.Unsubscribe(onDropDownClicked);
                _dropDownButton = newDropDownButton;
                if (newDropDownButton != null)
                {
                    if (DropDownPanel != null)
                    {
                        DropDownPanel.RenderLayer = value.RenderLayer;
                    }
                    newDropDownButton.MouseClicked.Subscribe(onDropDownClicked);
                }
            }
        }

        public ITextBox TextBox
        {
            get { return _textBox; }
            set { _textBox = value; }
        }

        public IPanel DropDownPanel { get; private set; }

        public Func<string, IButton> ItemButtonFactory { get; set; }

        public IEnumerable<IButton> ItemButtons { get { return _itemButtons; } }

        public IList<object> Items { get { return _items; } }

        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                _selectedIndex = value;
                var textBox = TextBox;
                if (value >= 0 && value < Items.Count)
                {
                    var selectedItem = Items[value];
                    if (textBox != null) textBox.Text = selectedItem.ToString();
                    OnSelectedItemChanged.Invoke(this, new ComboboxItemArgs(selectedItem, value));
                }
                else OnSelectedItemChanged.Invoke(this, new ComboboxItemArgs(null, value));
            }
        }

        public object SelectedItem
        {
            get
            {
                try
                {
                    return Items[SelectedIndex];
                }
                catch (IndexOutOfRangeException)
                {
                    return null;
                }
            }
        }

        public IEvent<ComboboxItemArgs> OnSelectedItemChanged { get; private set; }

        public override void Init(IEntity entity)
        {
            base.Init(entity);
            DropDownPanel = _uiFactory.GetPanel(entity.ID + "_Panel", new EmptyImage(1f, 1f), 0f, 0f);
            DropDownPanel.Anchor = new PointF(0f, 1f);
            DropDownPanel.Y = -1f;
            DropDownPanel.AddComponent<IStackLayoutComponent>().RelativeSpacing = 1f;
            DropDownPanel.Visible = false;
            _tree = entity.GetComponent<IInObjectTree>();
        }

        private void onListChanged(object sender, AGSListChangedEventArgs<object> args)
        {
            if (args.ChangeType == ListChangeType.Remove)
            {
                var button = _itemButtons[args.Index];
                button.MouseClicked.Unsubscribe(onItemClicked);
                DropDownPanel.TreeNode.RemoveChild(button);
                _itemButtons.RemoveAt(args.Index);
            }
            else
            {
                string buttonText = args.Item.ToString();
                var newButton = ItemButtonFactory(buttonText);
                newButton.Text = buttonText;
                newButton.MouseClicked.Subscribe(onItemClicked);
                _itemButtons.Insert(args.Index, newButton);
                DropDownPanel.TreeNode.AddChild(newButton);
            }
            refreshItemsLayout();
        }

        private void refreshItemsLayout()
        {
            if (_itemButtons.Count == 0) return;
            DropDownPanel.ResetBaseSize(_itemButtons.Max(i => Math.Max(i.Width, i.TextWidth)), 
                                        _itemButtons.Sum(i => Math.Max(i.Height, i.TextHeight)));
            if (DropDownPanel.Image.Width != DropDownPanel.BaseSize.Width ||
                DropDownPanel.Image.Height != DropDownPanel.BaseSize.Height)
            {
                DropDownPanel.Image = new EmptyImage(DropDownPanel.BaseSize.Width, DropDownPanel.BaseSize.Height);
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
                float dropBorderLeft = 0f;
                if (dropDownButton.Border != null) dropBorderLeft = dropDownButton.Border.WidthLeft;
                dropDownButton.X = textbox.X + (textbox.Width < 0 ? textbox.TextWidth : textbox.Width) + dropBorderLeft;
            }
        }

        private void onItemClicked(object sender, MouseButtonEventArgs args)
        {
            SelectedIndex = _itemButtons.IndexOf((IButton)sender);
            hidePanel();
        }

        private void onDropDownClicked(object sender, MouseButtonEventArgs args)
        {
            if (_tree.TreeNode.HasChild(DropDownPanel)) hidePanel();
            else showPanel();
        }

        private void hidePanel()
        {            
            DropDownPanel.Visible = false;
            _tree.TreeNode.RemoveChild(DropDownPanel);
        }

        private void showPanel()
        {            
            DropDownPanel.Visible = true;
            _tree.TreeNode.AddChild(DropDownPanel);
        }
    }
}
