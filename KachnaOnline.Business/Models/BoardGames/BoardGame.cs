// BoardGame.cs
// Author: František Nečas

using System;

namespace KachnaOnline.Business.Models.BoardGames
{
    /// <summary>
    /// A model representing a board game.
    /// </summary>
    public class BoardGame
    {
        /// <summary>
        /// Unique Id of the board game.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of the board game.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description of the board game.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Image URL of the board game.
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Optional minimal number of players that can play the game.
        /// </summary>
        public int? PlayersMin { get; set; }
        /// <summary>
        /// Optional maximal number of players that can play the game.
        /// </summary>
        public int? PlayersMax { get; set; }
        
        /// <summary>
        /// Id of the <see cref="Category"/>.
        /// </summary>
        public int CategoryId { get; set; }
        /// <summary>
        /// <see cref="Category"/> of the board game.
        /// </summary>
        public Category Category { get; set; }

        /// <summary>
        /// Note related to the board game kept private to the managers.
        /// </summary>
        public string NoteInternal { get; set; }

        /// <summary>
        /// Id of the game owner.
        /// If set to null, the owner is not known or SU owns the game.
        /// </summary>
        public int? OwnerId { get; set; }

        /// <summary>
        /// Total number of pieces of the game available in the system.
        /// </summary>
        public int InStock { get; set; }
        /// <summary>
        /// The number of pieces of the game which are set as unavailable by a board games manager.
        /// For example they may be intentionally blocked from being borrowed due to an upcoming tournament.
        /// </summary>
        public int Unavailable { get; set; }
        /// <summary>
        /// The number of pieces of the game which are currently available based on reservations.
        /// </summary>
        public int Available { get; set; }
        /// <summary>
        /// Whether the game is visible to the public.
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// The duration of reservations unless explicitly specified.
        /// </summary>
        public TimeSpan? DefaultReservationTime { get; set; }
    }
}
