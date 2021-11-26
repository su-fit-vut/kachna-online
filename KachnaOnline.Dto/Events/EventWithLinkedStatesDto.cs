// EventWithLinkedStatesDto.cs
// Author: David Chocholatý

using System.Collections.Generic;
using KachnaOnline.Dto.ClubStates;

namespace KachnaOnline.Dto.Events
{
    /// <summary>
    /// Represents a past, a current or a future event with linked states.
    /// </summary>
    public class EventWithLinkedStatesDto: EventDto
    {
        /// <summary>
        /// A list of states linked to the event.
        /// </summary>
        /// <example>[23, 24, 26]</example>
        public List<StateDto> LinkedStatesDtos { get; set; }
    }
}
