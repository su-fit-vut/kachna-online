using KachnaOnline.Dto.Swagger;
using Newtonsoft.Json;

namespace KachnaOnline.Dto.ClubStates
{
    public class RepeatingStatePlanningResultDto
    {
        [SwaggerNotNull]
        public RepeatingStateManagerDto TargetRepeatingState { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public StatePlanningConflictResultDto ConflictResultDto { get; set; }
    }
}
