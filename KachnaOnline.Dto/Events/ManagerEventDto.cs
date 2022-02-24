namespace KachnaOnline.Dto.Events
{
    /// <summary>
    /// Represents a past, a current or a future event as seen by events manager.
    /// </summary>
    public class ManagerEventDto : EventDto
    {
        /// <summary>
        /// An ID of the events manager who created this event.
        /// </summary>
        /// <example>15</example>
        public int MadeById { get; set; }
    }
}
