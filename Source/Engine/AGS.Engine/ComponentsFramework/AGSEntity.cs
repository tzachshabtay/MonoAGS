using System;
using AGS.API;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Concurrent;
using Autofac;
using System.Linq;

namespace AGS.Engine
{
    public class AGSEntity : IEntity
    {
        private ConcurrentDictionary<Type, IComponent> _components;
        private List<IComponentBinding> _bindings;
        private Resolver _resolver;
        private bool _componentsInitialized;

        public AGSEntity(string id, Resolver resolver)
        {
            ID = id;
            _resolver = resolver;
            _components = new ConcurrentDictionary<Type, IComponent>();
            _bindings = new List<IComponentBinding>();
            OnComponentsInitialized = new AGSEvent();
            OnComponentsChanged = new AGSEvent<AGSListChangedEventArgs<IComponent>>();
        }

        ~AGSEntity()
        {
            dispose(false);
        }

        public string ID { get; private set; }

        public IEvent OnComponentsInitialized { get; private set; }

        public IEvent<AGSListChangedEventArgs<IComponent>> OnComponentsChanged { get; private set; }

        protected void InitComponents()
        {
            foreach (var component in this) component.Init(this);
            foreach (var component in this) component.AfterInit();
            _componentsInitialized = true;
            OnComponentsInitialized.Invoke();
        }

        #region IComponentsCollection implementation

        public TComponent AddComponent<TComponent>() where TComponent : IComponent
        {
            return (TComponent)AddComponent(typeof(TComponent));
        }

        public IComponent AddComponent(Type componentType)
        {
            var components = _components;
            if (components == null) return default(IComponent);
            return components.GetOrAdd(componentType, _ =>
            {
                IComponent component = (IComponent)_resolver.Container.Resolve(componentType);
                initComponentIfNeeded(component);
                return component;
            });
        }

        public bool AddComponent(IComponent component)
        {
			var components = _components;
            if (components == null) return false;
            bool addedComponent = false;
            components.GetOrAdd(component.GetType(), _ =>
            {
                addedComponent = true;
                initComponentIfNeeded(component);
                return component;
            });
            return addedComponent;
        }

        public bool RemoveComponent<TComponent>() where TComponent : IComponent
        {
            return RemoveComponent(typeof(TComponent));
        }

        public bool RemoveComponent(Type componentType)
        {
			var components = _components;
            if (components == null) return false;
            int count = Count;
            IComponent component;
            if (!components.TryRemove(componentType, out component)) return false;

            component.Dispose();
			OnComponentsChanged.Invoke(new AGSListChangedEventArgs<IComponent>(ListChangeType.Remove,
																  new AGSListItem<IComponent>(component, count--)));
            
            return true;
        }

        public bool RemoveComponent(IComponent component)
        {
			var components = _components;
			if (components == null) return false;
            IComponent existing;
            if (!components.TryGetValue(component.GetType(), out existing) || existing != component) return false;
            return RemoveComponent(component.GetType());
        }

        public bool HasComponent<TComponent>() where TComponent : IComponent
        {
            return HasComponent(typeof(TComponent));
        }

        public bool HasComponent(Type componentType)
        {
			var components = _components;
			if (components == null) return false;
            return components.ContainsKey(componentType);
        }

        public bool HasComponent(IComponent component)
        {
			var components = _components;
			if (components == null) return false;
            IComponent existing;
            return components.TryGetValue(component.GetType(), out existing) && existing == component;
        }

        public TComponent GetComponent<TComponent>() where TComponent : IComponent
        {
            return (TComponent)GetComponent(typeof(TComponent));
        }

        public IComponent GetComponent(Type componentType)
        {
			var components = _components;
            if (components == null) return default(IComponent);
            IComponent component;
            if (!components.TryGetValue(componentType, out component)) return null;
            return component;
        }

        public IComponentBinding Bind<TComponent>(Action<TComponent> onAdded, Action<TComponent> onRemoved) where TComponent : IComponent
        {
            AGSComponentBinding<TComponent> binding = new AGSComponentBinding<TComponent>(this, onAdded, onRemoved, _componentsInitialized);
            _bindings.Add(binding);
            return binding;
        }

        public int Count
        {
            get
            {
				var components = _components;
                if (components == null) return 0;
                return components.Count;
            }
        }

        public void Dispose()
        {
            dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region IEnumerable implementation

        public IEnumerator<IComponent> GetEnumerator()
        {
			var components = _components;
			if (components == null) return new List<IComponent>().GetEnumerator();
            return components.Values.GetEnumerator();
        }

        #endregion

        #region IEnumerable implementation

        IEnumerator IEnumerable.GetEnumerator()
        {
			var components = _components;
            if (components == null) return new List<IComponent>().GetEnumerator();
            return ((IEnumerable)components.Values).GetEnumerator();
        }

        #endregion

        #region Component Empty Implementation

        public void AfterInit() { }

        #endregion

        private void initComponentIfNeeded(IComponent component)
        {
            if (!_componentsInitialized) return;
            component.Init(this);
            component.AfterInit();
            OnComponentsChanged.Invoke(new AGSListChangedEventArgs<IComponent>(ListChangeType.Add,
                                                          new AGSListItem<IComponent>(component, Count)));
        }

        private void dispose(bool disposing)
        {
            var components = _components;
            if (components == null) return;
            foreach (var component in components.Values)
            {
                component.Dispose();
            }
            _components = null;

            var bindings = _bindings;
            if (bindings == null) return;
            foreach (var binding in bindings)
            {
                binding.Unbind();
            }
            _bindings = null;
        }
    }
}

