// StateModificationDto.cs
// Author: Ondřej Ondryáš

using System;

namespace KachnaOnline.Dto.ClubStates
{
    /// <summary>
    /// A modification of a planned (or current) state.
    /// </summary>
    public class StateModificationDto
    {
        /// <summary>
        /// The ID of the new 'made by' user.
        /// Set to null not to modify this property.
        /// This property may only be modified by administrators.
        /// </summary>
        public int? MadeById { get; set; }

        /// <summary>
        /// The new start date and time.
        /// Set to null not to modify this property.
        /// </summary>
        public DateTime? Start { get; set; }

        /// <summary>
        /// The new planned end date and time.
        /// Set to null not to modify this property.
        /// </summary>
        public DateTime? PlannedEnd { get; set; }

        /// <summary>
        /// The new internal note.
        /// Set to an empty string to remove the note.
        /// Set to null not to modify this property.
        /// </summary>
        /// <example>This is a note that will only be visible to state managers.</example>
        public string NoteInternal { get; set; }

        /// <summary>
        /// The new public note.
        /// Set to an empty string to remove the note.
        /// Set to null not to modify this property.
        /// </summary>
        /// <example>This is a note that will be visible to everyone.</example>
        public string NotePublic { get; set; }
    }
}
