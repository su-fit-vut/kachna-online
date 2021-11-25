// ClubStateOptions.cs
// Author: Ondřej Ondryáš

namespace KachnaOnline.Business.Configuration
{
    public class ClubStateOptions
    {
        public int MaximumDaysSpanForStatesListAllowed { get; set; } = 62;
        public string SuDiscordWebhookUrl { get; set; }
        public string FitwideDiscordWebhookUrl { get; set; }
    }
}
