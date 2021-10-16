// StateDto.cs
// Author: Ondřej Ondryáš

using System;
using System.Text.Json.Serialization;
using KachnaOnline.Dto.Swagger;

namespace KachnaOnline.Dto.ClubState
{
    /// <summary>
    /// Represents the current or a future (planned) state of the club.
    /// </summary>
    public class StateDto
    {
        /// <summary>
        /// The current state of the club.
        /// </summary>
        public StateType State { get; set; }

        /// <summary>
        /// Information about the manager that created this state.
        /// </summary>
        public StateMadeByDto MadeBy { get; set; }

        /// <summary>
        /// The beginning of the state.
        /// </summary>
        public DateTime Start { get; set; }

        /// <summary>
        /// The planned end of the state. If null, the state has no planned end and there is no state
        /// planned in the future.
        /// </summary>
        public DateTime? PlannedEnd { get; set; }

        /// <summary>
        /// A public note.
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// The ID of an associated event. This is included in the response only when the state is linked to an event. 
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? EventId { get; set; }

        /// <summary>
        /// The next planned state. 
        /// </summary>
        [SwaggerNotNull]
        public StateDto FollowingState { get; set; }

        /// <summary>
        /// An internal note. This is included in the response only when there's a internal note and the request
        /// is authorized to a state manager.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string NoteInternal { get; set; }
    }
}
