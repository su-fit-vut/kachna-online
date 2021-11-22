namespace KachnaOnline.Dto.ClubStates
{
    public class RepeatingStateManagerDto : RepeatingStateDto
    {
        /// <summary>
        /// The ID of the state.
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Information about the manager that created this state.
        /// </summary>
        public StateMadeByDto MadeBy { get; set; }
        
        /// <summary>
        /// An internal note.
        /// </summary>
        public string NoteInternal { get; set; }
    }
}
