using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KachnaOnline.Data.Entities.BoardGames;

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

        // Navigation properties
        public virtual ICollection<UserRole> Roles { get; set; }
        public virtual ICollection<Reservation> Reservations { get; set; }
    }
}
