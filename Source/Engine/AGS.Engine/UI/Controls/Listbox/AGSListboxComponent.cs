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
        private float _minHeight, _maxHeight;
        private IScaleComponent _scale;
        private IInObjectTree _tree;
        private IImageComponent _image;
        private IGameState _state;
        private IStackLayoutComponent _layout;

        public AGSListboxComponent(IUIFactory factory, IGameState state)
        {
            _state = state;
            _uiFactory = factory;
            _itemButtons = new List<IButton>();
            _items = new AGSBindingList<IStringItem>(10);
            _items.OnListChanged.Subscribe(onListChanged);
            _selectedIndex = -1;
            _maxHeight = float.MaxValue;
            OnSelectedItemChanged = new AGSEvent<ListboxItemArgs>();
        }

        public override void Init(IEntity entity)
        {
            base.Init(entity);
            entity.Bind<IScaleComponent>(c => _scale = c, _ => _scale = null);
            entity.Bind<IInObjectTree>(c => _tree = c, _ => _tree = null);
            entity.Bind<IImageComponent>(c => _image = c, _ => _image = null);
            entity.Bind<IStackLayoutComponent>(c => 
            { 
                c.RelativeSpacing = -1f; 
                c.OnLayoutChanged.Subscribe(onLayoutChanged); 
                c.StartLayout();
                _layout = c;
            }, c => 
            {
                c.StopLayout();
                c.OnLayoutChanged.Unsubscribe(onLayoutChanged);
                _layout = null;
            });
        }

        public Func<string, IButton> ItemButtonFactory { get; set; }
        
        public IEnumerable<IButton> ItemButtons { get { return _itemButtons; } }
        
        public IAGSBindingList<IStringItem> Items { get { return _items; } }

        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                _selectedIndex = value;
                if (value >= 0 && value < Items.Count)
                {
                    var selectedItem = Items[value];
                    OnSelectedItemChanged.Invoke(new ListboxItemArgs(selectedItem, value));
                }
                else OnSelectedItemChanged.Invoke(new ListboxItemArgs(null, value));    
            }
        }

        public IStringItem SelectedItem
        {
            get
            {
                try
                {
                    if (SelectedIndex < 0) return null;
                    return Items[SelectedIndex];
                }
                catch (IndexOutOfRangeException)
                {
                    return null;
                }
                catch (ArgumentOutOfRangeException)
                {
                    return null;
                }
            }
        }

        public float MinHeight 
        {
            get { return _minHeight; }
            set 
            {
#pragma warning disable RECS0018 // Comparison of floating point numbers with equality operator
                if (_minHeight == value) return;
#pragma warning restore RECS0018 // Comparison of floating point numbers with equality operator
                _minHeight = value;
                refreshItemsLayout();
            }
        }

        public float MaxHeight
        {
            get { return _maxHeight; }
            set
            {
#pragma warning disable RECS0018 // Comparison of floating point numbers with equality operator
                if (_maxHeight == value) return;
#pragma warning restore RECS0018 // Comparison of floating point numbers with equality operator
                _maxHeight = value;
                refreshItemsLayout();
            }
        }

        public IEvent<ListboxItemArgs> OnSelectedItemChanged { get; private set; }

        private void onListChanged(AGSListChangedEventArgs<IStringItem> args)
        {
            var tree = _tree;
            if (args.ChangeType == ListChangeType.Remove)
            {
                var items = args.Items.OrderByDescending(i => i.Index);
                foreach (var item in items)
                {
                    var button = _itemButtons[item.Index];
                    button.MouseClicked.Unsubscribe(onItemClicked);
                    if (tree != null) tree.TreeNode.RemoveChild(button);
                    _state.UI.Remove(button);
                    _itemButtons.RemoveAt(item.Index);
                }
            }
            else
            {
                var items = args.Items.OrderBy(i => i.Index);
                var newButtons = new List<IObject>(10);
                foreach (var item in items)
                {
                    string buttonText = item.Item.Text;
                    var newButton = ItemButtonFactory(buttonText);
                    newButton.Text = buttonText;
                    newButton.MouseClicked.Subscribe(onItemClicked);
                    _itemButtons.Insert(item.Index, newButton);
                    newButtons.Add(newButton);
                }
                if (tree != null) tree.TreeNode.AddChildren(newButtons);
            }
            refreshItemsLayout();
        }

        private void onLayoutChanged()
        {
            refreshItemsLayout();
        }

        private void refreshItemsLayout()
        {
            if (_itemButtons.Count == 0) return;
            var scale = _scale;
            if (scale == null) return;
            scale.BaseSize = new SizeF(_itemButtons.Max(i => Math.Max(i.Width, i.TextWidth)),
                                       MathUtils.Clamp(_itemButtons.Sum(i => Math.Max(i.Height, i.TextHeight)), _minHeight, _maxHeight));
            _layout.StartLocation = scale.Height;
            var image = _image;
            if (image == null) return;
#pragma warning disable RECS0018 // Comparison of floating point numbers with equality operator
            if (_image.Image.Width != scale.BaseSize.Width ||
                _image.Image.Height != scale.BaseSize.Height)
#pragma warning restore RECS0018 // Comparison of floating point numbers with equality operator
            {
                _image.Image = new EmptyImage(scale.BaseSize.Width, scale.BaseSize.Height);
            }
        }

        private void onItemClicked(MouseButtonEventArgs args)
        {
            SelectedIndex = _itemButtons.IndexOf((IButton)args.ClickedEntity);
        }
    }
}
