// User.cs
// Author: Ondřej Ondryáš

namespace KachnaOnline.Business.Models.Users
{
    public class User
    {
        /// <summary>
        /// The user's unique identifier. This ID matches the user's ID in KIS.
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// The user's full name.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// The user's e-mail address.
        /// </summary>
        public string Email { get; set; }
        
        /// <summary>
        /// The user's nickname to show to the public instead of their name.
        /// </summary>
        public string Nickname { get; set; }
        
        /// <summary>
        /// The user's Discord ID to generate mentions in Discord messages.
        /// </summary>
        public ulong? DiscordId { get; set; }
        
        /// <summary>
        /// Flag signalising whether the user's account has been disabled by an administrator.
        /// </summary>
        public bool Disabled { get; set; }
    }
}
