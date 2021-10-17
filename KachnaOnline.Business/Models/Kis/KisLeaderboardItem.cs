// KisLeaderboardItem.cs
// Author: Ondřej Ondryáš

using System.Text.Json.Serialization;

namespace KachnaOnline.Business.Models.Kis
{
    public class KisLeaderboardItem
    {
        [JsonPropertyName("nickname")] public string Name { get; set; }
        [JsonPropertyName("prestige_gain")] public decimal Prestige { get; set; }
    }
}
