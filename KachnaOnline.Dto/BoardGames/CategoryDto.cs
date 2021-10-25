// CategoryDto.cs
// Author: František Nečas

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
        public string Name { get; set; }

        /// <summary>
        /// Colour to display the category as, consistent with the colours on the shelves
        /// in the student club.
        /// </summary>
        public string ColourHex { get; set; }
    }
}
