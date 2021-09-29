using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KachnaOnline.Data.Entities.BoardGames
{
    [Table("BoardGameCategories")]
    public class Category
    {
        [Key] public int Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(64)]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false)]
        [DefaultValue("000000")]
        [RegularExpression("[0-9a-f]{6}")]
        public string ColourHex { get; set; }
        
        // Navigation properties
        public virtual ICollection<BoardGame> Games { get; set; }
    }
}
