using System;

namespace KachnaOnline.Business.Models.BoardGames
{
    /// <summary>
    /// A model representing a board game reservation.
    /// </summary>
    public class Reservation
    {
        /// <summary>
        /// Unique ID of the reservation.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// ID of the user whom the reservation belongs to.
        /// </summary>
        public int MadeById { get; set; }

        /// <summary>
        /// When the reservation was initially created.
        /// </summary>
        public DateTime MadeOn { get; set; }

        /// <summary>
        /// Note related to the reservation made by the user.
        /// </summary>
        public string NoteUser { get; set; }

        /// <summary>
        /// Note related to the reservation private to the managers.
        /// </summary>
        public string NoteInternal { get; set; }

        /// <summary>
        /// ID of Discord message sent by a webhook, present if a Discord message was successfully sent.
        /// </summary>
        public ulong? WebhookMessageId { get; set; }
    }
}
