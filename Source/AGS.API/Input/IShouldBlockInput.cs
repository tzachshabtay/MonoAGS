namespace AGS.API
{
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
