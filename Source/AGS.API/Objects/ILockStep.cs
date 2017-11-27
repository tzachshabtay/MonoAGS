namespace AGS.API
{
    /// <summary>
    /// A mechanism to lock changes for various components, to ensure multiple changes happen at once.
    /// </summary>
    public interface ILockStep
    {
        /// <summary>
        /// Locks the component from making changes.
        /// </summary>
        void Lock();

        /// <summary>
        /// Prepares the component for unlocking (changes are calculated based on everything that happened since the lock),
        /// but no events are not fired yet.
        /// </summary>
        void PrepareForUnlock();

        /// <summary>
        /// Unlock this component and fires any change events if there were changes.
        /// </summary>
        void Unlock();
    }
}
