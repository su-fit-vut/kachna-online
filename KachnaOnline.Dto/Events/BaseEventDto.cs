// BaseEventDto.cs
// Author: David Chocholatý

using System;
using System.ComponentModel.DataAnnotations;

namespace KachnaOnline.Dto.Events
{
    /// <summary>
    /// Represents a base event DTO.
    /// </summary>
    public class BaseEventDto
    {
        /// <summary>
        /// A name of the event.
        /// </summary>
        /// <example>Start@FIT</example>
        [StringLength(128)]
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }

        /// <summary>
        /// A place associated with the event.
        /// </summary>
        /// <example>FIT BUT</example>
        [StringLength(256)]
        public string Place { get; set; }

        /// <summary>
        /// A URL for the place associated with the event.
        /// </summary>
        /// <example>https://www.placeurl.com</example>
        [StringLength(512)]
        public string PlaceUrl { get; set; }

        /// <summary>
        /// A URL for the image associated with the event.
        /// </summary>
        /// <example>https://www.imageurl.com</example>
        [StringLength(512)]
        public string ImageUrl { get; set; }

        /// <summary>
        /// A short description of the event.
        /// </summary>
        /// <example>Meet the faculty.</example>
        [StringLength(512)]
        [Required(AllowEmptyStrings = false)]
        public string ShortDescription { get; set; }

        /// <summary>
        /// A full description of the event.
        /// </summary>
        /// <example>Event will be held at the courtyard and the main hall of FIT BUT. The attendees will be officially
        /// welcomed at the faculty and introduced to the inner workings of the life at the faculty. All this while
        /// enjoying additional activities or going to grab an alcoholic liquor in the students club U Kachničky.</example>
        public string FullDescription { get; set; }

        /// <summary>
        /// A URL of the event.
        /// </summary>
        /// <example>https://www.url.com</example>
        [StringLength(512)]
        public string Url { get; set; }

        /// <summary>
        /// A beginning of the event.
        /// </summary>
        /// <example>2022-09-18T12:30</example>
        [Required]
        public DateTime From { get; set; }

        /// <summary>
        /// An end of the event.
        /// </summary>
        /// <example>2022-09-20T19:00</example>
        [Required]
        public DateTime To { get; set; }

        /// <summary>
        /// The linked planned states IDs.
        /// </summary>
        /// <example>[357, 358, 360]</example>
        public int[] LinkedPlannedStateIds { get; set; }
    }
}
