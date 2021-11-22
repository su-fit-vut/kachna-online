// BoardGamesNotificationService.cs
// Author: František Nečas

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KachnaOnline.Business.Services.BoardGamesNotifications.Abstractions;
using Microsoft.Extensions.Logging;

namespace KachnaOnline.Business.Services.BoardGamesNotifications
{
    public class BoardGamesNotificationService : IBoardGamesNotificationService
    {
        private readonly IBoardGamesNotificationHandler[] _notificationHandlers;
        private readonly ILogger<BoardGamesNotificationService> _logger;

        public BoardGamesNotificationService(IEnumerable<IBoardGamesNotificationHandler> notificationHandlers,
            ILogger<BoardGamesNotificationService> logger)
        {
            _notificationHandlers = notificationHandlers.ToArray();
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task TriggerReservationCreated(int reservationId)
        {
            _logger.LogDebug("Processing trigger actions for the reservation creation of reservation {ReservationId}",
                reservationId);
            foreach (var notificationHandler in _notificationHandlers)
            {
                try
                {
                    await notificationHandler.PerformReservationCreated(reservationId);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "An error occurred when performing reservation created action {ActionName}",
                        notificationHandler.GetType().Name);
                }
            }
        }

        /// <inheritdoc />
        public async Task TriggerReservationFullyAssigned(int reservationId)
        {
            _logger.LogDebug("Processing trigger actions for the full assignment of reservation {ReservationId}",
                reservationId);
            foreach (var notificationHandler in _notificationHandlers)
            {
                try
                {
                    await notificationHandler.PerformReservationFullyAssigned(reservationId);
                }
                catch (Exception e)
                {
                    _logger.LogError(e,
                        "An error occurred when performing reservation full assignment action {ActionName}",
                        notificationHandler.GetType().Name);
                }
            }
        }

        /// <inheritdoc />
        public async Task TriggerReservationItemExtensionRequest(int itemId)
        {
            _logger.LogDebug("Processing trigger actions for the reservation item extension of item {ItemId}", itemId);
            foreach (var notificationHandler in _notificationHandlers)
            {
                try
                {
                    await notificationHandler.PerformReservationItemExtensionRequest(itemId);
                }
                catch (Exception e)
                {
                    _logger.LogError(e,
                        "An error occurred when performing reservation extension request action {ActionName}",
                        notificationHandler.GetType().Name);
                }
            }
        }

        /// <inheritdoc />
        public async Task TriggerReservationItemExpiresSoon(int itemId)
        {
            _logger.LogDebug("Processing trigger actions for the near expiration of reservation item {ItemId}", itemId);
            foreach (var notificationHandler in _notificationHandlers)
            {
                try
                {
                    await notificationHandler.PerformReservationItemExpiresSoon(itemId);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "An error occurred when performing near expiration action {ActionName}",
                        notificationHandler.GetType().Name);
                }
            }
        }

        /// <inheritdoc />
        public async Task TriggerReservationItemExpired(int itemId)
        {
            _logger.LogDebug("Processing trigger actions for the expiration of reservation item {ItemId}", itemId);
            foreach (var notificationHandler in _notificationHandlers)
            {
                try
                {
                    await notificationHandler.PerformReservationItemExpired(itemId);
                }
                catch (Exception e)
                {
                    _logger.LogError(e,
                        "An error occurred when performing reservation item expiration action {ActionName}",
                        notificationHandler.GetType().Name);
                }
            }
        }
    }
}
