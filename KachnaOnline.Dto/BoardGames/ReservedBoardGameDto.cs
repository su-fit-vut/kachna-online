namespace KachnaOnline.Dto.BoardGames
{
    /// <summary>
    /// A set of information returned about a reserved board game.
    /// </summary>
    public class ReservedBoardGameDto
    {
        /// <summary>
        /// ID of the board game.
        /// </summary>
        /// <example>5</example>
        public int Id { get; set; }

        /// <summary>
        /// Name of the board game.
        /// </summary>
        /// <example>Dixit</example>
        public string Name { get; set; }
    }
}
