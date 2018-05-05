using System;
using AGS.API;

namespace AGS.Engine
{
    public class AGSComponentBinding<TComponent> : IComponentBinding where TComponent : IComponent
    {
        private readonly Action<TComponent> onAdded, onRemoved;
        private readonly IComponentsCollection _collection;

        public AGSComponentBinding(IComponentsCollection collection, Action<TComponent> onAdded, 
                                   Action<TComponent> onRemoved)
        {
            _collection = collection;
            this.onAdded = onAdded;
            this.onRemoved = onRemoved;
            collection.OnComponentsChanged.Subscribe(onComponentsChanged);
			TComponent component = collection.GetComponent<TComponent>();
            if (component != null) onAdded.Invoke(component);
            else if (!collection.ComponentsInitialized) collection.OnComponentsInitialized.Subscribe(onComponentsInitialized);
        }

        private void onComponentsChanged(AGSListChangedEventArgs<IComponent> args)
        {
            if (args.ChangeType == ListChangeType.Add && onAdded != null)
			{
				foreach (var component in args.Items)
				{
                    if (component.Item is TComponent boundComponent) onAdded(boundComponent);
				}
			}
            else if (args.ChangeType == ListChangeType.Remove && onRemoved != null)
			{
				foreach (var component in args.Items)
				{
                    if (component.Item is TComponent boundComponent)
                    {
                        onRemoved(boundComponent);
                        if (!_collection.ComponentsInitialized)
                        {
                            _collection.OnComponentsInitialized.Subscribe(onComponentsInitialized);
                        }
                    }
				}
			}
        }

        private void onComponentsInitialized()
        {
            if (onAdded == null) return;
            foreach (var component in _collection)
            {
                if (component is TComponent boundComponent) onAdded(boundComponent);
            }
        }

        public void Unbind()
        {
            _collection.OnComponentsChanged.Unsubscribe(onComponentsChanged);
            _collection.OnComponentsInitialized.Unsubscribe(onComponentsInitialized);
            if (onRemoved == null) return;
            foreach (var component in _collection)
            {
                if (component is TComponent boundComponent) onRemoved(boundComponent);
            }
        }
    }
}