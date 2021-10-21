// StateMadeByDto.cs
// Author: Ondřej Ondryáš

using KachnaOnline.Dto.Swagger;

namespace KachnaOnline.Dto.ClubStates
{
    /// <summary>
    /// Represents a set of identifiers of a state manager.
    /// </summary>
    public class StateMadeByDto
    {
        /// <summary>
        /// The full name or nickname of a user.
        /// </summary>
        [SwaggerNotNull]
        public string Name { get; set; }

        /// <summary>
        /// The Discord ID of a user.
        /// </summary>
        public string DiscordId { get; set; }

        /// <summary>
        /// The ID of a user. This is included in the response only when the request
        /// is authorized to a state manager or an admin. 
        /// </summary>
        public int Id { get; set; }
    }
}
