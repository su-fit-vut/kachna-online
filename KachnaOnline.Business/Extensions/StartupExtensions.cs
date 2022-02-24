using System;
using System.Net.Http.Headers;
using KachnaOnline.Business.Configuration;
using KachnaOnline.Business.Constants;
using KachnaOnline.Business.Facades;
using KachnaOnline.Business.Mappings;
using KachnaOnline.Business.Services;
using KachnaOnline.Business.Services.Abstractions;
using KachnaOnline.Business.Services.StatePlanning;
using KachnaOnline.Business.Services.StatePlanning.Abstractions;
using KachnaOnline.Business.Services.StatePlanning.TransitionHandlers;
using KachnaOnline.Business.Services.BoardGamesNotifications;
using KachnaOnline.Business.Services.BoardGamesNotifications.Abstractions;
using KachnaOnline.Business.Services.BoardGamesNotifications.NotificationHandlers;
using Lib.Net.Http.WebPush;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KachnaOnline.Business.Extensions
{
    public static class StartupExtensions
    {
        public static void AddBusinessLayer(this IServiceCollection services, IConfiguration configuration)
        {
            // Add AutoMapper and load mapping profiles from this assembly.
            services.AddAutoMapper(typeof(UserMappings), typeof(KisMappings), typeof(BoardGamesMappings),
                typeof(EventMappings), typeof(PushSubscriptionMappings));

            // Add KIS HTTP client factory.
            var kisOptions = configuration.GetSection("Kis").Get<KisOptions>();
            var kisUrl = kisOptions.ApiBaseUrl;
            var kisDisplayToken = kisOptions.DisplayToken;
            if (!Uri.TryCreate(kisUrl, UriKind.RelativeOrAbsolute, out var kisUri))
            {
                throw new Exception("Invalid KIS configuration: invalid KIS API URL.");
            }

            // Add unauthorized client (used for making login requests).
            services.AddHttpClient(KisConstants.KisHttpClient, client => client.BaseAddress = kisUri);
            // Add client authorized using the configured display token (used for offer and leaderboard requests).
            services.AddHttpClient(KisConstants.KisDisplayHttpClient, client =>
            {
                client.BaseAddress = kisUri;
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", kisDisplayToken);
            });

            // Add memory cache.
            services.AddMemoryCache();

            // Add HTTP context accessor.
            services.AddHttpContextAccessor();

            // Add push notification service
            services.AddScoped<PushServiceClient>();

            // Add notification service
            services.AddTransient<IBoardGamesNotificationHandler, DiscordBoardGamesNotificationHandler>();
            services.AddTransient<IBoardGamesNotificationHandler, MailBoardGamesNotificationHandler>();
            services.AddTransient<IBoardGamesNotificationHandler, PushBoardGamesNotificationHandler>();
            services.AddScoped<IBoardGamesNotificationService, BoardGamesNotificationService>();
            services.AddHostedService<BoardGamesNotificationBackgroundService>();

            // Add state planner.
            services.AddTransient<IStateTransitionHandler, SaveEndedDateTimeTransitionHandler>();
            services.AddTransient<IStateTransitionHandler, SuDiscordTransitionHandler>();
            services.AddTransient<IStateTransitionHandler, FitwideDiscordTransitionHandler>();
            services.AddTransient<IStateTransitionHandler, PushNotificationTransitionHandler>();
            services.AddScoped<IStateTransitionService, StateTransitionService>();
            services.AddSingleton<IStatePlannerService, StatePlannerService>();
            services.AddHostedService<StatePlannerBackgroundService>();

            // Add custom services.
            services.AddScoped<IKisService, KisService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IClubStateService, ClubStateService>();
            services.AddScoped<IBoardGamesService, BoardGamesService>();
            services.AddScoped<IEventsService, EventsService>();
            services.AddScoped<IPushSubscriptionsService, PushSubscriptionsService>();

            // Add facades.
            services.AddScoped<ClubInfoFacade>();
            services.AddScoped<ClubStateFacade>();
            services.AddScoped<RepeatingStatesFacade>();
            services.AddScoped<PushSubscriptionsFacade>();
            services.AddScoped<BoardGamesFacade>();
            services.AddScoped<EventsFacade>();
            services.AddScoped<UserFacade>();
            services.AddScoped<ImagesFacade>();
        }
    }
}
