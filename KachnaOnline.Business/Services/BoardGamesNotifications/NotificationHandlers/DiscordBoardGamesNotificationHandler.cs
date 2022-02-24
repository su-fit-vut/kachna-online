using System.Net.Http;
using System.Threading.Tasks;
using KachnaOnline.Business.Configuration;
using KachnaOnline.Business.Exceptions.BoardGames;
using KachnaOnline.Business.Services.Abstractions;
using KachnaOnline.Business.Services.BoardGamesNotifications.Abstractions;
using KachnaOnline.Business.Services.Discord;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KachnaOnline.Business.Services.BoardGamesNotifications.NotificationHandlers
{
    /// <summary>
    /// Implements a handler sending messages to Discord via a webhook.
    /// </summary>
    public class DiscordBoardGamesNotificationHandler : DiscordWebhookClient, IBoardGamesNotificationHandler
    {
        private readonly ILogger<DiscordBoardGamesNotificationHandler> _logger;
        private readonly IBoardGamesService _boardGamesService;
        private readonly IUserService _userService;

        public DiscordBoardGamesNotificationHandler(IOptionsMonitor<BoardGamesOptions> boardGamesOptionsMonitor,
            IHttpClientFactory httpClientFactory, ILogger<DiscordBoardGamesNotificationHandler> logger,
            IBoardGamesService boardGamesService, IUserService userService)
            : base(boardGamesOptionsMonitor.CurrentValue.SuDiscordWebhookUrl, httpClientFactory, logger)
        {
            _logger = logger;
            _boardGamesService = boardGamesService;
            _userService = userService;
        }

        /// <inheritdoc />
        /// <remarks>
        /// Sends a discord message to SU discord about the new reservation and saves its ID
        /// for later deletion when it's completely assigned.
        /// </remarks>
        public async Task PerformReservationCreated(int reservationId)
        {
            try
            {
                var reservation = await _boardGamesService.GetReservation(reservationId);
                var items = await _boardGamesService.GetReservationItems(reservationId);
                var user = await _userService.GetUser(reservation.MadeById);

                var name = user is null ? "" : user.Name;
                var msg = $"Uživatel {name} právě vytvořil novou rezervaci s hrami:";
                foreach (var item in items)
                {
                    try
                    {
                        var game = await _boardGamesService.GetBoardGame(item.BoardGameId);
                        msg += $"\\n - {game.Name}";
                    }
                    catch (BoardGameNotFoundException)
                    {
                        _logger.LogError("Reserved board game not found while sending Discord message.");
                    }
                }

                var message = await this.SendWebhookMessage(msg, true);
                if (message is not null)
                {
                    await _boardGamesService.UpdateReservationDiscordMessageId(reservationId, message.Id);
                }
            }
            catch (ReservationNotFoundException)
            {
                // Should not happen, just in case.
                _logger.LogError("Failed to send Discord message, reservation not found.");
            }
            catch (ReservationManipulationFailedException)
            {
                _logger.LogError("Failed to save Discord webhook message ID.");
            }
        }

        /// <inheritdoc />
        /// <remarks>
        /// Removes a previously sent message from SU Discord.
        /// </remarks>
        public async Task PerformReservationFullyAssigned(int reservationId)
        {
            try
            {
                var reservation = await _boardGamesService.GetReservation(reservationId);
                if (reservation.WebhookMessageId is not null)
                {
                    await this.DeleteWebhookMessage(reservation.WebhookMessageId.Value);
                }
            }
            catch (ReservationNotFoundException)
            {
                // Should not happen, just in case
                _logger.LogError("Failed to delete webhook message, reservation not found.");
            }
        }

        /// <inheritdoc />
        /// <remarks>
        /// Sends a message about the extension request to SU discord.
        /// </remarks>
        public async Task PerformReservationItemExtensionRequest(int itemId)
        {
            var item = await _boardGamesService.GetReservationItem(itemId);
            if (item is null)
            {
                _logger.LogError("Item with requested extension not found in DB.");
                return;
            }

            try
            {
                var game = await _boardGamesService.GetBoardGame(item.BoardGameId);
                var reservation = await _boardGamesService.GetReservation(item.ReservationId);
                var user = await _userService.GetUser(reservation.MadeById);

                var name = user is null ? "" : user.Name;
                var msg = $"Uživatel {name} právě zažádal o prodloužení rezervace na hru {game.Name}";
                if (item.ExpiresOn.HasValue)
                {
                    var expiration = item.ExpiresOn.Value;
                    msg += $", jejíž platnost končí {expiration.Day}. {expiration.Month}. {expiration.Year}.";
                }
                else
                {
                    msg += ".";
                }

                await this.SendWebhookMessage(msg);
            }
            catch (ReservationNotFoundException)
            {
                // Should not happen, just in case
                _logger.LogError("Failed to send message about extension request, reservation not found.");
            }
            catch (BoardGameNotFoundException)
            {
                // Should not happen, just in case
                _logger.LogError("Failed to send message about extension request, board game not found.");
            }
        }

        /// <inheritdoc />
        /// <remarks>
        /// Does nothing.
        /// </remarks>
        public Task PerformReservationItemExpiresSoon(int itemId)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        /// <remarks>
        /// Sends a message about the expiration to SU discord.
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

                var name = user is null ? "" : user.Name;
                var msg = $"Uživateli {name} právě vypršela rezervace na hru {game.Name}.";
                await this.SendWebhookMessage(msg);
            }
            catch (ReservationNotFoundException)
            {
                // Should not happen, just in case
                _logger.LogError("Failed to send message about reservation expiration, reservation not found.");
            }
            catch (BoardGameNotFoundException)
            {
                // Should not happen, just in case
                _logger.LogError("Failed to send message about extension request, board game not found.");
            }
        }
    }
}
