// ReservationDto.cs
// Author: František Nečas

using System;
using System.ComponentModel.DataAnnotations;

namespace KachnaOnline.Dto.BoardGames
{
    /// <summary>
    /// Represents a single board games reservation as seen by a regular user.
    /// </summary>
    public class ReservationDto
    {
        /// <summary>
        /// ID of the reservation.
        /// </summary>
        /// <example>1</example>
        public int Id { get; set; }
        
        /// <summary>
        /// Date which the reservation was made on.
        /// </summary>
        /// <example>2021-11-05T20:00</example>
        public DateTime MadeOn { get; set; }
        
        /// <summary>
        /// A user note.
        /// </summary>
        /// <example>For an upcoming party.</example>
        [StringLength(1024)]
        public string NoteUser { get; set; }
        
        /// <summary>
        /// Array of items in the reservation.
        /// </summary>
        public ReservationItemDto[] Items { get; set; }
    }
}
