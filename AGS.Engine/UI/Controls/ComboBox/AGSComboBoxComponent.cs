using AGS.API;
using System;
using System.Collections.Generic;

namespace AGS.Engine
{
    public class AGSComboBoxComponent : AGSComponent, IComboBoxComponent
    {
        private AGSBindingList<object> _items;
        private List<IButton> _itemButtons;
        private IInObjectTree _tree;
        private ITextbox _textBox;
        private IButton _dropDownButton;
        private IUIFactory _uiFactory;
        private int _selectedIndex;

        public AGSComboBoxComponent(IUIFactory factory)
        {
            _uiFactory = factory;
            _itemButtons = new List<IButton>();
            _items = new AGSBindingList<object>(10);
            _items.OnListChanged.Subscribe(onListChanged);
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
                refreshDropDownLayout();
                if (newDropDownButton != null)
                {
                    newDropDownButton.MouseClicked.Subscribe(onDropDownClicked);
                }
            }
        }

        public ITextbox Textbox
        {
            get { return _textBox; }
            set { _textBox = value; refreshDropDownLayout(); }
        }

        public IPanel DropDownPanel { get; private set; }

        public Func<IButton> ItemButtonFactory { get; set; }

        public IEnumerable<IButton> ItemButtons { get { return _itemButtons; } }

        public IList<object> Items { get { return _items; } }

        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                _selectedIndex = value;
                var textBox = Textbox;
                if (value >= 0 && value < Items.Count && textBox != null)
                {
                    textBox.Text = Items[value].ToString();
                }
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

        public override void Init(IEntity entity)
        {
            base.Init(entity);
            DropDownPanel = _uiFactory.GetPanel(entity.ID + "_Panel", new EmptyImage(1f, 1f), 0f, 0f);
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
                var newButton = ItemButtonFactory();
                newButton.Text = args.Item.ToString();
                newButton.MouseClicked.Subscribe(onItemClicked);
                _itemButtons.Insert(args.Index, newButton);
                DropDownPanel.TreeNode.AddChild(newButton);
            }
            refreshItemsLayout();
        }

        private void refreshItemsLayout()
        {
            DropDownPanel.Y = -DropDownButton.Height;
            float y = 0f;
            foreach (var button in _itemButtons)
            {
                button.Y = y;
                y -= button.Height;
            }
        }

        private void refreshDropDownLayout()
        {
            var textbox = Textbox;
            var dropDownButton = DropDownButton;
            if (dropDownButton == null) return;
            dropDownButton.X = textbox == null ? 0f : textbox.Width;
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
