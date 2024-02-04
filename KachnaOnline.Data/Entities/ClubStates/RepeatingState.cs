using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KachnaOnline.Data.Entities.Users;

namespace KachnaOnline.Data.Entities.ClubStates
{
    public class RepeatingState
    {
        [Key] public int Id { get; set; }
        [Required] public int MadeById { get; set; }
        [Required] public StateType State { get; set; }
        [Required] public DayOfWeek DayOfWeek { get; set; }

        [Required]
        [Column(TypeName = "timestamp without time zone")]
        public DateTime EffectiveFrom { get; set; }

        [Required]
        [Column(TypeName = "timestamp without time zone")]
        public DateTime EffectiveTo { get; set; }

        [Required] public TimeSpan TimeFrom { get; set; }
        [Required] public TimeSpan TimeTo { get; set; }

        [StringLength(1024)] public string NoteInternal { get; set; }
        [StringLength(1024)] public string NotePublic { get; set; }

        // Navigation properties
        public virtual ICollection<PlannedState> LinkedPlannedStates { get; set; }
        public virtual User MadeBy { get; set; }
    }
}
