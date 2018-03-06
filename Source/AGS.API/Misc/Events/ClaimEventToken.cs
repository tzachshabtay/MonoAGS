namespace AGS.API
{
    /// <summary>
    /// This is a token that can be used when you want to claim an event, so that other subscribers which follow you on the subscriber list will not get that event.
    /// <seealso cref="ClaimableCallback"/>
    /// </summary>
    public struct ClaimEventToken
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:AGS.API.ClaimEventToken"/> is claimed.
        /// If you claim the event (by setting Claimed = true), this event will not be propogated to the rest of the subscribers.
        /// </summary>
        /// <value><c>true</c> if claimed; otherwise, <c>false</c>.</value>
        public bool Claimed { get; set; }
    }
}
