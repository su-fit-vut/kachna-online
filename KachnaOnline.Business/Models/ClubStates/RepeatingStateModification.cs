// RepeatingStateModification.cs
// Author: Ondřej Ondryáš

using System;

namespace KachnaOnline.Business.Models.ClubStates
{
    /// <summary>
    /// A model for modifying a repeating state (and its planned states).
    /// </summary>
    public class RepeatingStateModification
    {
        /// <summary>
        /// The ID of the modified repeating state.
        /// </summary>
        public int RepeatingStateId { get; set; }

        /// <summary>
        /// The ID of the new 'made by' user for the repeating state and its planned states that haven't started yet.
        /// Set to null not to modify this property. 
        /// </summary>
        public int? MadeById { get; set; }

        /// <summary>
        /// The new state type for the repeating state and its planned states that haven't started yet.
        /// Set to null not to modify this property. 
        /// </summary>
        public StateType? State { get; set; }

        /// <summary>
        /// The new date the repeating state will be effective to.
        /// This must be a date higher then or equal to today.
        /// Set to null not to modify this property.
        /// </summary>
        public DateTime? EffectiveTo { get; set; }

        /// <summary>
        /// The new internal note for the repeating state and its planned states that haven't started yet.
        /// Set to <see cref="string.Empty"/> to remove the note.
        /// Set to null not to modify this property.
        /// </summary>
        public string NoteInternal { get; set; }

        /// <summary>
        /// The new public note for the repeating state and its planned states that haven't started yet.
        /// Set to <see cref="string.Empty"/> to remove the note.
        /// Set to null not to modify this property.
        /// </summary>
        public string NotePublic { get; set; }
    }
}
