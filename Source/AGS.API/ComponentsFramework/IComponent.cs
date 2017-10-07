using System;

namespace AGS.API
{
    /// <summary>
    /// A component can be added to an entity (object/character/button/etc), and add additional behavior to it.
    /// </summary>
	public interface IComponent : IDisposable
	{
        /// <summary>
        /// The name of the component.
        /// </summary>
		string Name { get; }

        /// <summary>
        /// Initializes the component. If the component is dependant on other components, they should be retrieved here
        /// using entity.GetComponent.
        /// </summary>
        /// <param name="entity"></param>
		void Init(IEntity entity);

        /// <summary>
        /// This called after all the components have been initialized. If the component requires
        /// data from another component AFTER it has been initialized, this is the place to do it.
        /// </summary>
        void AfterInit();
	}
}

