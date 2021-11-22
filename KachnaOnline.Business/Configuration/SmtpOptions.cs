// SmtpOptions.cs
// Author: František Nečas

namespace KachnaOnline.Business.Configuration
{
    public class SmtpOptions
    {
        /// <summary>
        /// SMTP server address.
        /// </summary>
        public string Host { get; set; }
        
        /// <summary>
        /// SMTP server port
        /// </summary>
        public int Port { get; set; }
        
        /// <summary>
        /// E-mail address to use for sending mail.
        /// </summary>
        public string Address { get; set; }
        
        /// <summary>
        /// Password to the e-mail <see cref="Address"/>.
        /// </summary>
        public string Password { get; set; }
    }
}
