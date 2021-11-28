// BaseBoardGame.cs
// Author: František Nečas

using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace KachnaOnline.Dto.BoardGames
{
    /// <summary>
    /// Contains basic board game properties seen by all users.
    /// </summary>
    public class BaseBoardGame
    {
        /// <summary>
        /// Full name of the game.
        /// </summary>
        /// <example>Carcassonne</example>
        [Required(AllowEmptyStrings = false)]
        [StringLength(256)]
        public string Name { get; set; }

        /// <summary>
        /// Description of the game.
        /// </summary>
        /// <example>The game board is a medieval landscape built by the players as the game progresses.</example>
        public string Description { get; set; }

        /// <summary>
        /// Url to an image of the game.
        /// </summary>
        /// <example>https://example.com/image.png</example>
        [StringLength(512)]
        public string ImageUrl { get; set; }

        /// <summary>
        /// Minimal number of players that can play the game.
        /// </summary>
        /// <example>2</example>
        [Range(1, int.MaxValue)]
        public int? PlayersMin { get; set; }

        /// <summary>
        /// Maximal number of players that can play the game.
        /// </summary>
        /// <example>5</example>
        [Range(1, int.MaxValue)]
        public int? PlayersMax { get; set; }

        /// <summary>
        /// The number of pieces of the game which are available for borrowing.
        /// </summary>
        /// <example>1</example>
        [Range(0, int.MaxValue)]
        public int Available { get; set; }

        /// <summary>
        /// The total number of pieces of the game available in the club.
        /// </summary>
        /// <example>2</example>
        [Required]
        [JsonRequired]
        [Range(0, int.MaxValue)]
        public int InStock { get; set; }
    }
}
