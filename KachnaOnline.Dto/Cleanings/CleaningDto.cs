using System;

namespace KachnaOnline.Dto.Cleanings
{
    /// <summary>
    /// Represents a past, a current or a future cleaning.
    /// </summary>
    public class CleaningDto : BaseCleaningDto
    {
        /// <summary>
        /// A cleaning ID.
        /// </summary>
        /// <example>395</example>
        public int Id { get; set; }

        /// <summary>
        /// Linked in cleaning service to show usernames, not just user ids, under cleanings
        /// Format is {ID, username}[]
        /// </summary>
        public Tuple<int, string>[] IdsToUsername { get; set; }
    }
}