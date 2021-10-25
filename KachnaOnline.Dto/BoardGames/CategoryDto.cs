// CategoryDto.cs
// Author: František Nečas

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace KachnaOnline.Dto.BoardGames
{
    /// <summary>
    /// Represents a board game category.
    /// </summary>
    public class CategoryDto
    {
        /// <summary>
        /// ID of the category.
        /// </summary>
        [BindNever]
        public int Id { get; set; }

        /// <summary>
        /// Name of the category describing the games inside (e.g. card games).
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [StringLength(64)]
        public string Name { get; set; }

        /// <summary>
        /// Colour to display the category as, consistent with the colours on the shelves
        /// in the student club.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [DefaultValue("000000")]
        [RegularExpression("[0-9a-f]{6}")]
        public string ColourHex { get; set; }
    }
}
