using System;

namespace KachnaOnline.Dto.ClubStates
{
    public class RepeatingStateDto
    {
        /// <summary>
        /// The type of this state.
        /// </summary>
        public StateType State { get; set; }

        /// <summary>
        /// The day of week when this state occurs.
        /// </summary>
        public DayOfWeek DayOfWeek { get; set; }

        /// <summary>
        /// The start date of this repeating state.
        /// </summary>
        public DateTime EffectiveFrom { get; set; }

        /// <summary>
        /// The end date of this repeating state.
        /// </summary>
        public DateTime EffectiveTo { get; set; }

        /// <summary>
        /// The time when this state's occurrences start.
        /// </summary>
        public TimeSpan TimeFrom { get; set; }

        /// <summary>
        /// The time when this state's occurrences end.
        /// </summary>
        public TimeSpan TimeTo { get; set; }

        /// <summary>
        /// The public note of this state.
        /// </summary>
        public string Note { get; set; }
    }
}
