using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KachnaOnline.Data.Entities.Users;

namespace KachnaOnline.Data.Entities.BoardGames
{
    [Table("BoardGameReservationItemEvents")]
    public class ReservationItemEvent
    {
        [Required] public int ReservationItemId { get; set; }
        [Required] public int MadeById { get; set; }
        [Required] public DateTime MadeOn { get; set; }
        [Required] public ReservationItemState NewState { get; set; }
        [Required] public ReservationEventType Type { get; set; }
        public DateTime? NewExpiryDateTime { get; set; }

        // Navigation properties
        public virtual ReservationItem ReservationItem { get; set; }
        public virtual User MadeBy { get; set; }
    }
}
