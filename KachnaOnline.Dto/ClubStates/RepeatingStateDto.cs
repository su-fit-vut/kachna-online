using System;

namespace KachnaOnline.Dto.ClubStates
{
    /// <summary>
    /// A repeating state. These serve as a 'template' for making recurrent planned states.
    /// </summary>
    public class RepeatingStateDto
    {
        /// <summary>
        /// The type of this repeating state and its occurrences.
        /// </summary>
        public StateType State { get; set; }

        /// <summary>
        /// The day of week when this repeating state occurs.
        /// </summary>
        public DayOfWeek DayOfWeek { get; set; }

        /// <summary>
        /// The start date of this repeating state's activity.
        /// </summary>
        public DateTime EffectiveFrom { get; set; }

        /// <summary>
        /// The end date of this repeating state's activity.
        /// </summary>
        public DateTime EffectiveTo { get; set; }

        /// <summary>
        /// The time when this repeating state's occurrences start.
        /// </summary>
        /// <example>16:00</example>
        public TimeSpan TimeFrom { get; set; }

        /// <summary>
        /// The time when this repeating state's occurrences end.
        /// </summary>
        /// <example>21:50</example>
        public TimeSpan TimeTo { get; set; }

        /// <summary>
        /// A public note.
        /// </summary>
        /// <example>This is a note visible to everyone.</example>
        public string Note { get; set; }
    }
}
