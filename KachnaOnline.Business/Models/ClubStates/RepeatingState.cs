using System;

namespace KachnaOnline.Business.Models.ClubStates
{
    /// <summary>
    /// Represents a repeating state.
    /// Also used as a model for creating a new repeating state.
    /// </summary>
    public class RepeatingState
    {
        public int Id { get; set; }
        public int MadeById { get; set; }
        public StateType State { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime EffectiveTo { get; set; }
        public TimeSpan TimeFrom { get; set; }
        public TimeSpan TimeTo { get; set; }
        public string NoteInternal { get; set; }
        public string NotePublic { get; set; }
    }
}
