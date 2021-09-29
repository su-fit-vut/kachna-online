using System.ComponentModel.DataAnnotations;

namespace KachnaOnline.Data.Entities.Users
{
    public class Role
    {
        [Key] public int Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(64)]
        public string Name { get; set; }
    }
}
