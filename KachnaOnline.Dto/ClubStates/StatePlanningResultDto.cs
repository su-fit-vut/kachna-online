namespace KachnaOnline.Dto.ClubStates
{
    /// <remarks>
    /// These objects are not returned from the API directly.
    /// </remarks>
    public class StatePlanningResultDto
    {
        public StatePlanningSuccessResultDto SuccessResultDto { get; set; }
        public StatePlanningConflictResultDto ConflictResultDto { get; set; }
    }
}
