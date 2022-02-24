using System.Text.Json.Serialization;

namespace KachnaOnline.Business.Models.Kis
{
    public class KisUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public string Nickname { get; set; }
        [JsonPropertyName("is_member")] public bool IsMember { get; set; }
        public string[] Roles { get; set; }
    }
}
