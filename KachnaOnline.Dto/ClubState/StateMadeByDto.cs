// StateMadeByDto.cs
// Author: Ondřej Ondryáš

using KachnaOnline.Dto.Swagger;

namespace KachnaOnline.Dto.ClubState
{
    public class StateMadeByDto
    {
        // Public
        [SwaggerNotNull] public string Name { get; set; }
        public string DiscordId { get; set; }

        // Only managers
        public int Id { get; set; }
    }
}
