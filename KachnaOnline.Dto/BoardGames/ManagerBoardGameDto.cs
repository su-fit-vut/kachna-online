// ManagerBoardGameDto.cs
// Author: František Nečas

using System.ComponentModel.DataAnnotations;
using KachnaOnline.Dto.Users;

namespace KachnaOnline.Dto.BoardGames
{
    /// <summary>
    /// Represents a board game as seen by a board game manager (fully identified).
    /// </summary>
    public class ManagerBoardGameDto : BoardGameDto
    {
        /// <summary>
        /// An internal note.
        /// </summary>
        /// <example>Bought in 2018.</example>
        [StringLength(1024)]
        public string NoteInternal { get; set; }

        /// <summary>
        /// Details about the owner. May be null if it is not provided (e.g. owned by Student Union).
        /// </summary>
        public MadeByUserDto Owner { get; set; }

        /// <summary>
        /// The number of pieces of the game which are marked as unavailable by a board game manager.
        /// This may be done for example to prepare for an upcoming tournament in the game.
        /// </summary>
        /// <example>1</example>
        [Required]
        public int Unavailable { get; set; }


        /// <summary>
        /// Whether the game should be visible to regular users.
        /// </summary>
        /// <example>true</example>
        [Required]
        public bool Visible { get; set; }

        /// <summary>
        /// The default duration of reservation in days which is used when a reservation of this game is created.
        /// If it is not specified, the system-wide default is used. This allows making reservations of games,
        /// which are highly sought after, last shorter.
        /// </summary>
        /// <example>30</example>
        [Range(1, int.MaxValue)]
        public int? DefaultReservationDays { get; set; }
    }
}
