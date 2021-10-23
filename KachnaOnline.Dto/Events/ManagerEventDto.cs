// ManagerEventDto.cs
// Author: David Chocholatý

namespace KachnaOnline.Dto.Events
{
    /// <summary>
    /// Represents a past, a current or a future event.
    /// </summary>
    public class ManagerEventDto : EventDto
    {
        /// <summary>
        /// An ID of the events manager who created this event.
        /// </summary>
        /// <example>590589082</example>
        public int MadeById { get; set; }
    }
}
