namespace KachnaOnline.Dto.ClubStates
{
    /// <summary>
    /// A type of state the club may be in.
    /// </summary>
    public enum StateType
    {
        /// <summary>
        /// The club is open to the public, the bar is in service.
        /// </summary>
        OpenBar = 0,

        /// <summary>
        /// The club is open to the public but the bar is not in service.
        /// </summary>
        OpenChillzone = 1,

        /// <summary>
        /// The club is closed to the public because there's a private event.
        /// This state type is generally only seen in responses to requests authorized to state managers.
        /// </summary>
        Private = 2,

        /// <summary>
        /// The club is closed to the public.
        /// </summary>
        Closed = 3
    }
}
