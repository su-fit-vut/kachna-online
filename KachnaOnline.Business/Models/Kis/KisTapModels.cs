// KisTapModels.cs
// Author: Ondřej Ondryáš

using System.Text.Json.Serialization;

namespace KachnaOnline.Business.Models.Kis
{
    /// <summary>
    /// Maps KIS #/components/schemas/labels.
    /// </summary>
    public class KisLabel
    {
        public string Color { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
    }

    /// <summary>
    /// Maps a subset of KIS #/components/schemas/article_listing (and /offered_article by inheritance).
    /// </summary>
    public class KisArticle
    {
        public int Id { get; set; }
        [JsonPropertyName("image_url")] public string ImageUrl { get; set; }
        public string Name { get; set; }
        public KisLabel[] Labels { get; set; }
        [JsonPropertyName("in_stock")]
        public decimal? InStock { get; set; }
        public decimal Price { get; set; }
        public decimal Prestige { get; set; }
    }

    /// <summary>
    /// Maps KIS #/components/schemas/unsealed_keg.
    /// </summary>
    public class KisBeerKeg
    {
        public KisArticle Article { get; set; }
        public int Id { get; set; }

        [JsonPropertyName("remaining_content")]
        public int RemainingContent { get; set; }
    }

    /// <summary>
    /// Maps KIS /beer/taps/{tap_id} response.
    /// </summary>
    public class KisTapInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [JsonPropertyName("offered_articles")] public KisArticle[] OfferedArticles { get; set; }
    }
}
