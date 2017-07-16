namespace AGS.API
{
    /// <summary>
    /// Represents binding of actions to be performed when a component is added/removed.
    /// </summary>
    public interface IComponentBinding
    {
        /// <summary>
        /// Unbind the trigger actions.
        /// </summary>
        void Unbind();
    }
}
