// PushSubscription.cs
// Author: František Nečas

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KachnaOnline.Data.Entities.Users;

namespace KachnaOnline.Data.Entities.PushSubscriptions
{
    [Table("PushSubscriptions")]
    public class PushSubscription
    {
        [Key] public string Endpoint { get; set; }
        public int? MadeById { get; set; }
        [Required] public bool StateChangesEnabled { get; set; }
        [Required] public bool BoardGamesEnabled { get; set; }

        // Navigation properties
        public virtual User MadeBy { get; set; }
        public virtual ICollection<PushSubscriptionKey> Keys { get; set; }
    }
}
