namespace KachnaOnline.Business.Models.ClubStates
{
    /// <summary>
    /// Represents a state the club may be in.
    /// </summary>
    public enum StateType
    {
        /// <summary>
        /// The club is open to the public, the bar is in service.
        /// </summary>
        OpenBar = 0,

        /// <summary>
        /// The club is open in a special setting.
        /// </summary>
        OpenEvent = 1,

        /// <summary>
        /// The club is closed to the public because there's a private event.
        /// This state type is generally only seen in responses to requests authorized to state managers.
        /// </summary>
        Private = 2,

        /// <summary>
        /// The club is closed to the public.
        /// </summary>
        Closed = 3,

        /// <summary>
        /// The club is open to the public in the 'tearoom' mode.
        /// </summary>
        OpenTearoom = 4,

        /// <summary>
        /// The club is open in the 'Kachna 4 everyone' mode.
        /// </summary>
        OpenAll = 5
    }
}
