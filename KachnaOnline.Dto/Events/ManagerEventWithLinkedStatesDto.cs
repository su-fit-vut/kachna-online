// ManagerEventWithLinkedStatesDto.cs
// Author: David Chocholatý

namespace KachnaOnline.Dto.Events
{
    /// <summary>
    /// Represents a past, a current or a future event with linked planned states as seen by events manager.
    /// </summary>
    public class ManagerEventWithLinkedStatesDto : EventWithLinkedStatesDto
    {
        /// <summary>
        /// An ID of the events manager who created this event.
        /// </summary>
        /// <example>15</example>
        public int MadeById { get; set; }
    }
}
