using System;
using System.ComponentModel;

namespace AGS.API
{
    /// <summary>
    /// A component can be added to an entity (object/character/button/etc), and add additional behavior to it.
    /// </summary>
	public interface IComponent : IDisposable, INotifyPropertyChanged
	{
        /// <summary>
        /// The name of the component.
        /// </summary>
		string Name { get; }

        /// <summary>
        /// The entity which holds this component.
        /// </summary>
        IEntity Entity { get; }

        /// <summary>
        /// The type under which the component was registered with the entity.
        /// </summary>
        Type RegistrationType { get; }

        /// <summary>
        /// Initializes the component. If the component is dependant on other components, they should be retrieved here
        /// using entity.GetComponent.
        /// </summary>
        /// <param name="entity">The entity which holds this component.</param>
        /// <param name="registrationType">The type under which the component was registered with the entity.</param>
		void Init(IEntity entity, Type registrationType);

        /// <summary>
        /// This called after all the components have been initialized. If the component requires
        /// data from another component AFTER it has been initialized, this is the place to do it.
        /// </summary>
        void AfterInit();
	}
}