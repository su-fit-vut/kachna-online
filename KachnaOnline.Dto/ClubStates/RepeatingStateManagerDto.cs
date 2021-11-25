using KachnaOnline.Dto.Users;

namespace KachnaOnline.Dto.ClubStates
{
    /// <summary>
    /// A repeating state that includes attributes that are only visible to state managers.
    /// </summary>
    public class RepeatingStateManagerDto : RepeatingStateDto
    {
        /// <summary>
        /// The ID of the repeating state.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Information about the manager that created this repeating state.
        /// </summary>
        public MadeByUserDto MadeByUser { get; set; }

        /// <summary>
        /// An internal note.
        /// </summary>
        /// <example>This is a note visible only to state managers.</example>
        public string NoteInternal { get; set; }
    }
}
