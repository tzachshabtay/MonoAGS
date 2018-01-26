using System;
using AGS.API;

namespace AGS.Engine
{
    public class EntityListSubscriptions<TEntity> where TEntity : IEntity
    {
        private readonly IEntitySubscription[] _subscriptions;
        private readonly bool _isTree;
        private readonly Action _onTreeChange;
        private readonly IAGSBindingList<TEntity> _list;

        public EntityListSubscriptions(IAGSBindingList<TEntity> list, bool isTree, Action onTreeChange, params IEntitySubscription[] subscriptions)
        {
            _list = list;
            _subscriptions = subscriptions;
            _isTree = isTree;
            _onTreeChange = onTreeChange;
            list.OnListChanged.Subscribe(onListChanged);
            foreach (var entity in list)
            {
                subscribeEntity(entity);
            }
        }

        public void Unsubscribe()
        {
            foreach (var entity in _list)
            {
                unsubscribeEntity(entity);
            }
        }

        private void onListChanged(AGSListChangedEventArgs<TEntity> args)
        {
            if (args.ChangeType == ListChangeType.Add)
            {
                foreach (var entity in args.Items)
                {
                    subscribeEntity(entity.Item);
                }
            }
            else
            {
                foreach (var entity in args.Items)
                {
                    unsubscribeEntity(entity.Item);
                }
            }
            _onTreeChange?.Invoke();
        }

        private void onListChanged(AGSListChangedEventArgs<IObject> args)
        {
            if (args.ChangeType == ListChangeType.Add)
            {
                foreach (var entity in args.Items)
                {
                    subscribeEntity(entity.Item);
                }
            }
            else
            {
                foreach (var entity in args.Items)
                {
                    unsubscribeEntity(entity.Item);
                }
            }
            _onTreeChange?.Invoke();
        }

        private void subscribeEntity(IEntity entity)
        {
            foreach (var subscription in _subscriptions)
            {
                subscription.Subscribe(entity);
            }
            if (_isTree)
            {
                var treeComponent = entity.GetComponent<IInObjectTreeComponent>();
                if (treeComponent != null)
                {
                    treeComponent.TreeNode.Children.OnListChanged.Subscribe(onListChanged);
                    foreach (var child in treeComponent.TreeNode.Children)
                    {
                        subscribeEntity(child);
                    }
                }
            }
        }

        private void unsubscribeEntity(IEntity entity)
        {
            foreach (var subscription in _subscriptions)
            {
                subscription.Unsubscribe(entity);
            }
            if (_isTree)
            {
                var treeComponent = entity.GetComponent<IInObjectTreeComponent>();
                if (treeComponent != null)
                {
                    treeComponent.TreeNode.Children.OnListChanged.Unsubscribe(onListChanged);
                    foreach (var child in treeComponent.TreeNode.Children)
                    {
                        unsubscribeEntity(child);
                    }
                }
            }
        }
    }
}
