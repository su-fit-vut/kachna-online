// MailOptions.cs
// Author: František Nečas, Ondřej Ondryáš

namespace KachnaOnline.Business.Configuration
{
    public class MailOptions
    {
        /// <summary>
        /// SMTP server address.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// SMTP server port
        /// </summary>
        public int Port { get; set; } = 465;

        /// <summary>
        /// E-mail address to use for sending mail.
        /// </summary>
        public string FromAddress { get; set; }

        /// <summary>
        /// SMTP username.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// SMTP password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Use SSL.
        /// </summary>
        public bool UseSsl { get; set; } = true;

        /// <summary>
        /// The sender's name.
        /// </summary>
        public string DisplayName { get; set; } = "Kachna Online";
    }
}
