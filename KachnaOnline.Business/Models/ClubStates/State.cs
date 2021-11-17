// State.cs
// Author: Ondřej Ondryáš

using System;

namespace KachnaOnline.Business.Models.ClubStates
{
    /// <summary>
    /// Represents a club state, past, current or planned.
    /// </summary>
    public class State
    {
        public int Id { get; set; }
        public int? MadeById { get; set; }
        public StateType Type { get; set; }
        public DateTime Start { get; set; }
        public DateTime? PlannedEnd { get; set; }

        public DateTime? Ended { get; set; }
        public int? ClosedById { get; set; }

        public string NoteInternal { get; set; }
        public string NotePublic { get; set; }

        public int? EventId { get; set; }

        public State FollowingState { get; set; }
    }
}
