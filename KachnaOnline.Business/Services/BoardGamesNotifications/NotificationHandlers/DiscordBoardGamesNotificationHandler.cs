// DiscordBoardGamesNotificationHandler.cs
// Author: František Nečas

using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using KachnaOnline.Business.Configuration;
using KachnaOnline.Business.Exceptions.BoardGames;
using KachnaOnline.Business.Models.Discord;
using KachnaOnline.Business.Services.Abstractions;
using KachnaOnline.Business.Services.BoardGamesNotifications.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace KachnaOnline.Business.Services.BoardGamesNotifications.NotificationHandlers
{
    /// <summary>
    /// Implements a handler sending messages to Discord via a webhook.
    /// </summary>
    public class DiscordBoardGamesNotificationHandler : IBoardGamesNotificationHandler
    {
        private readonly IOptionsMonitor<BoardGamesOptions> _boardGamesOptionsMonitor;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<DiscordBoardGamesNotificationHandler> _logger;
        private readonly IBoardGamesService _boardGamesService;
        private readonly IUserService _userService;

        public DiscordBoardGamesNotificationHandler(IOptionsMonitor<BoardGamesOptions> boardGamesOptionsMonitor,
            IHttpClientFactory httpClientFactory, ILogger<DiscordBoardGamesNotificationHandler> logger,
            IBoardGamesService boardGamesService, IUserService userService)
        {
            _boardGamesOptionsMonitor = boardGamesOptionsMonitor;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _boardGamesService = boardGamesService;
            _userService = userService;
        }

        /// <summary>
        /// Sends a webhook message to SU Discord.
        /// </summary>
        /// <param name="message">Message to send.</param>
        /// <param name="wait">Whether to wait for server message confirmation. False by default.</param>
        /// <returns>Message model if <paramref name="wait"/> was true and the request succeeded, null otherwise.</returns>
        private async Task<DiscordMessage> SendWebhookMessage(string message, bool wait = false)
        {
            if (string.IsNullOrEmpty(_boardGamesOptionsMonitor.CurrentValue.SuWebhookUrl) || message == null)
            {
                _logger.LogError("Webhook URL not available");
                return null;
            }

            using var client = _httpClientFactory.CreateClient();
            var content = $"{{ \"content\": \"{message}\" }}";
            var url = _boardGamesOptionsMonitor.CurrentValue.SuWebhookUrl;
            var waitString = wait.ToString().ToLower();
            var response = await client.PostAsync($"{url}?wait={waitString}",
                new StringContent(content, Encoding.UTF8, "application/json"));
            if (response.StatusCode != HttpStatusCode.OK)
            {
                return null;
            }

            var responseData = await response.Content.ReadAsStringAsync();
            _logger.LogDebug("Webhook response payload: {data}", responseData);
            return JsonConvert.DeserializeObject<DiscordMessage>(responseData);
        }

        /// <summary>
        /// Deletes a webhook message
        /// </summary>
        /// <param name="messageId">ID of the message to delete.</param>
        private async Task DeleteWebhookMessage(ulong messageId)
        {
            if (string.IsNullOrEmpty(_boardGamesOptionsMonitor.CurrentValue.SuWebhookUrl))
            {
                _logger.LogError("Webhook URL not available");
                return;
            }

            var baseUrl = _boardGamesOptionsMonitor.CurrentValue.SuWebhookUrl;
            using var client = _httpClientFactory.CreateClient();
            await client.DeleteAsync($"{baseUrl}/messages/{messageId}");
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
                var msg = $"Uživatel {name} právě zažádal o prodloužení rezervace na hru {game.Name}, která končí {item.ExpiresOn}.";
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
