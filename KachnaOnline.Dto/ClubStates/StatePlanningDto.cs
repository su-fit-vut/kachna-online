// StatePlanningDto.cs
// Author: Ondřej Ondryáš

using System;

namespace KachnaOnline.Dto.ClubStates
{
    public class StatePlanningDto
    {
        public StateType State { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? PlannedEnd { get; set; }
        public string NoteInternal { get; set; }
        public string NotePublic { get; set; }
    }
}
