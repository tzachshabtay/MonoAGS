using System;
using AGS.API;

namespace AGS.Engine
{
    public class AGSComponentBinding<TComponent> : IComponentBinding where TComponent : IComponent
    {
        private readonly Action<TComponent> onAdded, onRemoved;
        private readonly IComponentsCollection _collection;
        private TComponent _component;
        private readonly Action<AGSListChangedEventArgs<IComponent>> _componentChangedCallback;
        private readonly Action _componentsInitializedCallback;

        public AGSComponentBinding(IComponentsCollection collection, Action<TComponent> onAdded, 
                                   Action<TComponent> onRemoved)
        {
            _collection = collection;
            _componentChangedCallback = onComponentsChanged;
            _componentsInitializedCallback = onComponentsInitialized;
            this.onAdded = onAdded;
            this.onRemoved = onRemoved;
            collection.OnComponentsChanged.Subscribe(_componentChangedCallback);
            _component = collection.GetComponent<TComponent>();
            if (_component != null) onAdded.Invoke(_component);
            else if (!collection.ComponentsInitialized) collection.OnComponentsInitialized.Subscribe(_componentsInitializedCallback);
        }

        private void onComponentsChanged(AGSListChangedEventArgs<IComponent> args)
        {
            if (args.ChangeType == ListChangeType.Add && onAdded != null)
			{
				foreach (var component in args.Items)
				{
                    if (component.Item is TComponent boundComponent)
                    {
                        _component = boundComponent;
                        onAdded(boundComponent);
                        return;
                    }
				}
			}
            else if (args.ChangeType == ListChangeType.Remove && onRemoved != null)
			{
				foreach (var component in args.Items)
				{
                    if (component.Item is TComponent boundComponent)
                    {
                        onRemoved(boundComponent);
                        _component = default;
                        if (!_collection.ComponentsInitialized)
                        {
                            _collection.OnComponentsInitialized.Subscribe(onComponentsInitialized);
                        }
                        break;
                    }
				}
			}
        }

        private void onComponentsInitialized()
        {
            if (onAdded == null) return;
            foreach (var component in _collection)
            {
                if (component is TComponent boundComponent)
                {
                    onAdded(boundComponent);
                    _component = boundComponent;
                    return;
                }
            }
        }

        public void Unbind()
        {
            _collection.OnComponentsChanged.Unsubscribe(_componentChangedCallback);
            _collection.OnComponentsInitialized.Unsubscribe(_componentsInitializedCallback);
            if (onRemoved == null) return;
            var boundComponent = _component;
            if (boundComponent == null) return;
            onRemoved(boundComponent);
            _component = default;
        }
    }
}
