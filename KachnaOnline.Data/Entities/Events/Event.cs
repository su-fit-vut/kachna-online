// Event.cs
// Author: Ondřej Ondryáš

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using KachnaOnline.Data.Entities.ClubStates;
using KachnaOnline.Data.Entities.Users;

namespace KachnaOnline.Data.Entities.Events
{
    public class Event
    {
        [Key] public int Id { get; set; }
        [Required] public int MadeById { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(128)]
        public string Name { get; set; }

        [StringLength(256)]
        public string Place { get; set; }
        [StringLength(512)]
        public string PlaceUrl { get; set; }
        [StringLength(512)]
        public string ImageUrl { get; set; }

        [StringLength(512)] [Required] public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        [StringLength(512)] public string Url { get; set; }
        [Required] public DateTime From { get; set; }
        [Required] public DateTime To { get; set; }

        // Navigation properties
        public virtual User MadeBy { get; set; }
        public virtual ICollection<PlannedState> LinkedPlannedStates { get; set; }
    }
}
