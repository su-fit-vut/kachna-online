using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KachnaOnline.Data.Entities.BoardGames
{
    [Table("BoardGameReservationItems")]
    public class ReservationItem
    {
        [Key] public int Id { get; set; }
        [Required] public int ReservationId { get; set; }
        [Required] public int BoardGameId { get; set; }
        public DateTime? ExpiresOn { get; set; }
        public bool NotifiedOnExpiration { get; set; }
        public bool NotifiedBeforeExpiration { get; set; }

        // Navigation properties
        public virtual Reservation Reservation { get; set; }
        public virtual BoardGame BoardGame { get; set; }
        public virtual ICollection<ReservationItemEvent> Events { get; set; }
    }
}
