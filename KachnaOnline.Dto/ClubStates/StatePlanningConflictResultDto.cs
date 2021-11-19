using System.Collections.Generic;
using Newtonsoft.Json;

namespace KachnaOnline.Dto.ClubStates
{
    public class StatePlanningConflictResultDto
    {
        /// <summary>
        /// A list of existing states that prevent a state from being planned or changed.
        /// </summary>
        public List<StateDto> CollidingStates { get; set; }
    }
}
