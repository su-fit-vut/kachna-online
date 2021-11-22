// BoardGamesNotificationBackgroundService.cs
// Author: František Nečas

using System;
using System.Threading;
using System.Threading.Tasks;
using KachnaOnline.Business.Configuration;
using KachnaOnline.Business.Data.Repositories.Abstractions;
using KachnaOnline.Business.Services.BoardGamesNotifications.Abstractions;
using KachnaOnline.Business.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KachnaOnline.Business.Services.BoardGamesNotifications
{
    /// <summary>
    /// Represents a background service that periodically checks for expired or soon to be expired
    /// reservation items and fires corresponding events through <see cref="IBoardGamesNotificationService"/>.
    /// </summary>
    public class BoardGamesNotificationBackgroundService : BackgroundService
    {
        private readonly ILogger<BoardGamesNotificationBackgroundService> _logger;
        private readonly IOptionsMonitor<BoardGamesOptions> _optionsMonitor;
        private readonly IServiceProvider _serviceProvider;

        public BoardGamesNotificationBackgroundService(ILogger<BoardGamesNotificationBackgroundService> logger,
            IServiceProvider serviceProvider, IOptionsMonitor<BoardGamesOptions> optionsMonitor)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _optionsMonitor = optionsMonitor;
        }

        /// <inheritdoc />
        /// <remarks>
        /// This method checks for two types of events:
        ///     1) expired reservation items
        ///     2) items which will expire soon (specified by <see cref="BoardGamesOptions"/>).
        ///
        /// It checks for these events in specified intervals.
        /// </remarks>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting background notification service.");

            while (!stoppingToken.IsCancellationRequested)
            {
                await using (var scope = _serviceProvider.CreateAsyncScope())
                {
                    var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                    var itemRepository = uow.ReservationItems;
                    // Expired items
                    foreach (var item in await itemRepository.GetExpiredUnnotified())
                    {
                        TaskUtils.FireAndForget(_serviceProvider, _logger, async (services, _) =>
                        {
                            var notificationService = services.GetRequiredService<IBoardGamesNotificationService>();
                            await notificationService.TriggerReservationItemExpired(item.Id);
                        });
                        item.NotifiedOnExpiration = true;
                        try
                        {
                            await uow.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(e, "Failed to save notification flag to database.");
                        }
                    }

                    // Items about to expire.
                    var targetNotificationDate =
                        DateTime.Now.Add(TimeSpan.FromDays(_optionsMonitor.CurrentValue.NotifyBeforeExpirationDays));
                    foreach (var item in await itemRepository.GetExpiredUnnotified(targetNotificationDate))
                    {
                        TaskUtils.FireAndForget(_serviceProvider, _logger, async (services, _) =>
                        {
                            var notificationService = services.GetRequiredService<IBoardGamesNotificationService>();
                            await notificationService.TriggerReservationItemExpiresSoon(item.Id);
                        });
                        item.NotifiedBeforeExpiration= true;
                        try
                        {
                            await uow.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(e, "Failed to save notification flag to database.");
                        }
                        
                    }
                }

                var delay = TimeSpan.FromMinutes(_optionsMonitor.CurrentValue.NotificationServiceIntervalMinutes);
                _logger.LogDebug("Notification service waiting for {delay} before checking again.", delay);
                await Task.Delay(delay, stoppingToken);
            }
        }
    }
}
