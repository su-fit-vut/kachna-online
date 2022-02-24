using System;
using System.Threading.Tasks;
using KachnaOnline.Business.Configuration;
using KachnaOnline.Business.Exceptions.BoardGames;
using KachnaOnline.Business.Services.Abstractions;
using KachnaOnline.Business.Services.BoardGamesNotifications.Abstractions;
using KachnaOnline.Business.Services.Push;
using Lib.Net.Http.WebPush;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KachnaOnline.Business.Services.BoardGamesNotifications.NotificationHandlers
{
    public class PushBoardGamesNotificationHandler : PushNotificationClient, IBoardGamesNotificationHandler
    {
        private readonly IBoardGamesService _boardGamesService;
        private readonly IPushSubscriptionsService _pushSubscriptionsService;
        private readonly ILogger<PushBoardGamesNotificationHandler> _logger;

        public PushBoardGamesNotificationHandler(IOptionsMonitor<PushOptions> pushOptions, PushServiceClient pushClient,
            IBoardGamesService boardGamesService, IPushSubscriptionsService pushSubscriptionsService,
            ILogger<PushBoardGamesNotificationHandler> logger) : base(pushOptions, pushClient, logger)
        {
            _boardGamesService = boardGamesService;
            _pushSubscriptionsService = pushSubscriptionsService;
            _logger = logger;
        }

        public Task PerformReservationCreated(int reservationId)
        {
            return Task.CompletedTask;
        }

        public Task PerformReservationFullyAssigned(int reservationId)
        {
            return Task.CompletedTask;
        }

        public Task PerformReservationItemExtensionRequest(int itemId)
        {
            return Task.CompletedTask;
        }

        public async Task PerformReservationItemExpiresSoon(int itemId)
        {
            _logger.LogDebug("Sending push notification, reservation item expires soon.");
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
                var expiration = item.ExpiresOn.Value;
                var subscriptions =
                    await _pushSubscriptionsService.GetUserBoardGamesEnabledSubscriptions(reservation.MadeById);
                var message = $"Tvá výpůjčka hry {game.Name} vyprší {expiration:dd. MM.}. " +
                              $"Domluv se s někým z SU na vrácení nebo požádej o prodloužení.";
                foreach (var subscription in subscriptions)
                {
                    try
                    {
                        await this.SendNotification(subscription, "Blíží se konec výpůjční doby deskovky", message);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Cannot send a push notification.");
                    }
                }
            }
            catch (ReservationNotFoundException)
            {
                // Should not happen, just in case
                _logger.LogError("Failed to send notification about near expiration, reservation not found.");
            }
            catch (BoardGameNotFoundException)
            {
                // Should not happen, just in case
                _logger.LogError("Failed to send notification about near expiration, board game not found.");
            }
        }

        public async Task PerformReservationItemExpired(int itemId)
        {
            _logger.LogDebug("Sending push notification, reservation item expired soon.");
            var item = await _boardGamesService.GetReservationItem(itemId);
            if (item?.ExpiresOn is null)
            {
                _logger.LogError("Item expired not found in DB.");
                return;
            }

            try
            {
                var game = await _boardGamesService.GetBoardGame(item.BoardGameId);
                var reservation = await _boardGamesService.GetReservation(item.ReservationId);
                var subscriptions =
                    await _pushSubscriptionsService.GetUserBoardGamesEnabledSubscriptions(reservation.MadeById);
                var message = $"Tvá výpůjčka hry {game.Name} vypršela. " +
                              $"Domluv se s někým z SU na vrácení nebo požádej o prodloužení.";
                foreach (var subscription in subscriptions)
                {
                    await this.SendNotification(subscription, "Výpůjční doba deskovky vypršela", message);
                }
            }
            catch (ReservationNotFoundException)
            {
                // Should not happen, just in case
                _logger.LogError("Failed to send notification about expiration, reservation not found.");
            }
            catch (BoardGameNotFoundException)
            {
                // Should not happen, just in case
                _logger.LogError("Failed to send notification about expiration, board game not found.");
            }
        }
    }
}
