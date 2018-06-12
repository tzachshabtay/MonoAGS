using System;
using AGS.API;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Concurrent;
using Autofac;
using System.Linq;
using System.ComponentModel;
using PropertyChanged;
using System.Diagnostics;

namespace AGS.Engine
{
    [DoNotNotify]
    public abstract class AGSEntity : IEntity
    {
        //storing the components as lazy values to avoid having the component binding kick in more than once, see here: https://andrewlock.net/making-getoradd-on-concurrentdictionary-thread-safe-using-lazy/ 
        private ConcurrentDictionary<Type, Lazy<API.IComponent>> _components;
        private AGSConcurrentHashSet<API.IComponentBinding> _bindings;
        private Resolver _resolver;
        private string _displayName;
        private IBlockingEvent _onDisposed;

        public event PropertyChangedEventHandler PropertyChanged;

        private static AGSConcurrentHashSet<string> _ids = new AGSConcurrentHashSet<string>(1000, false);

        public AGSEntity(string id, Resolver resolver)
        {
            ID = id;
            if (!_ids.Add(id))
            {
                throw new ArgumentException($"Duplicate entity: {id}");
            }
            _resolver = resolver;
            _components = new ConcurrentDictionary<Type, Lazy<API.IComponent>>();
            _bindings = new AGSConcurrentHashSet<API.IComponentBinding>(200, false);
            OnComponentsInitialized = new AGSEvent();
            OnComponentsChanged = new AGSEvent<AGSListChangedEventArgs<API.IComponent>>();
            _onDisposed = new AGSEvent();
        }

        public static void ClearIDs() => _ids.Clear();

        ~AGSEntity()
        {
            dispose(false);
        }

        public string ID { get; private set; }

        public string DisplayName 
        { 
            get { return _displayName; }
            set 
            {
                if (_displayName == value) return;
                _displayName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DisplayName)));
            }
        }

        public string GetFriendlyName() => DisplayName ?? ID;

        public override string ToString() => GetFriendlyName();

		public bool ComponentsInitialized { get; private set; }

        public IBlockingEvent OnComponentsInitialized { get; private set; }

        public IBlockingEvent<AGSListChangedEventArgs<API.IComponent>> OnComponentsChanged { get; private set; }

        protected void InitComponents()
        {
            var components = _components;
            if (components == null) return;
            foreach (var componentPair in components) componentPair.Value.Value.Init(this, componentPair.Key);
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
                initComponentIfNeeded(component, componentType);
                return component;
            })).Value;
        }

        public bool AddComponent<TComponent>(API.IComponent component) where TComponent : API.IComponent
        {
			var components = _components;
            if (components == null) return false;
            bool addedComponent = false;
            Type componentType = typeof(TComponent);
            var _ = components.GetOrAdd(componentType, __ => new Lazy<API.IComponent>(() =>
            {
                addedComponent = true;
                initComponentIfNeeded(component, componentType);
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
            if (!components.TryRemove(componentType, out var component)) return false;

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

        public TComponent PopComponent<TComponent>() where TComponent : API.IComponent
        {
            var components = _components;
            if (components == null)
                return default;
            int count = Count;
            if (!components.TryRemove(typeof(TComponent), out var component))
                return default;

            OnComponentsChanged.Invoke(new AGSListChangedEventArgs<API.IComponent>(ListChangeType.Remove,
                                                                  new AGSListItem<API.IComponent>(component.Value, count--)));

            return (TComponent)component.Value;
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
            if (components == null)
                yield break;
            foreach (var component in components.Values)
            {
                yield return component.Value;
            }
        }

        public void OnDisposed(Action action)
        {
            _onDisposed?.Subscribe(action);
            if (_onDisposed == null) action?.Invoke();
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

        private void initComponentIfNeeded(API.IComponent component, Type componentType)
        {
            if (!ComponentsInitialized) return;
            component.Init(this, componentType);
            component.AfterInit();
            OnComponentsChanged.Invoke(new AGSListChangedEventArgs<API.IComponent>(ListChangeType.Add,
                                                          new AGSListItem<API.IComponent>(component, Count)));
        }

        private void dispose(bool disposing)
        {
            _ids.Remove(ID);
            var bindings = _bindings;
            if (bindings != null)
            {
                foreach (var binding in bindings)
                {
                    binding.Unbind();
                }
            }

            _onDisposed?.Invoke();
            _onDisposed?.Dispose();
            _onDisposed = null;

            var components = _components;
            if (components != null)
            {
                foreach (var component in components.Values)
                {
                    component.Value.Dispose();
                }
            }

            _components = null;
            _bindings = null;
            PropertyChanged = null;
        }
    }
}
