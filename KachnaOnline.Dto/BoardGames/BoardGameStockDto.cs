// BoardGameStockDto.cs
// Author: František Nečas

using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace KachnaOnline.Dto.BoardGames
{
    /// <summary>
    /// Represents stock of a board game and its visibility.
    /// </summary>
    public class BoardGameStockDto
    {
        /// <summary>
        /// The total number of pieces of the game available in the club.
        /// </summary>
        /// <example>2</example>
        [Required]
        [JsonRequired]
        public int InStock { get; set; }

        /// <summary>
        /// The number of pieces of the game which are marked as unavailable by a board game manager.
        /// This may be done for example to prepare for an upcoming tournament in the game.
        /// </summary>
        /// <example>1</example>
        [Required]
        [JsonRequired]
        public int Unavailable { get; set; }

        /// <summary>
        /// Whether the game should be visible to regular users.
        /// </summary>
        /// <example>false</example>
        [Required]
        [JsonRequired]
        public bool Visible { get; set; }
    }
}
