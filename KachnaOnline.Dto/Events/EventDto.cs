// EventDto.cs
// Author: David Chocholatý

namespace KachnaOnline.Dto.Events
{
    /// <summary>
    /// Represents a past, a current or a future event.
    /// </summary>
    public class EventDto : BaseEventDto
    {
        /// <summary>
        /// An event ID.
        /// </summary>
        /// <example>395</example>
        public int Id { get; set; }
    }
}
