// DiscordMessage.cs
// Author: František Nečas

using System;

namespace KachnaOnline.Business.Models.Discord
{
    /// <summary>
    /// Represents a single Discord message as returned by Discord.
    /// </summary>
    /// <remarks>
    /// Only the attributes that we use or could be useful are included here. For full information, refer to:
    ///     https://discord.com/developers/docs/resources/channel#message-object
    /// </remarks>
    public class DiscordMessage
    {
        /// <summary>
        /// Unique ID of the message.
        /// </summary>
        public ulong Id { get; set; }
        
        /// <summary>
        /// ID of the channel that the message is in.
        /// </summary>
        public ulong ChannelId { get; set; }
        
        /// <summary>
        /// When the message was posted.
        /// </summary>
        public DateTime Timestamp { get; set; }
    }
}
