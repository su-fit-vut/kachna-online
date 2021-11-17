using Newtonsoft.Json;

namespace KachnaOnline.Dto.ClubStates
{
    public class StatePlanningSuccessResultDto
    {
        public StateDto NewState { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public StateDto ModifiedState { get; set; }
    }
}
