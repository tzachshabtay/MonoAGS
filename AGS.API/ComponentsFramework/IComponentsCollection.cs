using System;
using System.Collections.Generic;

namespace AGS.API
{
	public interface IComponentsCollection : IEnumerable<IComponent>, IDisposable
	{
		TComponent AddComponent<TComponent>() where TComponent : IComponent;
		IComponent AddComponent(Type componentType);
		bool AddComponent<TComponent> (TComponent component) where TComponent : IComponent;

		bool RemoveComponents<TComponent>() where TComponent : IComponent;
		bool RemoveComponents(Type componentType);
		bool RemoveComponent(IComponent component);

		bool HasComponent<TComponent>() where TComponent : IComponent;
		bool HasComponent(Type componentType);
		bool HasComponent(IComponent component);

		TComponent GetComponent<TComponent>() where TComponent : IComponent;
		IComponent GetComponent(Type componentType);

		IEnumerable<TComponent> GetComponents<TComponent>() where TComponent : IComponent;
		IEnumerable<IComponent> GetComponents(Type componentType);

		int Count { get; }
		int CountType(Type componentType);
		int CountType<TComponent>() where TComponent : IComponent;

        IEvent<AGSEventArgs> OnComponentsInitialized { get; }
	}
}

