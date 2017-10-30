using System;
using System.Collections.Generic;

namespace AGS.API
{
    /// <summary>
    /// A collection of componenets: allows you to add, remove, get and iterate components.
    /// </summary>
	public interface IComponentsCollection : IEnumerable<IComponent>, IDisposable
	{
        /// <summary>
        /// Adds a new component with the specified type.
        /// </summary>
        /// <returns>The component, if added successfully, null otherwise (if a component with this type already exists).</returns>
        /// <typeparam name="TComponent">The component type.</typeparam>
		TComponent AddComponent<TComponent>() where TComponent : IComponent;

        /// <summary>
        /// Adds a new component with the specified type.
        /// </summary>
        /// <returns>The component, if added successfully, null otherwise (if a component with this type already exists).</returns>
        /// <param name="componentType">The component type.</param>
		IComponent AddComponent(Type componentType);

        /// <summary>
        /// Adds the specified component.
        /// </summary>
        /// <returns><c>true</c>, if component was added, <c>false</c> otherwise (if a component with this type already exists).</returns>
        /// <param name="component">Component.</param>
        /// <typeparam name="TComponent">The component type (with which it will be registered and can be queried by other components).</typeparam>
        /// <example>
        /// The type parameter is used to specify under which type the component will be registed in the entity.
        /// Suppose you want to replace an existing component in the entity, for example the rotation component.
        /// By using `IRotateComponent` as your type parameter, other components will get your component back when querying for `IRotateComponent`. 
        /// Your component should implement `IRotateComponent`.
        /// <code>
        /// public class MyRotateComponent : AGSComponent, IRotateComponent
        /// {
        ///  ...
        /// }
        /// 
        /// var myRotate = new MyRotateComponent();
        /// var existingRotate = obj.GetComponent<IRotateComponent>();
        /// Console.WriteLine(myRotate == existingRotate); //Writes "false"
        /// 
        /// obj.RemoveComponent<IRotateComponent>();
        /// obj.AddComponent<IRotateComponent>(myRotate);
        /// 
        /// existingRotate = obj.GetComponent<IRotateComponent>();
        /// Console.WriteLine(myRotate == existingRotate); //Writes "true"
        /// </code>
        /// </example>
        bool AddComponent<TComponent>(IComponent component) where TComponent : IComponent;

        /// <summary>
        /// Removes the component with the specified type (if it exists).
        /// </summary>
        /// <returns><c>true</c>, if component was removed, <c>false</c> otherwise.</returns>
        /// <typeparam name="TComponent">The component type.</typeparam>
		bool RemoveComponent<TComponent>() where TComponent : IComponent;

		/// <summary>
		/// Removes the component with the specified type (if it exists).
		/// </summary>
		/// <returns><c>true</c>, if component was removed, <c>false</c> otherwise.</returns>
		/// <param name="componentType">The component type.</param>
		bool RemoveComponent(Type componentType);

        /// <summary>
        /// Removes the specified component.
        /// </summary>
        /// <returns><c>true</c>, if component was removed, <c>false</c> otherwise.</returns>
        /// <param name="component">Component.</param>
		bool RemoveComponent(IComponent component);

        /// <summary>
        /// Is there a component of the specified type in the collection?
        /// </summary>
        /// <returns><c>true</c>, if component exists, <c>false</c> otherwise.</returns>
        /// <typeparam name="TComponent">The component type.</typeparam>
		bool HasComponent<TComponent>() where TComponent : IComponent;

        /// <summary>
        /// Is there a component of the specified type in the collection?
        /// </summary>
        /// <returns><c>true</c>, if component exists, <c>false</c> otherwise.</returns>
        /// <typeparam name="TComponent">The component type.</typeparam>
		bool HasComponent(Type componentType);

        /// <summary>
        /// Is the specified component in the collection?
        /// </summary>
        /// <returns><c>true</c>, if component exists, <c>false</c> otherwise.</returns>
        /// <typeparam name="TComponent">The component.</typeparam>
		bool HasComponent(IComponent component);

        /// <summary>
        /// Gets the component of the specified type.
        /// </summary>
        /// <returns>The component if found, null if no components of this type in the collection.</returns>
        /// <typeparam name="TComponent">The component type.</typeparam>
		TComponent GetComponent<TComponent>() where TComponent : IComponent;

        /// <summary>
        /// Gets the component of the specified type.
        /// </summary>
        /// <returns>The component if found, null if no components of this type in the collection.</returns>
        /// <typeparam name="TComponent">The component type.</typeparam>
        IComponent GetComponent(Type componentType);

        /// <summary>
        /// Bind actions to when a component is added/removed.
        /// </summary>
        /// <returns>The binding.</returns>
        /// <param name="onAdded">Callback to be called when component is added.</param>
        /// <param name="onRemoved">Callback to be called when component is removed.</param>
        /// <typeparam name="TComponent">The 1st type parameter.</typeparam>
        IComponentBinding Bind<TComponent>(Action<TComponent> onAdded, Action<TComponent> onRemoved) where TComponent : IComponent;

        /// <summary>
        /// Gets the number of components.
        /// </summary>
        /// <value>The count.</value>
		int Count { get; }

        /// <summary>
        /// An event that fires after all components were initialized.
        /// </summary>
        /// <value>The event.</value>
        IEvent OnComponentsInitialized { get; }

        /// <summary>
        /// An event which is triggered whenever a component is added/removed.
        /// </summary>
        /// <value>The event.</value>
        IEvent<AGSListChangedEventArgs<IComponent>> OnComponentsChanged { get; }
	}
}

