// BoardGameDto.cs
// Author: František Nečas

namespace KachnaOnline.Dto.BoardGames
{
    /// <summary>
    /// Represents a board game in the student club as seen by a regular user (fully identified).
    /// </summary>
    public class BoardGameDto : BaseBoardGame
    {
        /// <summary>
        /// ID of the game.
        /// </summary>
        /// <example>1</example>
        public int Id { get; set; }
        
        /// <summary>
        /// Nested <see cref="CategoryDto"/> object representing the category of the game.
        /// </summary>
        public CategoryDto Category { get; set; }
    }
}
