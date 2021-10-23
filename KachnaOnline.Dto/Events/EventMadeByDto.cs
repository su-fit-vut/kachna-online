// EventMadeByDto.cs
// Author: David Chocholatý

using KachnaOnline.Dto.Swagger;

namespace KachnaOnline.Dto.Events
{
    /// <summary>
    /// Represents a set of identifiers of an event manager.
    /// </summary>
    public class EventMadeByDto
    {
        /// <summary>
        /// The full name or nickname of a user.
        /// </summary>
        /// <example>David Chocholatý</example>
        [SwaggerNotNull]
        public string Name { get; set; }

        /// <summary>
        /// The Discord ID of a user.
        /// </summary>
        /// <example>4728490234923742340723</example>
        public string DiscordId { get; set; }

        /// <summary>
        /// The ID of a user. This is included in the response only when the request
        /// is authorized to an event manager or an admin.
        /// </summary>
        /// <example>3812313108</example>
        public int Id { get; set; }
    }
}
