using Newtonsoft.Json;

namespace KachnaOnline.Dto.Images
{
    public class ImageDto
    {
        public string Url { get; set; }
        public string Hash { get; set; }

        [JsonIgnore] public bool Exists { get; set; }
    }
}
