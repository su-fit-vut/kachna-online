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
        /// Details of state planning conflicts that occurred when (re-)planning the occurrences of the repeating state.
        /// Not included in the response if no conflicts occured.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public StatePlanningConflictResultDto ConflictResultDto { get; set; }
    }
}
