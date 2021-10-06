// StartupExtensions.cs
// Author: Ondřej Ondryáš

using System;
using KachnaOnline.Business.Configuration;
using KachnaOnline.Business.Services;
using KachnaOnline.Business.Services.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KachnaOnline.Business.Extensions
{
    public static class StartupExtensions
    {
        public static void AddBusinessLayer(this IServiceCollection services, IConfiguration configuration)
        {
            // Add AutoMapper and load mapping profiles from this assembly.
            services.AddAutoMapper(options => { options.AddMaps("KachnaOnline.Business"); });

            // Add KIS HTTP client factory.
            var kisUrl = configuration.GetSection("Kis").Get<KisOptions>().ApiBaseUrl;
            if (!Uri.TryCreate(kisUrl, UriKind.RelativeOrAbsolute, out var kisUri))
            {
                throw new Exception("Invalid KIS configuration: invalid KIS API URL.");
            }

            services.AddHttpClient("kis", client => client.BaseAddress = kisUri);
            
            // Add custom services.
            services.AddScoped<IKisService, KisService>();
            services.AddScoped<IUserService, UserService>();
        }
    }
}
