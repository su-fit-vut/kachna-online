// BoardGame.cs
// Author: Ondřej Ondryáš

using System;
using System.ComponentModel.DataAnnotations;
using KachnaOnline.Data.Entities.Users;

namespace KachnaOnline.Data.Entities.BoardGames
{
    public class BoardGame
    {
        [Key] public int Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(256)]
        public string Name { get; set; }

        public string Description { get; set; }
        [StringLength(512)] public string ImageUrl { get; set; }

        public int? PlayersMin { get; set; }
        public int? PlayersMax { get; set; }
        [Required] public int CategoryId { get; set; }

        [StringLength(1024)] public string NoteInternal { get; set; }

        public int? OwnerId { get; set; }

        [Required] [ConcurrencyCheck] public int InStock { get; set; }
        [Required] public int Unavailable { get; set; }
        [Required] public bool Visible { get; set; }

        public TimeSpan? DefaultReservationTime { get; set; }

        // Navigation properties
        public virtual Category Category { get; set; }
        public virtual User Owner { get; set; }
    }
}
