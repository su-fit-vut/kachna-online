// BoardGameDto.cs
// Author: František Nečas

using System;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

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
        [BindNever]
        public int? Id { get; set; }

        /// <summary>
        /// Full name of the game.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description of the game.
        /// May be null if it is not provided.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Url to an image of the game.
        /// May be null if it is not provided.
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Minimal number of players that can play the game.
        /// May be null if it is not provided.
        /// </summary>
        public int? PlayersMin { get; set; }
        /// <summary>
        /// Maximal number of players that can play the game.
        /// May be null if it is not provided.
        /// </summary>
        public int? PlayersMax { get; set; }
        
        /// <summary>
        /// Nested <see cref="CategoryDto"/> object representing the category of the game.
        /// </summary>
        public CategoryDto Category { get; set; }

        /// <summary>
        /// An internal note. Included in the response only if it is done by an authorized board game manager.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string NoteInternal { get; set; }

        /// <summary>
        /// ID of the owner. May be null if it is not provided (e.g. owned by Student Union).
        /// Included in the response only if it is done by an authorized board game manager.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? OwnerId { get; set; }

        /// <summary>
        /// The total number of pieces of the game available in the system.
        /// </summary>
        public int InStock { get; set; }
        /// <summary>
        /// The number of pieces of the game which are marked as unavailable by a board game manager.
        /// This may be done for example to prepare for an upcoming tournament in the game.
        /// </summary>
        public int Unavailable { get; set; }
        
        /// <summary>
        /// Whether the game should be visible to regular users.
        /// Included in the response only if it is done by an authorized board game manager.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? Visible { get; set; }

        /// <summary>
        /// The default duration of reservation which is used unless a different value is explicitly
        /// specified. Only included in the response if the request was done by an authorized board game manager.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public TimeSpan? DefaultReservationTime { get; set; }
    }
}
