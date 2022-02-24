using KachnaOnline.Business.Models.Users;

namespace KachnaOnline.Business.Extensions
{
    public static class UserExtensions
    {
        public static string GetDiscordMention(this User user, bool showName = false)
        {
            if (user is null)
                return null;

            if (!user.DiscordId.HasValue)
                return user.Nickname ?? (showName ? user.Name : null);

            return $"<@{user.DiscordId.Value}>";
        }
    }
}
