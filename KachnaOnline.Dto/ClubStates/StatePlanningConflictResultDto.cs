using System.Collections.Generic;
using Newtonsoft.Json;

namespace KachnaOnline.Dto.ClubStates
{
    public class StatePlanningConflictResultDto
    {
        public List<StateDto> CollidingStates { get; set; }
    }
}
