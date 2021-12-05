// MadeByUserDto.cs
// Author: Ondřej Ondryáš

using System.ComponentModel.DataAnnotations;
using KachnaOnline.Dto.Swagger;
using Newtonsoft.Json;

namespace KachnaOnline.Dto.Users
{
    /// <summary>
    /// A set of identifiers of a user who performed an action.
    /// </summary>
    public class MadeByUserDto
    {
        /// <summary>
        /// The ID of a user.
        /// This is included in the response only when the request is authorized to a state manager or an admin.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? Id { get; set; }

        /// <summary>
        /// The full name or nickname of a user.
        /// </summary>
        [SwaggerNotNull]
        public string Name { get; set; }

        /// <summary>
        /// Nickname of the user.
        /// </summary>
        /// <example>Fifinas</example>
        [StringLength(128)]
        public string Nickname { get; set; }

        /// <summary>
        /// Email of the user.
        /// </summary>
        /// <example>foo@bar.cz</example>
        public string Email { get; set; }
    }
}
