// RepeatingStateModification.cs
// Author: Ondřej Ondryáš

using System;

namespace KachnaOnline.Business.Models.ClubStates
{
    public class RepeatingStateModification
    {
        public int RepeatingStateId { get; set; }
        public int? MadeById { get; set; }
        public StateType? State { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public string NoteInternal { get; set; }
        public string NotePublic { get; set; }
    }
}
