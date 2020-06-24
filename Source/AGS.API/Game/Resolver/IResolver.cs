namespace AGS.API
{
    /// <summary>
    /// The resolver is an object that allows you to retrieve various systems from the engine.
    /// It uses an inversion of control container (https://en.wikipedia.org/wiki/Inversion_of_control) in order
    /// to allow you to replace the built-in engine system implementations with your own implementations.
    /// For more details, see: https://tzachshabtay.github.io/MonoAGS/articles/customizations.html
    /// </summary>
    public interface IResolver
    {
        /// <summary>
        /// Resolves a service.
        /// </summary>
        /// <returns>The requested service.</returns>
        /// <typeparam name="TService">The type of the service you request.</typeparam>
        TService Resolve<TService>();
    }
}