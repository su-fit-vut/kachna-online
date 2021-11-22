// StartupExtensions.cs
// Author: Ondřej Ondryáš

using System;
using System.Net.Http.Headers;
using KachnaOnline.Business.Configuration;
using KachnaOnline.Business.Constants;
using KachnaOnline.Business.Facades;
using KachnaOnline.Business.Mappings;
using KachnaOnline.Business.Services;
using KachnaOnline.Business.Services.Abstractions;
using KachnaOnline.Business.Services.BoardGamesNotifications;
using KachnaOnline.Business.Services.BoardGamesNotifications.Abstractions;
using KachnaOnline.Business.Services.BoardGamesNotifications.NotificationHandlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KachnaOnline.Business.Extensions
{
    public static class StartupExtensions
    {
        public static void AddBusinessLayer(this IServiceCollection services, IConfiguration configuration)
        {
            // Add AutoMapper and load mapping profiles from this assembly.
            services.AddAutoMapper(typeof(UserMappings), typeof(KisMappings), typeof(BoardGamesMappings));

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
            
            // Add notification service
            services.AddTransient<IBoardGamesNotificationHandler, DiscordBoardGamesNotificationHandler>();
            services.AddTransient<IBoardGamesNotificationHandler, MailBoardGamesNotificationHandler>();
            services.AddScoped<IBoardGamesNotificationService, BoardGamesNotificationService>();
            services.AddHostedService<BoardGamesNotificationBackgroundService>();

            // Add custom services.
            services.AddScoped<IKisService, KisService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IBoardGamesService, BoardGamesService>();

            // Add facades.
            services.AddScoped<ClubInfoFacade>();
            services.AddScoped<BoardGamesFacade>();
        }
    }
}
