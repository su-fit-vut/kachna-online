// PastStateDto.cs
// Author: Ondřej Ondryáš

using System;
using Newtonsoft.Json;

namespace KachnaOnline.Dto.ClubStates
{
    /// <summary>
    /// A state of the club that has already ended.
    /// </summary>
    public class PastStateDto : StateDto
    {
        /// <summary>
        /// The actual date and time of when this state ended.
        /// </summary>
        public DateTime ActualEnd { get; set; }

        /// <summary>
        /// The ID of the user that has triggered the end of this state.
        /// This is included in the response only when the state was manually ended by a user.
        /// This is included in the response only when the request is authorized to a state manager or an admin.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public StateMadeByDto ClosedBy { get; set; }
    }
}
