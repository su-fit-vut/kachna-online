namespace KachnaOnline.Dto.BoardGames
{
    /// <summary>
    /// Represents a board game category that is returned by the API (fully identified).
    /// </summary>
    public class CategoryDto : CreateCategoryDto
    {
        /// <summary>
        /// ID of the category.
        /// </summary>
        /// <example>5</example>
        public int Id { get; set; }
    }
}
