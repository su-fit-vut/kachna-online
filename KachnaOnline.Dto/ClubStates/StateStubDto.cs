namespace KachnaOnline.Dto.ClubStates
{
    /// <summary>
    /// A state of the club with no additional info apart from its ID.
    /// </summary>
    /// <remarks>
    /// This is a 'stub' parent class of <see cref="StateDto"/>. It is used as the type of its
    /// <see cref="StateDto.FollowingState"/> property. This allows the API to return the ID of a state's following
    /// state without providing the full data of the following state. This is used in endpoints in which it isn't
    /// useful to return all information about following states but existence of such state should be represented.
    /// </remarks>
    public class StateStubDto
    {
        /// <summary>
        /// The ID of the state.
        /// </summary>
        public int Id { get; set; }
    }
}
