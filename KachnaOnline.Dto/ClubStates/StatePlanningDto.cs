using System;
using System.ComponentModel.DataAnnotations;

namespace KachnaOnline.Dto.ClubStates
{
    /// <summary>
    /// A model for planning a new state.
    /// </summary>
    public class StatePlanningDto
    {
        /// <summary>
        /// The new state of the club.
        /// </summary>
        public StateType State { get; set; }

        /// <summary>
        /// The beginning date and time of the state.
        /// Set to null to start the new state now.
        /// </summary>
        public DateTime? Start { get; set; }

        /// <summary>
        /// The planned end of the state.
        /// Set to null to set the end to the start of the following planned state.
        /// </summary>
        public DateTime? PlannedEnd { get; set; }

        /// <summary>
        /// An internal note.
        /// </summary>
        /// <example>This is a note that will only be visible to state managers.</example>
        [StringLength(1024)]
        public string NoteInternal { get; set; }

        /// <summary>
        /// A public note.
        /// </summary>
        /// <example>This is a note that will be visible to everyone.</example>
        [StringLength(1024)]
        public string NotePublic { get; set; }
    }
}
