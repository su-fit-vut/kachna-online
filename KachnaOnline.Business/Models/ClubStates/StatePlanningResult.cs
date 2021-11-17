// NewStateResult.cs
// Author: Ondřej Ondryáš

using System;
using System.Collections.Generic;

namespace KachnaOnline.Business.Models.ClubStates
{
    /// <summary>
    /// Represents a result of a state planning attempt. 
    /// </summary>
    public readonly struct StatePlanningResult
    {
        /// <summary>
        /// The ID of the newly (re-)planned state.
        /// </summary>
        public int TargetStateId { get; init; }

        /// <summary>
        /// The planned end of the newly (re-)planned state.
        /// </summary>
        public DateTime TargetStatePlannedEnd { get; init; }

        /// <summary>
        /// The ID of state that has been modified as a result of the newly (re-)planned state.
        /// Null if no state has been modified.
        /// </summary>
        public int? ModifiedPreviousStateId { get; init; }

        /// <summary>
        /// The new planned end of state that has been modified as a result of the newly (re-)planned state.
        /// Null if no state has been modified.
        /// </summary>
        public DateTime? ModifiedPreviousStatePlannedEnd { get; init; }
        
        /// <summary>
        /// An array of already planned states that prevented the state to be planned. 
        /// </summary>
        public List<int> OverlappingStatesIds { get; init; }

        /// <summary>
        /// True if the state was prevented from being planned due to another state overlapping with it.
        /// </summary>
        public bool HasOverlappingStates => this.OverlappingStatesIds is { Count: >0 };
    }
}
