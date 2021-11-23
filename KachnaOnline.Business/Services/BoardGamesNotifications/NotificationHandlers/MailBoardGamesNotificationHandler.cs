// MailBoardGamesNotificationHandler.cs
// Author: František Nečas

using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using KachnaOnline.Business.Configuration;
using KachnaOnline.Business.Constants;
using KachnaOnline.Business.Exceptions.BoardGames;
using KachnaOnline.Business.Models.Users;
using KachnaOnline.Business.Services.Abstractions;
using KachnaOnline.Business.Services.BoardGamesNotifications.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KachnaOnline.Business.Services.BoardGamesNotifications.NotificationHandlers
{
    /// <summary>
    /// Implements a handler sending messages via e-mail.
    /// </summary>
    public class MailBoardGamesNotificationHandler : IBoardGamesNotificationHandler
    {
        private readonly IOptionsMonitor<SmtpOptions> _smtpOptionsMonitor;
        private readonly ILogger<MailBoardGamesNotificationHandler> _logger;
        private readonly IBoardGamesService _boardGamesService;
        private readonly IUserService _userService;

        public MailBoardGamesNotificationHandler(IOptionsMonitor<SmtpOptions> smtpOptionsMonitor,
            ILogger<MailBoardGamesNotificationHandler> logger, IBoardGamesService boardGamesService,
            IUserService userService)
        {
            _smtpOptionsMonitor = smtpOptionsMonitor;
            _logger = logger;
            _boardGamesService = boardGamesService;
            _userService = userService;
        }

        /// <inheritdoc />
        /// <remarks>
        /// Does nothing.
        /// </remarks>
        public Task PerformReservationCreated(int reservationId)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        /// <remarks>
        /// Does nothing.
        /// </remarks>
        public Task PerformReservationFullyAssigned(int reservationId)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        /// <remarks>
        /// Does nothing.
        /// </remarks>
        public Task PerformReservationItemExtensionRequest(int itemId)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Sends an e-mail via SMTP.
        /// </summary>
        /// <param name="subject">Subject of the e-mail.</param>
        /// <param name="content">HTML Contents of the e-mail.</param>
        /// <param name="user">User to send the e-mail to.</param>
        private async Task SendEmail(string subject, string content, User user)
        {
            var config = _smtpOptionsMonitor.CurrentValue;
            if (string.IsNullOrEmpty(config.Address) || string.IsNullOrEmpty(config.Host) ||
                string.IsNullOrEmpty(config.Password))
            {
                _logger.LogError("SMTP authentication not available.");
                return;
            }

            var fromAddress = new MailAddress(config.Address, SmtpConstants.SenderName);
            var toAddress = new MailAddress(user.Email, user.Name);
            var smtp = new SmtpClient()
            {
                Host = config.Host,
                Port = config.Port,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(config.Address, config.Password)
            };
            var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = content,
                IsBodyHtml = true,
            };
            await smtp.SendMailAsync(message);
        }

        /// <inheritdoc />
        /// <remarks>
        /// Sends an email to the user who reserved the game notifying about the near expiration and
        /// the option of extending the reservation.
        /// </remarks>
        public async Task PerformReservationItemExpiresSoon(int itemId)
        {
            var item = await _boardGamesService.GetReservationItem(itemId);
            if (item?.ExpiresOn is null)
            {
                _logger.LogError("Item expiring soon not found in DB.");
                return;
            }

            try
            {
                var game = await _boardGamesService.GetBoardGame(item.BoardGameId);
                var reservation = await _boardGamesService.GetReservation(item.ReservationId);
                var user = await _userService.GetUser(reservation.MadeById);
                if (user is null)
                {
                    _logger.LogError("User whose reservation is about to expire not found.");
                    return;
                }

                var expiration = item.ExpiresOn.Value;
                // TODO: possibly include a link to frontend card where the game can be extended.
                var message = $@"Ahoj,<br><br>
Tvá rezervace na deskovou hru <b>{game.Name}</b> již brzy vyprší, konkrétně 
<b>{expiration.Day}. {expiration.Month}. {expiration.Year}</b>.
Domluv se, prosím, s někým ze Studentské Unie na vrácení hry zpět do klubu. V případě, že
se ti hra zalíbila a rád bys ji měl půjčenou ještě déle, můžeš také požádat v portálu člena
o prodloužení, nebo ti ji může prodloužit člen Studentské Unie, pokud se s ním domluvíš.<br><br>
Tvoje Kachna Online";
                await this.SendEmail("Platnost rezervace deskové hry v klubu U Kachničky brzy skončí!", message, user);
            }
            catch (ReservationNotFoundException)
            {
                // Should not happen, just in case
                _logger.LogError("Failed to send message about near expiration, reservation not found.");
            }
            catch (BoardGameNotFoundException)
            {
                // Should not happen, just in case
                _logger.LogError("Failed to send message about near expiration, board game not found.");
            }
        }

        /// <inheritdoc />
        /// <remarks>
        /// Sends an email to the user who reserved the game notifying about the expiration and the option
        /// of extending the reservation.
        /// </remarks>
        public async Task PerformReservationItemExpired(int itemId)
        {
            var item = await _boardGamesService.GetReservationItem(itemId);
            if (item is null)
            {
                _logger.LogError("Expired item not found in DB.");
                return;
            }

            try
            {
                var game = await _boardGamesService.GetBoardGame(item.BoardGameId);
                var reservation = await _boardGamesService.GetReservation(item.ReservationId);
                var user = await _userService.GetUser(reservation.MadeById);
                if (user is null)
                {
                    _logger.LogError("User whose reservation expired not found.");
                    return;
                }

                // TODO: possibly include a link to frontend card where the game can be extended.
                var message = $@"Ahoj,<br><br>
Tvá rezervace na deskovou hru <b>{game.Name}</b> vypršela. 
Domluv se, prosím, s někým ze Studentské Unie na vrácení hry zpět do klubu. V případě, že
se ti hra zalíbila a rád bys ji měl půjčenou ještě déle, můžeš také požádat v portálu člena
o prodloužení, nebo ti ji může prodloužit člen Studentské Unie, pokud se s ním domluvíš.<br><br>
Tvoje Kachna Online";
                await this.SendEmail("Platnost rezervace deskové hry v klubu U Kachničky vypršela!", message, user);
            }
            catch (ReservationNotFoundException)
            {
                // Should not happen, just in case
                _logger.LogError("Failed to send message about expiration, reservation not found.");
            }
            catch (BoardGameNotFoundException)
            {
                // Should not happen, just in case
                _logger.LogError("Failed to send message about expiration, board game not found.");
            }
        }
    }
}
