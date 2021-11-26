// ModifiedEvent.cs
// Author: David Chocholatý

using System;
using System.Collections.Generic;

namespace KachnaOnline.Business.Models.Events
{
    /// <summary>
    /// A model representing a modified event.
    /// </summary>
    public class ModifiedEvent
    {
        /// <summary>
        /// A name of the event.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A place associated with the event.
        /// </summary>
        public string Place { get; set; }

        /// <summary>
        /// A URL for the place associated with the event.
        /// </summary>
        public string PlaceUrl { get; set; }

        /// <summary>
        /// A URL for the image associated with the event.
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// A short description of the event.
        /// </summary>
        public string ShortDescription { get; set; }

        /// <summary>
        /// A full description of the event.
        /// </summary>
        public string FullDescription { get; set; }

        /// <summary>
        /// A URL of the event.
        /// Set to <see cref="string.Empty"/> to remove the URL.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// A beginning of the event.
        /// </summary>
        public DateTime From { get; set; }

        /// <summary>
        /// An end of the event.
        /// </summary>
        public DateTime To { get; set; }

        /// <summary>
        /// The linked planned states IDs.
        /// </summary>
        public List<int> LinkedPlannedStateIds { get; set; }
    }
}
