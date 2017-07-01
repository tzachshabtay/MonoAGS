using System;
using System.Collections.Generic;
using System.Linq;
using AGS.API;

namespace AGS.Engine
{
    public class AGSListboxComponent : AGSComponent, IListboxComponent
    {
        private AGSBindingList<IStringItem> _items;
        private List<IButton> _itemButtons;
        private IUIFactory _uiFactory;
        private int _selectedIndex;
        private IScaleComponent _scale;
        private IInObjectTree _tree;
        private IImageComponent _image;

        public AGSListboxComponent(IUIFactory factory)
        {
            _uiFactory = factory;
            _itemButtons = new List<IButton>();
            _items = new AGSBindingList<IStringItem>(10);
            _items.OnListChanged.Subscribe(onListChanged);
            OnSelectedItemChanged = new AGSEvent<ListboxItemArgs>();
        }

        public override void Init(IEntity entity)
        {
            base.Init(entity);
            _scale = entity.GetComponent<IScaleComponent>();
            _tree = entity.GetComponent<IInObjectTree>();
            _image = entity.GetComponent<IImageComponent>();

            var stackLayout = entity.GetComponent<IStackLayoutComponent>();
            stackLayout.RelativeSpacing = 1f;
            stackLayout.StartLayout();
        }

        public Func<string, IButton> ItemButtonFactory { get; set; }
        
        public IEnumerable<IButton> ItemButtons { get { return _itemButtons; } }
        
        public IList<IStringItem> Items { get { return _items; } }

        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                _selectedIndex = value;
                if (value >= 0 && value < Items.Count)
                {
                    var selectedItem = Items[value];
                    OnSelectedItemChanged.Invoke(this, new ListboxItemArgs(selectedItem, value));
                }
                else OnSelectedItemChanged.Invoke(this, new ListboxItemArgs(null, value));    
            }
        }

        public IStringItem SelectedItem
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

        public IEvent<ListboxItemArgs> OnSelectedItemChanged { get; private set; }

        private void onListChanged(object sender, AGSListChangedEventArgs<IStringItem> args)
        {
            if (args.ChangeType == ListChangeType.Remove)
            {
                var items = args.Items.OrderByDescending(i => i.Index);
                foreach (var item in items)
                {
                    var button = _itemButtons[item.Index];
                    button.MouseClicked.Unsubscribe(onItemClicked);
                    _tree.TreeNode.RemoveChild(button);
                    _itemButtons.RemoveAt(item.Index);
                }
            }
            else
            {
                var items = args.Items.OrderBy(i => i.Index);
                foreach (var item in items)
                {
                    string buttonText = item.Item.Text;
                    var newButton = ItemButtonFactory(buttonText);
                    newButton.Text = buttonText;
                    newButton.MouseClicked.Subscribe(onItemClicked);
                    _itemButtons.Insert(item.Index, newButton);
                    _tree.TreeNode.AddChild(newButton);
                }
            }
            refreshItemsLayout();
        }

        private void refreshItemsLayout()
        {
            if (_itemButtons.Count == 0) return;
            _scale.ResetBaseSize(_itemButtons.Max(i => Math.Max(i.Width, i.TextWidth)),
                                        _itemButtons.Sum(i => Math.Max(i.Height, i.TextHeight)));
            if (_image.Image.Width != _scale.BaseSize.Width ||
                _image.Image.Height != _scale.BaseSize.Height)
            {
                _image.Image = new EmptyImage(_scale.BaseSize.Width, _scale.BaseSize.Height);
            }
        }

        private void onItemClicked(object sender, MouseButtonEventArgs args)
        {
	        SelectedIndex = _itemButtons.IndexOf((IButton)sender);
        }
    }
}
