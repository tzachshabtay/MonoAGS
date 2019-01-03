using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using AGS.API;
using PropertyChanged;

namespace AGS.Engine
{
    public class AGSListboxComponent : AGSComponent, IListboxComponent
    {
        private AGSBindingList<IStringItem> _items;
        private List<(IUIControl control, IStringItem item)> _itemControls;
        private int _selectedIndex;
        private float _minHeight, _maxHeight, _minWidth, _maxWidth;
        private SizeF _padding;
        private IScaleComponent _scale;
        private IInObjectTreeComponent _tree;
        private IImageComponent _image;
        private IGameState _state;
        private IStackLayoutComponent _layout;
        private IVisibleComponent _visible;
        private string _searchFilter;
        private List<AGSListChangedEventArgs<IStringItem>> _incomingChanges = new List<AGSListChangedEventArgs<IStringItem>>();

        public AGSListboxComponent(IGameState state)
        {
            _state = state;
            _itemControls = new List<(IUIControl, IStringItem)>();
            _items = new AGSBindingList<IStringItem>(10);
            _items.OnListChanged.Subscribe(onListChanged);
            _selectedIndex = -1;
            _maxHeight = _maxWidth = float.MaxValue;
            _padding = new SizeF(3f, 3f);
            OnSelectedItemChanged = new AGSEvent<ListboxItemArgs>();
            OnSelectedItemChanging = new AGSEvent<ListboxItemChangingArgs>();
        }

        public override void Init()
        {
            base.Init();
            Entity.Bind<IScaleComponent>(c => _scale = c, _ => _scale = null);
            Entity.Bind<IInObjectTreeComponent>(c => _tree = c, _ => _tree = null);
            Entity.Bind<IImageComponent>(c => _image = c, _ => _image = null);
            Entity.Bind<IVisibleComponent>(c =>
            {
                _visible = c;
                c.PropertyChanged += onVisibleChanged;
                applyAllChangesIfNeeded();
            }, c =>
            {
                _visible = null;
                c.PropertyChanged -= onVisibleChanged;
            });
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
                IStringItem item = null;
                if (value >= 0 && value < Items.Count)
                {
                    item = Items[value];
                }
                select(item, value); 
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

        public float MinWidth
        {
            get => _minWidth;
            set
            {
                if (MathUtils.FloatEquals(_minWidth, value))
                    return;
                _minWidth = value;
                refreshItemsLayout();
            }
        }

        public float MaxWidth
        {
            get => _maxWidth;
            set
            {
                if (MathUtils.FloatEquals(_maxWidth, value))
                    return;
                _maxWidth = value;
                refreshItemsLayout();
            }
        }

        public SizeF Padding
        {
            get => _padding;
            set
            {
                if (_padding.Equals(value))
                    return;
                _padding = value;
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

        public IEvent<ListboxItemChangingArgs> OnSelectedItemChanging { get; }

        private void onVisibleChanged(object sender, PropertyChangedEventArgs e)
        {
            applyAllChangesIfNeeded();
        }

        private async void select(IStringItem item, int index)
        {
            ListboxItemChangingArgs args = new ListboxItemChangingArgs(item, index);
            await OnSelectedItemChanging.InvokeAsync(args);
            if (args.ShouldCancel) return;

            _selectedIndex = index;
            OnSelectedItemChanged.Invoke(new ListboxItemArgs(item, index));
        }

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
            _layout?.ForceRefreshLayout();
            refreshItemsLayout();
        }

        private void onListChanged(AGSListChangedEventArgs<IStringItem> args)
        {
            _incomingChanges.Add(args);
            applyAllChangesIfNeeded();
        }

        private void applyAllChangesIfNeeded()
        {
            if (!_visible.Visible) return;
            var newIncoming = new List<AGSListChangedEventArgs<IStringItem>>(_incomingChanges.Capacity);
            var oldIncoming = Interlocked.Exchange(ref _incomingChanges, newIncoming);
            _layout?.StopLayout();
            foreach (var change in oldIncoming)
            {
                applyChange(change);
            }

            //refresh items layout is called twice: first time to update layout start location, second time to update drop panel size
            refreshItemsLayout();
            _layout?.StartLayout();
            _layout?.ForceRefreshLayout();
            refreshItemsLayout();
        }

        private void applyChange(AGSListChangedEventArgs<IStringItem> args)
        {
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
                    control.DestroyWithChildren();
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
            float potentialMaxWidth = getPotentialMaxWidth(visibleControls);
            scale.BaseSize = new SizeF(MathUtils.Clamp(Math.Max(potentialMaxWidth, visibleControls.Max(i => Math.Max(i.control.Width, i.control.GetComponent<ITextComponent>()?.TextWidth ?? 0f))) + _padding.Width, _minWidth, _maxWidth),
                                       MathUtils.Clamp(visibleControls.Sum(i => Math.Max(i.control.Height, i.control.GetComponent<ITextComponent>()?.TextHeight ?? 0f)) + _padding.Height, _minHeight, _maxHeight));
            _layout.StartLocation = scale.Height;
            var image = _image;
            if (image == null) return;
            if (!MathUtils.FloatEquals(_image.Image.Width, scale.BaseSize.Width) ||
                !MathUtils.FloatEquals(_image.Image.Height, scale.BaseSize.Height))
            {
                _image.Image = new EmptyImage(scale.BaseSize.Width, scale.BaseSize.Height);
            }
        }

        private float getPotentialMaxWidth(List<(IUIControl control, IStringItem item)> visibleControls)
        {
            //We should guarantee that the item with the longest text is measured even if it's cropped,
            //as that's probably the item with the maximum width which will give us the width of our container.
            (IUIControl control, IStringItem item) maxControl = default;
            int maxText = int.MinValue;
            foreach (var control in visibleControls)
            {
                if (control.item.Text.Length > maxText)
                {
                    maxText = control.item.Text.Length;
                    maxControl = control;
                }
            }
            if (maxText <= 0) return 0f;
            var textComponent = maxControl.control.GetComponent<ITextComponent>();
            if (textComponent == null) return 0f;
            var crop = maxControl.control.AddComponent<ICropSelfComponent>();
            crop.NeverGuaranteedToFullyCrop = true;
            if (textComponent.CustomTextCrop != null)
            {
                textComponent.CustomTextCrop.NeverGuaranteedToFullyCrop = true;
            }
            crop.CropEnabled = false;
            textComponent.PrepareTextBoundingBoxes();
            return textComponent.TextWidth;
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
