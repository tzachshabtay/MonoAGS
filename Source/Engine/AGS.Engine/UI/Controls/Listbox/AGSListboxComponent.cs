using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AGS.API;
using PropertyChanged;

namespace AGS.Engine
{
    public class AGSListboxComponent : AGSComponent, IListboxComponent
    {
        private AGSBindingList<IStringItem> _items;
        private List<(IUIControl control, IStringItem item)> _itemControls;
        private IUIFactory _uiFactory;
        private int _selectedIndex;
        private float _minHeight, _maxHeight;
        private IScaleComponent _scale;
        private IInObjectTreeComponent _tree;
        private IImageComponent _image;
        private IGameState _state;
        private IStackLayoutComponent _layout;
        private string _searchFilter;

        public AGSListboxComponent(IUIFactory factory, IGameState state)
        {
            _state = state;
            _uiFactory = factory;
            _itemControls = new List<(IUIControl, IStringItem)>();
            _items = new AGSBindingList<IStringItem>(10);
            _items.OnListChanged.Subscribe(onListChanged);
            _selectedIndex = -1;
            _maxHeight = float.MaxValue;
            OnSelectedItemChanged = new AGSEvent<ListboxItemArgs>();
        }

        public override void Init()
        {
            base.Init();
            Entity.Bind<IScaleComponent>(c => _scale = c, _ => _scale = null);
            Entity.Bind<IInObjectTreeComponent>(c => _tree = c, _ => _tree = null);
            Entity.Bind<IImageComponent>(c => _image = c, _ => _image = null);
            Entity.Bind<IStackLayoutComponent>(c => 
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

        public Func<string, IUIControl> ListItemFactory { get; set; }

        public IEnumerable<IUIControl> ListItemUIControls => _itemControls.Select(c => c.control);

        public IAGSBindingList<IStringItem> Items => _items;

        [DoNotCheckEquality] //we skip equality checks as we want a combo box drop down to close even when we select the already selected item
        public int SelectedIndex
        {
            get => _selectedIndex;
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
            get => _minHeight;
            set 
            {
                if (MathUtils.FloatEquals(_minHeight, value)) return;
                _minHeight = value;
                refreshItemsLayout();
            }
        }

        public float MaxHeight
        {
            get => _maxHeight;
            set
            {
                if (MathUtils.FloatEquals(_maxHeight, value)) return;
                _maxHeight = value;
                refreshItemsLayout();
            }
        }

        public string SearchFilter
        {
            get => _searchFilter;
            set
            {
                value = value?.ToLowerInvariant() ?? "";
                _searchFilter = value;
                applySearch(value);
            }
        }

        public IBlockingEvent<ListboxItemArgs> OnSelectedItemChanged { get; }

        private void applySearch(string filter)
        {
            _layout?.StopLayout();
            foreach (var (control, item) in _itemControls)
            {
                var customSearch = item as ICustomSearchItem;
                if (customSearch != null)
                {
                    control.Visible = customSearch.Contains(filter);
                }
                else control.Visible = (item.Text?.ToLowerInvariant() ?? "").Contains(filter);
            }
            _layout?.StartLayout();
        }

        private void onListChanged(AGSListChangedEventArgs<IStringItem> args)
        {
            _layout?.StopLayout();
            var tree = _tree;
            if (args.ChangeType == ListChangeType.Remove)
            {
                var items = args.Items.OrderByDescending(i => i.Index);
                var controls = new List<(IUIControl, IStringItem)>(_itemControls);
                foreach (var item in items)
                {
                    var control = _itemControls[item.Index].control;
                    control.MouseClicked.Unsubscribe(onItemClicked);
                    tree?.TreeNode.RemoveChild(control);
                    _state.UI.Remove(control);
                    controls.RemoveAt(item.Index);
                }
                _itemControls = controls;
            }
            else
            {
                var items = args.Items.OrderBy(i => i.Index);
                var newControls = new List<IObject>(10);
                var controls = new List<(IUIControl, IStringItem)>(_itemControls);
                foreach (var item in items)
                {
                    var newControl = ListItemFactory(item.Item.Text);
                    newControl.MouseClicked.Subscribe(onItemClicked);
                    controls.Insert(item.Index, (newControl, item.Item));
                    newControls.Add(newControl);
                }
                _itemControls = controls;
                tree?.TreeNode.AddChildren(newControls);
            }
            refreshItemsLayout();
            _layout?.StartLayout();
        }

        private void onLayoutChanged()
        {
            refreshItemsLayout();
        }

        private void refreshItemsLayout()
        {
            if (_itemControls.Count == 0) return;
            var scale = _scale;
            if (scale == null) return;
            var visibleControls = _itemControls.Where(i => i.control.Visible).ToList();
            if (visibleControls.Count == 0) return;
            scale.BaseSize = new SizeF(visibleControls.Max(i => Math.Max(i.control.Width, i.control.GetComponent<ITextComponent>()?.TextWidth ?? 0f)),
                                       MathUtils.Clamp(visibleControls.Sum(i => Math.Max(i.control.Height, i.control.GetComponent<ITextComponent>()?.TextHeight ?? 0f)), _minHeight, _maxHeight));
            _layout.StartLocation = scale.Height;
            var image = _image;
            if (image == null) return;
            if (!MathUtils.FloatEquals(_image.Image.Width, scale.BaseSize.Width) ||
                !MathUtils.FloatEquals(_image.Image.Height, scale.BaseSize.Height))
            {
                _image.Image = new EmptyImage(scale.BaseSize.Width, scale.BaseSize.Height);
            }
        }

        private void onItemClicked(MouseButtonEventArgs args)
        {
            var control = args.ClickedEntity;
            for (int index = _itemControls.Count - 1; index >= 0; index--)
            {
                if (_itemControls[index].control == control)
                {
                    SelectedIndex = index;
                    return;
                }
            }
            SelectedIndex = -1;
        }
    }
}
