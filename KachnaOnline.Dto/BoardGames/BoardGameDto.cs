// BoardGameDto.cs
// Author: František Nečas

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace KachnaOnline.Dto.BoardGames
{
    /// <summary>
    /// Represents a board game in the student club.
    /// </summary>
    public class BoardGameDto
    {
        /// <summary>
        /// ID of the game.
        /// </summary>
        /// <example>1</example>
        public int Id { get; set; }

        /// <summary>
        /// Full name of the game.
        /// </summary>
        /// <example>Carcassonne</example>
        [Required(AllowEmptyStrings = false)]
        [StringLength(256)]
        public string Name { get; set; }

        /// <summary>
        /// Description of the game.
        /// May be null if it is not provided.
        /// </summary>
        /// <example>The game board is a medieval landscape built by the players as the game progresses.</example>
        public string Description { get; set; }
        /// <summary>
        /// Url to an image of the game.
        /// May be null if it is not provided.
        /// </summary>
        /// <example>https://example.com/image.png</example>
        [StringLength(512)] 
        public string ImageUrl { get; set; }

        /// <summary>
        /// Minimal number of players that can play the game.
        /// May be null if it is not provided.
        /// </summary>
        /// <example>2</example>
        public int? PlayersMin { get; set; }
        /// <summary>
        /// Maximal number of players that can play the game.
        /// May be null if it is not provided.
        /// </summary>
        /// <example>5</example>
        public int? PlayersMax { get; set; }
        
        /// <summary>
        /// ID of the category. Not included in responses, it is present in the
        /// Category attribute.
        /// </summary>
        /// <example>4</example>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? CategoryId { get; set; }
        /// <summary>
        /// Nested <see cref="CategoryDto"/> object representing the category of the game.
        /// </summary>
        public CategoryDto Category { get; set; }

        /// <summary>
        /// An internal note. Included in the response only if it is done by an authorized board game manager.
        /// </summary>
        /// <example>Bought in 2018.</example>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [StringLength(1024)]
        public string NoteInternal { get; set; }

        /// <summary>
        /// ID of the owner. May be null if it is not provided (e.g. owned by Student Union).
        /// Included in the response only if it is done by an authorized board game manager.
        /// </summary>
        /// <example>100</example>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? OwnerId { get; set; }

        /// <summary>
        /// The total number of pieces of the game available in the system.
        /// Included in the response only if it is done by an authorized board game manager.
        /// </summary>
        /// <example>2</example>
        [Required]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? InStock { get; set; }
        /// <summary>
        /// The number of pieces of the game which are marked as unavailable by a board game manager.
        /// This may be done for example to prepare for an upcoming tournament in the game.
        /// Included in the response only if it is done by an authorized board game manager.
        /// </summary>
        /// <example>1</example>
        [Required]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? Unavailable { get; set; }
        /// <summary>
        /// The number of pieces of the game which are available for borrowing. This is the only
        /// number of games returned to a user who is not a board game manager.
        /// </summary>
        /// <example>1</example>
        public int Available { get; set; }
        
        /// <summary>
        /// Whether the game should be visible to regular users.
        /// Included in the response only if it is done by an authorized board game manager.
        /// </summary>
        /// <example>true</example>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [Required]
        public bool? Visible { get; set; }

        /// <summary>
        /// The default duration of reservation in days which is used when a reservation of this game is created.
        /// If it is not specified, the system-wide default is used. This allows making reservations of games,
        /// which are highly sought after, last shorter.
        /// Only included in the response if the request was done by an authorized game manager.
        /// Not included in the response if it is not specified for the game.
        /// </summary>
        /// <example>30</example>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? DefaultReservationDays { get; set; }
    }
}
