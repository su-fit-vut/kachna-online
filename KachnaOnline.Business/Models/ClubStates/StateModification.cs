// StateModification.cs
// Author: Ondřej Ondryáš

using System;

namespace KachnaOnline.Business.Models.ClubStates
{
    /// <summary>
    /// A model for modifying a club state.
    /// </summary>
    public class StateModification
    {
        /// <summary>
        /// The ID of the modified state.
        /// Set to null to assume the current state.
        /// </summary>
        public int? StateId { get; set; }

        /// <summary>
        /// The ID of the new 'made by' user.
        /// Set to null not to modify this property.
        /// </summary>
        public int? MadeById { get; set; }

        /// <summary>
        /// The new start.
        /// Set to null not to modify this property.
        /// </summary>
        public DateTime? Start { get; set; }

        /// <summary>
        /// The new planned end.
        /// Set to null not to modify this property.
        /// </summary>
        public DateTime? PlannedEnd { get; set; }

        /// <summary>
        /// The new internal note.
        /// Set to <see cref="string.Empty"/> to remove the note.
        /// Set to null not to modify this property.
        /// </summary>
        public string NoteInternal { get; set; }

        /// <summary>
        /// The new public note.
        /// Set to <see cref="string.Empty"/> to remove the note.
        /// Set to null not to modify this property.
        /// </summary>
        public string NotePublic { get; set; }
    }
}
