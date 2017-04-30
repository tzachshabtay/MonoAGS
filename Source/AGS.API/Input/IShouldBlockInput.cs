namespace AGS.API
{
    /// <summary>
    /// This interface is used when processing input to determine whether input should be allowed and processed.
    /// For example, when the room is in transition, input is not allowed.
    /// You can change the behavior of when input is allowed or not by overriding this interface and hooking it
    /// up with the Resolver.
    /// </summary>
    public interface IShouldBlockInput
    {
        /// <summary>
        /// Whenever there's an input event (user presses a key, clicks a mouse button, touches the screen, etc) 
        /// this method will be queried to see if input is allowed to be processed currently.
        /// </summary>
        /// <returns><c>true</c>, if block input was shoulded, <c>false</c> otherwise.</returns>
        bool ShouldBlockInput();
    }
}
