// Reservation.cs
// Author: Ondřej Ondryáš

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KachnaOnline.Data.Entities.Users;

namespace KachnaOnline.Data.Entities.BoardGames
{
    [Table("BoardGameReservations")]
    public class Reservation
    {
        [Key] public int Id { get; set; }
        [Required] public int MadeById { get; set; }
        [Required] public DateTime MadeOn { get; set; }
        [StringLength(1024)] public string NoteUser { get; set; }
        [StringLength(1024)] public string NoteInternal { get; set; }
        public ulong? WebhookMessageId { get; set; }

        // Navigation properties
        public virtual User MadeBy { get; set; }
        public virtual ICollection<ReservationItem> Items { get; set; }
    }
}
