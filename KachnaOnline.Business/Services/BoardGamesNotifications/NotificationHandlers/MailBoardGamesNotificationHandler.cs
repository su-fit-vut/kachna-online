using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using KachnaOnline.Business.Configuration;
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
        private readonly IOptionsMonitor<MailOptions> _smtpOptionsMonitor;
        private readonly ILogger<MailBoardGamesNotificationHandler> _logger;
        private readonly IBoardGamesService _boardGamesService;
        private readonly IUserService _userService;

        public MailBoardGamesNotificationHandler(IOptionsMonitor<MailOptions> smtpOptionsMonitor,
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
            // TODO: Cache SMTP clients. Making a new one for each e-mail may lead to starvation.

            var config = _smtpOptionsMonitor.CurrentValue;
            if (string.IsNullOrEmpty(config.FromAddress) || string.IsNullOrEmpty(config.Host))
            {
                _logger.LogError("SMTP authentication not available.");
                return;
            }

            var fromAddress = new MailAddress(config.FromAddress, config.DisplayName);
            var toAddress = new MailAddress(user.Email, user.Name);

            using var smtp = new SmtpClient()
            {
                Host = config.Host,
                Port = config.Port,
                EnableSsl = config.UseSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network
            };

            if (!string.IsNullOrEmpty(config.Username) || !string.IsNullOrEmpty(config.Password))
            {
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(config.Username, config.Password);
            }

            var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = content,
                IsBodyHtml = true
            };

            _logger.LogDebug("Sending an e-mail to {ToAddress}.", toAddress.Address);
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
Tv?? v??p??j??ka deskov?? hry <b>{game.Name}</b> ji?? brzy vypr????, konkr??tn??
<b>{expiration:dd. MM. yyyy}</b>.
Domluv se, pros??m, s n??k??m ze Studentsk?? unie na vr??cen?? hry zp??t do klubu. V p????pad??, ??e
se ti hra zal??bila a r??d bys ji m??l*a p??j??enou je??t?? d??le, m????e?? na webu Kachna Online po????dat
o prodlou??en?? nebo ti ji m????e prodlou??it ??len Studentsk?? unie, pokud se s n??m domluv????.<br><br>
Tvoje Kachna Online";
                await this.SendEmail("V??p??j??n?? doba deskov?? hry v klubu U Kachni??ky brzy vypr????", message, user);
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
Tv?? v??p??j??ka deskov?? hry <b>{game.Name}</b> vypr??ela.
Domluv se, pros??m, s n??k??m ze Studentsk?? unie na vr??cen?? hry zp??t do klubu. V p????pad??, ??e
se ti hra zal??bila a r??d bys ji m??l*a p??j??enou je??t?? d??le, m????e?? na webu Kachna Online po????dat
o prodlou??en?? nebo ti ji m????e prodlou??it ??len Studentsk?? unie, pokud se s n??m domluv????.<br><br>
Tvoje Kachna Online";
                await this.SendEmail("V??p??j??n?? doba deskov?? hry v klubu U Kachni??ky vypr??ela", message, user);
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
