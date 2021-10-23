// RepeatingStatePlanningResult.cs
// Author: Ondřej Ondryáš

using System;
using System.Collections.Generic;

namespace KachnaOnline.Business.Models.ClubStates
{
    public class RepeatingStatePlanningResult
    {
        /// <summary>
        /// The ID of the created or modified Repeating State
        /// </summary>
        public int TargetRepeatingStateId { get; set; }
        public List<State> OverlappingStates { get; set; }
        public bool HasOverlappingStates => this.OverlappingStates is { Count: >0 };
    }
}
