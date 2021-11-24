using System;

namespace KachnaOnline.Dto.ClubStates
{
    /// <summary>
    /// A model for planning a new repeating state.
    /// </summary>
    public class RepeatingStatePlanningDto
    {
        /// <summary>
        /// The type of the new repeating state and its occurrences.
        /// </summary>
        public StateType State { get; set; }

        /// <summary>
        /// The day of week when the new repeating state will occur.
        /// </summary>
        public DayOfWeek DayOfWeek { get; set; }

        /// <summary>
        /// The start date of the repeating state's activity.
        /// </summary>
        public DateTime EffectiveFrom { get; set; }

        /// <summary>
        /// The end date of the repeating state's activity.
        /// </summary>
        public DateTime EffectiveTo { get; set; }

        /// <summary>
        /// The time when the new repeating state's occurrences start.
        /// </summary>
        /// <example>16:00</example>
        public TimeSpan TimeFrom { get; set; }

        /// <summary>
        /// The time when the new repeating state's occurrences end.
        /// </summary>
        /// <example>21:50</example>
        public TimeSpan TimeTo { get; set; }

        /// <summary>
        /// An internal note.
        /// </summary>
        /// <example>This is a note that will only be visible to state managers.</example>
        public string NoteInternal { get; set; }

        /// <summary>
        /// A public note.
        /// </summary>
        /// <example>This is a note that will be visible to everyone.</example>
        public string NotePublic { get; set; }
    }
}
