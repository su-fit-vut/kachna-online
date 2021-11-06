// NewEventResult.cs
// Author: David Chocholatý

namespace KachnaOnline.Business.Models.Events
{
    public readonly struct NewEventResult
    {
        /// <summary>
        /// The ID of the newly planned event.
        /// </summary>
        public int NewEventId { get; init; }

        /// <summary>
        /// True if a database or synchronization error occurs when planning the event.
        /// </summary>
        public bool Failed { get; init; }

        /// <summary>
        /// True if the event was planned.
        /// </summary>
        public bool Successful => !this.Failed;
    }
}
