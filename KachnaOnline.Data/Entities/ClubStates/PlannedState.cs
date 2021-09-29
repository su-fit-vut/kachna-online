using System;
using System.ComponentModel.DataAnnotations;
using KachnaOnline.Data.Entities.Events;
using KachnaOnline.Data.Entities.Users;

namespace KachnaOnline.Data.Entities.ClubStates
{
    public class PlannedState
    {
        [Key] public int Id { get; set; }
        [Required] public int MadeById { get; set; }
        [Required] public DateTime Start { get; set; }
        public DateTime? PlannedEnd { get; set; }
        [Required] public StateType State { get; set; }
        public DateTime? Ended { get; set; }
        public int? ClosedById { get; set; }

        [StringLength(1024)] public string NoteInternal { get; set; }
        [StringLength(1024)] public string NotePublic { get; set; }

        public int? NextPlannedStateId { get; set; }
        public int? RepeatingStateId { get; set; }
        public int? AssociatedEventId { get; set; }

        // Navigation properties
        public virtual PlannedState NextPlannedState { get; set; }
        public virtual RepeatingState RepeatingState { get; set; }
        public virtual Event AssociatedEvent { get; set; }
        public virtual User MadeBy { get; set; }
        public virtual User ClosedBy { get; set; }
    }
}
