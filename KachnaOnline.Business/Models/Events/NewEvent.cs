// NewEvent.cs
// Author: David Chocholatý

namespace KachnaOnline.Business.Models.Events
{
    /// <summary>
    /// A model representing a new event.
    /// </summary>
    public class NewEvent : ModifiedEvent
    {
        /// <summary>
        /// An ID of the events manager who created this event.
        /// </summary>
        public int MadeById { get; set; }
    }
}
