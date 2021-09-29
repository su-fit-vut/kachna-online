using System.ComponentModel.DataAnnotations;

namespace KachnaOnline.Data.Entities.Users
{
    public class UserRole
    {
        [Required] public int UserId { get; set; }
        [Required] public int RoleId { get; set; }
        public int? AssignedByUserId { get; set; }
        
        // Navigation properties
        public virtual User User { get; set; }
        public virtual User AssignedByUser { get; set; }
        public virtual Role Role { get; set; }
    }
}
