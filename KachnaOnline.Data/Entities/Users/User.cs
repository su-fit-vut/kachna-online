// User.cs
// Author: Ondřej Ondryáš

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KachnaOnline.Data.Entities.BoardGames;
using KachnaOnline.Data.Entities.PushSubscriptions;

namespace KachnaOnline.Data.Entities.Users
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(320)]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(128)]
        public string Name { get; set; }

        [StringLength(128)] public string Nickname { get; set; }

        public ulong? DiscordId { get; set; }

        [Required] public bool Disabled { get; set; } = false;

        // Navigation properties
        public virtual ICollection<UserRole> Roles { get; set; }
        public virtual ICollection<Reservation> Reservations { get; set; }
        public virtual ICollection<PushSubscription> PushSubscriptions { get; set; }
    }
}
