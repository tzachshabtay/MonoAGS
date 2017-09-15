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
        private ConcurrentDictionary<Type, AGSConcurrentHashSet<IComponent>> _components;
        private List<IComponentBinding> _bindings;
        private Resolver _resolver;
        private bool _componentsInitialized;

        public AGSEntity(string id, Resolver resolver)
        {
            ID = id;
            _resolver = resolver;
            _components = new ConcurrentDictionary<Type, AGSConcurrentHashSet<IComponent>>();
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
            AGSConcurrentHashSet<IComponent> ofType = components.GetOrAdd(componentType, _ => new AGSConcurrentHashSet<IComponent>());
            IComponent existingComponent = ofType.FirstOrDefault();
            if (existingComponent == null || existingComponent.AllowMultiple)
            {
                IComponent component = (IComponent)_resolver.Container.Resolve(componentType);
                ofType.Add(component);
                initComponentIfNeeded(component);
                return component;
            }
            return existingComponent;
        }

        public bool AddComponent(IComponent component)
        {
			var components = _components;
            if (components == null) return false;
            AGSConcurrentHashSet<IComponent> ofType = components.GetOrAdd(component.GetType(), _ => new AGSConcurrentHashSet<IComponent>());
            IComponent existingComponent = ofType.FirstOrDefault();
            if (existingComponent == null || existingComponent.AllowMultiple)
            {
                ofType.Add(component);
                initComponentIfNeeded(component);
                return true;
            }
            return false;
        }

        public bool RemoveComponents<TComponent>() where TComponent : IComponent
        {
            return RemoveComponents(typeof(TComponent));
        }

        public bool RemoveComponents(Type componentType)
        {
			var components = _components;
            if (components == null) return false;
            AGSConcurrentHashSet<IComponent> ofType;
            int count = Count;
            if (!components.TryRemove(componentType, out ofType) || ofType.Count == 0) return false;
            foreach (var component in ofType)
            {
                component.Dispose();
				OnComponentsChanged.Invoke(new AGSListChangedEventArgs<IComponent>(ListChangeType.Remove,
																	  new AGSListItem<IComponent>(component, count--)));
            }
            return true;
        }

        public bool RemoveComponent(IComponent component)
        {
			var components = _components;
			if (components == null) return false;
            AGSConcurrentHashSet<IComponent> ofType;
            if (!components.TryGetValue(component.GetType(), out ofType)) return false;
            component.Dispose();
            if (ofType.Remove(component))
            {
                OnComponentsChanged.Invoke(new AGSListChangedEventArgs<IComponent>(ListChangeType.Remove, 
                                                                      new AGSListItem<IComponent>(component, Count)));
                return true;
            }
            return false;
        }

        public bool HasComponent<TComponent>() where TComponent : IComponent
        {
            return HasComponent(typeof(TComponent));
        }

        public bool HasComponent(Type componentType)
        {
			var components = _components;
			if (components == null) return false;
            AGSConcurrentHashSet<IComponent> ofType;
            return (components.TryGetValue(componentType, out ofType) && ofType.Count > 0);
        }

        public bool HasComponent(IComponent component)
        {
			var components = _components;
			if (components == null) return false;
            AGSConcurrentHashSet<IComponent> ofType;
            return (components.TryGetValue(component.GetType(), out ofType) && ofType.Contains(component));
        }

        public TComponent GetComponent<TComponent>() where TComponent : IComponent
        {
            return (TComponent)GetComponent(typeof(TComponent));
        }

        public IComponent GetComponent(Type componentType)
        {
			var components = _components;
            if (components == null) return default(IComponent);
            AGSConcurrentHashSet<IComponent> ofType;
            if (!components.TryGetValue(componentType, out ofType)) return null;
            return ofType.FirstOrDefault();
        }

        public IEnumerable<TComponent> GetComponents<TComponent>() where TComponent : IComponent
        {
            return GetComponents(typeof(TComponent)).Cast<TComponent>();
        }

        public IEnumerable<IComponent> GetComponents(Type componentType)
        {
			var components = _components;
            if (components == null) return new List<IComponent>();
            AGSConcurrentHashSet<IComponent> ofType;
            if (!components.TryGetValue(componentType, out ofType)) return new List<IComponent>();
            return ofType;
        }

        public int CountType(Type componentType)
        {
			var components = _components;
            if (components == null) return 0;
            AGSConcurrentHashSet<IComponent> ofType;
            if (!components.TryGetValue(componentType, out ofType)) return 0;
            return ofType.Count;
        }

        public int CountType<TComponent>() where TComponent : IComponent
        {
            return CountType(typeof(TComponent));
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
                return components.Sum(c => c.Value.Count);
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
            return components.SelectMany(c => c.Value).GetEnumerator();
        }

        #endregion

        #region IEnumerable implementation

        IEnumerator IEnumerable.GetEnumerator()
        {
			var components = _components;
            if (components == null) return new List<IComponent>().GetEnumerator();
            return ((IEnumerable)components.SelectMany(c => c.Value)).GetEnumerator();
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
            foreach (var component in components.SelectMany(c => c.Value))
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

