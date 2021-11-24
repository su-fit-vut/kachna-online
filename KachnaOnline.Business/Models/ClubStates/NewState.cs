// NewState.cs
// Author: Ondřej Ondryáš

using System;

namespace KachnaOnline.Business.Models.ClubStates
{
    /// <summary>
    /// A model for planning a new state.
    /// </summary>
    public class NewState
    {
        public int MadeById { get; set; }
        public StateType Type { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? PlannedEnd { get; set; }
        public int? FollowingStateId { get; set; }

        public string NoteInternal { get; set; }
        public string NotePublic { get; set; }

        internal int? RepeatingStateId { get; set; }
    }
}
