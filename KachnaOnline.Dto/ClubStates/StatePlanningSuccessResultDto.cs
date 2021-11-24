using Newtonsoft.Json;

namespace KachnaOnline.Dto.ClubStates
{
    /// <summary>
    /// A successful result of a state planning request.
    /// </summary>
    public class StatePlanningSuccessResultDto
    {
        /// <summary>
        /// Details of the newly planned state.
        /// </summary>
        public StateDto NewState { get; set; }

        /// <summary>
        /// Details of another state has been modified as a result of the newly (re-)planned state.
        /// Not included in the response if no other state has been modified.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public StateDto ModifiedState { get; set; }
    }
}
