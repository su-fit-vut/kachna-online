using System.Collections.Generic;
using KachnaOnline.Dto.Swagger;
using Newtonsoft.Json;

namespace KachnaOnline.Dto.ClubStates
{
    /// <summary>
    /// A result of a repeating state planning request.
    /// </summary>
    public class RepeatingStatePlanningResultDto
    {
        /// <summary>
        /// The created or modified repeating state.
        /// </summary>
        [SwaggerNotNull]
        public RepeatingStateManagerDto TargetRepeatingState { get; set; }

        /// <summary>
        /// A list of existing states that prevent a new repeating state occurrence from being planned.
        /// Not included in the response if no conflicts occured.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<StateDto> CollidingStates { get; set; }
    }
}
