using System.Collections.Generic;

namespace KachnaOnline.Dto.ClubStates
{
    /// <summary>
    /// Contains a list of existing states that prevent a state from being planned or changed.
    /// </summary>
    public class StatePlanningConflictResultDto
    {
        /// <summary>
        /// A list of existing states that prevent a state from being planned or changed.
        /// </summary>
        public List<StateDto> CollidingStates { get; set; }
    }
}
