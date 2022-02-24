namespace KachnaOnline.Business.Models.BoardGames
{
    /// <summary>
    /// Type of reservation item event.
    /// </summary>
    public enum ReservationEventType
    {
        /// <summary>
        /// Newly created, implicitly inserted on creation.
        /// </summary>
        Created = 0,

        /// <summary>
        /// Cancelled by the user before handing over.
        /// </summary>
        Cancelled = 1,

        /// <summary>
        /// Assigned to a board game manager.
        /// </summary>
        Assigned = 2,

        /// <summary>
        /// Handed over to the user.
        /// </summary>
        HandedOver = 3,

        /// <summary>
        /// User requested an extension.
        /// </summary>
        ExtensionRequested = 4,

        /// <summary>
        /// User was granted an extension.
        /// </summary>
        ExtensionGranted = 5,

        /// <summary>
        /// User was refused an extension.
        /// </summary>
        ExtensionRefused = 6,

        /// <summary>
        /// The game has been returned.
        /// </summary>
        Returned = 7
    }
}
