using System;
using AGS.API;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Concurrent;
using Autofac;
using System.Linq;
using System.ComponentModel;
using PropertyChanged;

namespace AGS.Engine
{
    [DoNotNotify]
    public abstract class AGSEntity : IEntity
    {
        //storing the components as lazy values to avoid having the component binding kick in more than once, see here: https://andrewlock.net/making-getoradd-on-concurrentdictionary-thread-safe-using-lazy/ 
        private ConcurrentDictionary<Type, Lazy<API.IComponent>> _components;
        private AGSConcurrentHashSet<API.IComponentBinding> _bindings;
        private Resolver _resolver;

        //This a design limitation, as all of the preset entities (object, character, etc) implement the components as a convinience they also need to implement the PropertyChanged event, though there really
        //is no need to provide it on the entity level (if there is then we'll need to add support).
        public event PropertyChangedEventHandler PropertyChanged
        {
            add { throw new NotSupportedException(); }
            remove { throw new NotSupportedException(); }
        }

        public AGSEntity(string id, Resolver resolver)
        {
            ID = id;
            _resolver = resolver;
            _components = new ConcurrentDictionary<Type, Lazy<API.IComponent>>();
            _bindings = new AGSConcurrentHashSet<API.IComponentBinding>();
            OnComponentsInitialized = new AGSEvent();
            OnComponentsChanged = new AGSEvent<AGSListChangedEventArgs<API.IComponent>>();
        }

        ~AGSEntity()
        {
            dispose(false);
        }

        public string ID { get; private set; }

        public string DisplayName { get; set; }

        public string GetFriendlyName() { return DisplayName ?? ID; }

        public bool ComponentsInitialized { get; private set; }

        public IBlockingEvent OnComponentsInitialized { get; private set; }

        public IBlockingEvent<AGSListChangedEventArgs<API.IComponent>> OnComponentsChanged { get; private set; }

        protected void InitComponents()
        {
            foreach (var component in this) component.Init(this);
            foreach (var component in this) component.AfterInit();
            ComponentsInitialized = true;
            OnComponentsInitialized.Invoke();
        }

        #region API.IComponentsCollection implementation

        public TComponent AddComponent<TComponent>() where TComponent : API.IComponent
        {
            return (TComponent)AddComponent(typeof(TComponent));
        }

        public API.IComponent AddComponent(Type componentType)
        {
            var components = _components;
            if (components == null) return default;
            return components.GetOrAdd(componentType, _ => new Lazy<API.IComponent>(() =>
            {
                API.IComponent component = (API.IComponent)_resolver.Container.Resolve(componentType);
                initComponentIfNeeded(component);
                return component;
            })).Value;
        }

        public bool AddComponent<TComponent>(API.IComponent component) where TComponent : API.IComponent
        {
			var components = _components;
            if (components == null) return false;
            bool addedComponent = false;
            var _ = components.GetOrAdd(typeof(TComponent), __ => new Lazy<API.IComponent>(() =>
            {
                addedComponent = true;
                initComponentIfNeeded(component);
                return component;
            })).Value;
            return addedComponent;
        }

        public bool RemoveComponent<TComponent>() where TComponent : API.IComponent
        {
            return RemoveComponent(typeof(TComponent));
        }

        public bool RemoveComponent(Type componentType)
        {
			var components = _components;
            if (components == null) return false;
            int count = Count;
            Lazy<API.IComponent> component;
            if (!components.TryRemove(componentType, out component)) return false;

            component.Value.Dispose();
			OnComponentsChanged.Invoke(new AGSListChangedEventArgs<API.IComponent>(ListChangeType.Remove,
																  new AGSListItem<API.IComponent>(component.Value, count--)));
            
            return true;
        }

        public bool RemoveComponent(API.IComponent component)
        {
			var components = _components;
			if (components == null) return false;
            if (!components.TryGetValue(component.GetType(), out var existing) || existing.Value != component) return false;
            return RemoveComponent(component.GetType());
        }

        public bool HasComponent<TComponent>() where TComponent : API.IComponent
        {
            return HasComponent(typeof(TComponent));
        }

        public bool HasComponent(Type componentType)
        {
			var components = _components;
			if (components == null) return false;
            return components.ContainsKey(componentType);
        }

        public bool HasComponent(API.IComponent component)
        {
			var components = _components;
			if (components == null) return false;
            return components.TryGetValue(component.GetType(), out var existing) && existing.Value == component;
        }

        public TComponent GetComponent<TComponent>() where TComponent : API.IComponent
        {
            return (TComponent)GetComponent(typeof(TComponent));
        }

        public API.IComponent GetComponent(Type componentType)
        {
			var components = _components;
            if (components == null) return default;
            if (!components.TryGetValue(componentType, out var component)) return null;
            return component.Value;
        }

        public API.IComponentBinding Bind<TComponent>(Action<TComponent> onAdded, Action<TComponent> onRemoved) where TComponent : API.IComponent
        {
            var bindings = _bindings;
            if (bindings == null) return null; //Entity was already disposed -> not binding
            AGSComponentBinding<TComponent> binding = new AGSComponentBinding<TComponent>(this, onAdded, onRemoved);
            bindings.Add(binding);
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

        public IEnumerator<API.IComponent> GetEnumerator()
        {
			var components = _components;
			if (components == null) return new List<API.IComponent>().GetEnumerator();
            return components.Values.Select(c => c.Value).GetEnumerator();
        }

        #endregion

        #region IEnumerable implementation

        IEnumerator IEnumerable.GetEnumerator()
        {
			var components = _components;
            if (components == null) return new List<API.IComponent>().GetEnumerator();
            return ((IEnumerable)components.Values).GetEnumerator();
        }

        #endregion

        #region Component Empty Implementation

        public void AfterInit() { }

        #endregion

        private void initComponentIfNeeded(API.IComponent component)
        {
            if (!ComponentsInitialized) return;
            component.Init(this);
            component.AfterInit();
            OnComponentsChanged.Invoke(new AGSListChangedEventArgs<API.IComponent>(ListChangeType.Add,
                                                          new AGSListItem<API.IComponent>(component, Count)));
        }

        private void dispose(bool disposing)
        {
            var components = _components;
            if (components == null) return;
            foreach (var component in components.Values)
            {
                component.Value.Dispose();
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

