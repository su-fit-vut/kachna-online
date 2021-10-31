// CreateCategoryDto.cs
// Author: František Nečas

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KachnaOnline.Dto.BoardGames
{
    /// <summary>
    /// Represents a board game category for creation or updating.
    /// </summary>
    public class CreateCategoryDto
    {
        /// <summary>
        /// Name of the category describing the games inside (e.g. card games).
        /// </summary>
        /// <example>Hardcore games</example>
        [Required(AllowEmptyStrings = false)]
        [StringLength(64)]
        public string Name { get; set; }

        /// <summary>
        /// Colour to display the category as, consistent with the colours on the shelves
        /// in the student club.
        /// </summary>
        /// <example>000000</example>
        [Required(AllowEmptyStrings = false)]
        [DefaultValue("000000")]
        [RegularExpression("[0-9a-f]{6}")]
        public string ColourHex { get; set; }
    }
}
