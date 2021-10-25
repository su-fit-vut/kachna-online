// Category.cs
// Author: František Nečas

namespace KachnaOnline.Business.Models.BoardGames
{
    /// <summary>
    /// A model representing a board game category.
    /// </summary>
    public class Category
    {
        /// <summary>
        /// Unique ID of the category.
        /// </summary>
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
