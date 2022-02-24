using System;

namespace KachnaOnline.Dto.ClubStates
{
    public class LegacyStateDto
    {
        public StateType NextPlannedState { get; set; }
        public DateTime? NextStateDateTime { get; set; }
        public DateTime? NextOpeningDateTime { get; set; }
        public StateType State { get; set; }
        public DateTime LastChange { get; set; }
        public DateTime? ExpectedEnd { get; set; }
        public string Note { get; set; }
        public string[] BeersOnTap { get; set; }
    }
}
