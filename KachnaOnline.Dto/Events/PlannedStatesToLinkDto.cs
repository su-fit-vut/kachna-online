using System.ComponentModel.DataAnnotations;

namespace KachnaOnline.Dto.Events
{
    /// <summary>
    /// Represents a list of planned states to link to an event.
    /// </summary>
    public class PlannedStatesToLinkDto
    {
        /// <summary>
        /// IDs of the planned states to be linked to the event.
        /// </summary>
        /// <remarks>
        /// Put an empty list to unlink all already linked planned states from the event.
        /// </remarks>
        /// <example>[342, 343, 345]</example>
        [Required]
        public int[] PlannedStateIds { get; set; }
    }
}
