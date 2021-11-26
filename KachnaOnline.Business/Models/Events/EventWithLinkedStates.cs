// EventWithLinkedStates.cs
// Author: David Chocholatý

using System.Collections.Generic;
using KachnaOnline.Business.Models.ClubStates;
using KachnaOnline.Dto.Events;

namespace KachnaOnline.Business.Models.Events
{
    /// <summary>
    /// A model representing an event with linked states.
    /// </summary>
    public class EventWithLinkedStates : Event
    {
        /// <summary>
        /// A list of linked states.
        /// </summary>
        public List<State> LinkedStates { get; set; }
    }
}
