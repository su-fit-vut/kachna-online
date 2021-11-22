using System;

namespace KachnaOnline.Dto.ClubStates
{
    public class RepeatingStatePlanningDto
    {
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
