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
        private ConcurrentDictionary<Type, Lazy<IComponent>> _components;
        private AGSConcurrentHashSet<IComponentBinding> _bindings;
        private Resolver _resolver;
        private bool _componentsInitialized;

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
            _components = new ConcurrentDictionary<Type, Lazy<IComponent>>();
            _bindings = new AGSConcurrentHashSet<IComponentBinding>();
            OnComponentsInitialized = new AGSEvent();
            OnComponentsChanged = new AGSEvent<AGSListChangedEventArgs<IComponent>>();
        }

        ~AGSEntity()
        {
            dispose(false);
        }

        public string ID { get; private set; }

        public IBlockingEvent OnComponentsInitialized { get; private set; }

        public IBlockingEvent<AGSListChangedEventArgs<IComponent>> OnComponentsChanged { get; private set; }

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
            if (components == null) return default;
            return components.GetOrAdd(componentType, _ => new Lazy<IComponent>(() =>
            {
                IComponent component = (IComponent)_resolver.Container.Resolve(componentType);
                initComponentIfNeeded(component);
                return component;
            })).Value;
        }

        public bool AddComponent<TComponent>(IComponent component) where TComponent : IComponent
        {
			var components = _components;
            if (components == null) return false;
            bool addedComponent = false;
            var _ = components.GetOrAdd(typeof(TComponent), __ => new Lazy<IComponent>(() =>
            {
                addedComponent = true;
                initComponentIfNeeded(component);
                return component;
            })).Value;
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
            Lazy<IComponent> component;
            if (!components.TryRemove(componentType, out component)) return false;

            component.Value.Dispose();
			OnComponentsChanged.Invoke(new AGSListChangedEventArgs<IComponent>(ListChangeType.Remove,
																  new AGSListItem<IComponent>(component.Value, count--)));
            
            return true;
        }

        public bool RemoveComponent(IComponent component)
        {
			var components = _components;
			if (components == null) return false;
            if (!components.TryGetValue(component.GetType(), out var existing) || existing.Value != component) return false;
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
            return components.TryGetValue(component.GetType(), out var existing) && existing.Value == component;
        }

        public TComponent GetComponent<TComponent>() where TComponent : IComponent
        {
            return (TComponent)GetComponent(typeof(TComponent));
        }

        public IComponent GetComponent(Type componentType)
        {
			var components = _components;
            if (components == null) return default;
            if (!components.TryGetValue(componentType, out var component)) return null;
            return component.Value;
        }

        public IComponentBinding Bind<TComponent>(Action<TComponent> onAdded, Action<TComponent> onRemoved) where TComponent : IComponent
        {
            var bindings = _bindings;
            if (bindings == null) return null; //Entity was already disposed -> not binding
            AGSComponentBinding<TComponent> binding = new AGSComponentBinding<TComponent>(this, onAdded, onRemoved, _componentsInitialized);
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

        public IEnumerator<IComponent> GetEnumerator()
        {
			var components = _components;
			if (components == null) return new List<IComponent>().GetEnumerator();
            return components.Values.Select(c => c.Value).GetEnumerator();
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

