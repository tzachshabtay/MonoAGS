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
		private ConcurrentDictionary<Type, List<IComponent>> _components;
		private Resolver _resolver;

		public AGSEntity(string id, Resolver resolver)
		{
			ID = id;
			_resolver = resolver;
			_components = new ConcurrentDictionary<Type, List<IComponent>> ();
            OnComponentsInitialized = new AGSEvent<AGSEventArgs>();
		}

		public string ID { get; private set; }

        public IEvent<AGSEventArgs> OnComponentsInitialized { get; private set; }

		protected void InitComponents()
		{
			foreach (var component in this) component.Init(this);
            OnComponentsInitialized.Invoke(this, new AGSEventArgs());
		}

		#region IComponentsCollection implementation

		public TComponent AddComponent<TComponent>() where TComponent : IComponent
		{
			return (TComponent) AddComponent(typeof(TComponent));
		}

		public IComponent AddComponent(Type componentType)
		{
			List<IComponent> ofType = _components.GetOrAdd(componentType, _ => new List<IComponent> ());
			if (ofType.Count == 0 || ofType[0].AllowMultiple)
			{
				IComponent component = (IComponent)_resolver.Container.Resolve(componentType);
				ofType.Add(component);
				return component;
			}
			return null;
		}

		public bool AddComponent<TComponent>(TComponent component) where TComponent : IComponent
		{
			List<IComponent> ofType = _components.GetOrAdd(typeof(TComponent), _ => new List<IComponent> ());
			if (ofType.Count == 0 || component.AllowMultiple)
			{
				ofType.Add(component);
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
			List<IComponent> ofType;
			if (!_components.TryRemove(componentType, out ofType) || ofType.Count == 0) return false;
			foreach (var component in ofType)
			{
				component.Dispose();
			}
			return true;
		}

		public bool RemoveComponent(IComponent component)
		{
			List<IComponent> ofType;
			if (!_components.TryGetValue(component.GetType(), out ofType)) return false;
			component.Dispose();
			return ofType.Remove(component);
		}

		public bool HasComponent<TComponent>() where TComponent : IComponent
		{
			return HasComponent(typeof(TComponent));
		}

		public bool HasComponent(Type componentType)
		{
			List<IComponent> ofType;
			return (_components.TryGetValue(componentType, out ofType) && ofType.Count > 0);
		}

		public bool HasComponent(IComponent component)
		{
			List<IComponent> ofType;
			return (_components.TryGetValue(component.GetType(), out ofType) && ofType.Contains(component));
		}

		public TComponent GetComponent<TComponent>() where TComponent : IComponent
		{
			return (TComponent)GetComponent(typeof(TComponent));
		}

		public IComponent GetComponent(Type componentType)
		{
			List<IComponent> ofType;
			if (!_components.TryGetValue(componentType, out ofType)) return null;
			return ofType.FirstOrDefault();
		}

		public IEnumerable<TComponent> GetComponents<TComponent>() where TComponent : IComponent
		{
			return GetComponents(typeof(TComponent)).Cast<TComponent>();
		}

		public IEnumerable<IComponent> GetComponents(Type componentType)
		{
			List<IComponent> ofType;
			if (!_components.TryGetValue(componentType, out ofType)) return new List<IComponent>();
			return ofType;
		}

		public int CountType(Type componentType)
		{
			List<IComponent> ofType;
			if (!_components.TryGetValue(componentType, out ofType)) return 0;
			return ofType.Count;
		}

		public int CountType<TComponent>() where TComponent : IComponent
		{
			return CountType(typeof(TComponent));
		}

		public int Count
		{
			get
			{
				return _components.Sum(c => c.Value.Count);
			}
		}

		public void Dispose()
		{
			foreach (var component in this)
			{
				component.Dispose();
			}
			_components.Clear();
		}

		#endregion

		#region IEnumerable implementation

		public IEnumerator<IComponent> GetEnumerator()
		{
			return _components.SelectMany(c => c.Value).GetEnumerator();
		}

		#endregion

		#region IEnumerable implementation

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)_components.SelectMany(c => c.Value)).GetEnumerator();
		}

		#endregion
	}
}

