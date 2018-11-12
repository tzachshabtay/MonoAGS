using System;
using System.Collections.Concurrent;
using System.Linq;
using AGS.API;

namespace AGS.Engine
{
    public class EntitySubscription<TComponent> : IEntitySubscription where TComponent : IComponent
    {
        private readonly string[] _propertyNames;
        private readonly Action _onPropertyChanged;
        private readonly Action<TComponent> _onAdd, _onRemove;
        private readonly ConcurrentDictionary<string, IComponentBinding> _bindings;

        public EntitySubscription(Action onPropertyChanged, Action<TComponent> onAdd = null, 
                                  Action<TComponent> onRemove = null, params string[] propertyNames)
        {
            _onPropertyChanged = onPropertyChanged;
            _onAdd = onAdd;
            _onRemove = onRemove;
            _propertyNames = propertyNames;
            _bindings = new ConcurrentDictionary<string, IComponentBinding>();
        }

        public void Subscribe(IEntity entity)
        {
            var binding = entity.Bind<TComponent>(onComponentAdded, onComponentRemoved);
            _bindings[entity.ID] = binding;
        }

        public void Unsubscribe(IEntity entity)
        {
            _bindings.TryRemove(entity.ID, out var binding);
            binding?.Unbind();
            onComponentRemoved(entity.GetComponent<TComponent>());
        }

        private void onComponentAdded(TComponent component)
        {
            _onAdd?.Invoke(component);
            if (_propertyNames.Length > 0)
            {
                component.PropertyChanged += onPropertyChanged;
            }
        }

        private void onComponentRemoved(TComponent component)
        {
#pragma warning disable RECS0017 // Possible compare of value type with 'null'
            if (component == null) return;
#pragma warning restore RECS0017 // Possible compare of value type with 'null'
            _onRemove?.Invoke(component);
            if (_propertyNames.Length > 0)
            {
                component.PropertyChanged -= onPropertyChanged;
            }
        }

        private void onPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs args)
        {
            if (_propertyNames.Any(p => p == args.PropertyName)) _onPropertyChanged();
        }
    }
}
