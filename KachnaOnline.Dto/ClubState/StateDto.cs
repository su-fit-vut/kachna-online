// StateDto.cs
// Author: Ondřej Ondryáš

using System;

namespace KachnaOnline.Dto.ClubState
{
    public class StateDto
    {
        /// <summary>
        /// The current state of the club.
        /// </summary>
        public StateType State { get; set; }

        public StateMadeByDto MadeBy { get; set; }

        public DateTime Start { get; set; }
        public DateTime? PlannedEnd { get; set; }

        public string Note { get; set; }

        public int? EventId { get; set; }
        public StateDto FollowingState { get; set; }

        // Only managers
        public string NoteInternal { get; set; }
    }
}
