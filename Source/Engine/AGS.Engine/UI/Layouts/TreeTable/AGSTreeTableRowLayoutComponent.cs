using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using AGS.API;

namespace AGS.Engine
{
    public class AGSTreeTableRowLayoutComponent : AGSComponent, ITreeTableRowLayoutComponent
    {
        private ITreeTableLayout _table;
        private IInObjectTreeComponent _tree;
        private IVisibleComponent _visible;
        private EntityListSubscriptions<IObject> _subscriptions;
        private List<float> _columnSizes;

        public AGSTreeTableRowLayoutComponent(IRestrictionList restrictionList)
        {
            RestrictionList = restrictionList;
            if (restrictionList != null) restrictionList.PropertyChanged += onRestrictionListChanged;
            _columnSizes = new List<float>();
        }

        public ITreeTableLayout Table
        {
            get => _table;
            set 
            {
                unsubscribeTable();
                _table = value;
                value.OnQueryLayout.Subscribe(onQueryLayout);
                value.OnRefreshLayoutNeeded.Subscribe(onRefreshLayout);
                value.Rows.Add(this);
            }
        }

        public IRestrictionList RestrictionList { get; private set; }

        public override void Init()
        {
            base.Init();
            Entity.Bind<IVisibleComponent>(c => 
            { 
                _visible = c; 
                c.PropertyChanged += onVisiblePropertyChanged;
                _table?.PerformLayout();
            }, c => 
            { 
                _visible = null;
                c.PropertyChanged -= onVisiblePropertyChanged;
                _table?.PerformLayout();
            });

            Entity.Bind<IInObjectTreeComponent>(c => 
            {
                _tree = c;
                calculateColumns();
            }, _ => _tree = null);

            subscribeChildren();
        }

        public override void Dispose()
        {
            base.Dispose();
            unsubscribeChildren();
            unsubscribeTable();

            var restrictionList = RestrictionList;
            if (restrictionList != null)
            {
                restrictionList.PropertyChanged -= onRestrictionListChanged;
                RestrictionList = null;
            }
        }

        private void subscribeChildren()
        {
            var boundingBoxSubscription = new EntitySubscription<IBoundingBoxWithChildrenComponent>(null,
                c => c.OnBoundingBoxWithChildrenChanged.Subscribe(onSizeChanged),
                c => c.OnBoundingBoxWithChildrenChanged.Unsubscribe(onSizeChanged));

            var visibleSubscription = new EntitySubscription<IVisibleComponent>(onVisibleChanged, propertyNames: nameof(IVisibleComponent.Visible));

            _subscriptions = new EntityListSubscriptions<IObject>(_tree.TreeNode.Children, false, calculateColumns, boundingBoxSubscription, visibleSubscription);
        }

        private void onSizeChanged()
        {
            calculateColumns();
        }

        private void onVisibleChanged()
        {
            calculateColumns();
        }

        private void onRestrictionListChanged(object sender, PropertyChangedEventArgs args)
        {
            calculateColumns();
        }

        private void calculateColumns()
        {
            var tree = _tree;
            if (tree == null)
            { 
                return; 
            }
            if (tree.TreeNode.ChildrenCount > _columnSizes.Count)
            {
                int diff = tree.TreeNode.ChildrenCount - _columnSizes.Count;
                while (diff > 0)
                {
                    diff--;
                    _columnSizes.Add(0f);
                }
            }
            if (tree.TreeNode.ChildrenCount < _columnSizes.Count)
            {
                int diff = _columnSizes.Count - tree.TreeNode.ChildrenCount;
                while (diff > 0)
                {
                    diff--;
                    _columnSizes.Remove(_columnSizes[0]);
                }
            }
            for (int index = 0; index < tree.TreeNode.ChildrenCount; index++)
            {
                var child = tree.TreeNode.Children[index];
                var width = isRestricted(child) ? 0f :
                    child.AddComponent<IBoundingBoxWithChildrenComponent>()?.PreCropBoundingBoxWithChildren.Width ?? 0f;
                _columnSizes[index] = width;
            }
            _table?.PerformLayout();
        }

        private bool isRestricted(IEntity entity)
        {
            return RestrictionList?.IsRestricted(entity.ID) ?? false;
        }

        private void unsubscribeChildren()
        {
            _subscriptions?.Unsubscribe();
            _subscriptions = null;
        }

        private void unsubscribeTable()
        {
            _table?.OnRefreshLayoutNeeded?.Unsubscribe(onRefreshLayout);
            _table?.OnQueryLayout?.Unsubscribe(onQueryLayout);
        }

        private void onVisiblePropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName != nameof(IVisibleComponent.Visible)) return;
            _table?.PerformLayout();
        }

        private void onQueryLayout(QueryLayoutEventArgs args)
        {
            if (!(_visible?.Visible ?? true) || _columnSizes == null) return;
            if (_columnSizes.Count > args.ColumnSizes.Count)
            {
                int diff = _columnSizes.Count - args.ColumnSizes.Count;
                while (diff > 0)
                {
                    diff--;
                    args.ColumnSizes.Add(0f);
                }
            }
            for (int index = 0; index < _columnSizes.Count; index++)
            {
                if (_columnSizes[index] > args.ColumnSizes[index])
                {
                    args.ColumnSizes[index] = _columnSizes[index];
                }
            }
        }

        private void onRefreshLayout()
        {
            var table = Table;
            var tree = _tree;
            if (table == null || tree == null) return;
            float x = table.StartX;
            int length = Math.Min(tree.TreeNode.ChildrenCount, table.ColumnSizes.Count);
            for (int index = 0; index < length; index++)
            {
                var child = tree.TreeNode.Children[index];
                child.X = x;
                var width = isRestricted(child) ? child.AddComponent<IBoundingBoxWithChildrenComponent>()?.PreCropBoundingBoxWithChildren.Width ?? 0f : 
                                                table.ColumnSizes[index];
                x += width + table.ColumnPadding;
            }
        }
    }
}
